using System.Net;
using System.Net.NetworkInformation;
using System.Security.Claims;
using FastUpTime.Data;
using FastUpTime.Models;
using FastUpTime.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastUpTime.Controllers;

[ApiController]
[Route("[controller]")]
public class SitesController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    public SitesController(AppDbContext dbContext) => _dbContext = dbContext;

    [HttpGet]
    //[Authorize("Admin")]
    public async Task<List<Site>> GetAll()
    {
        return await _dbContext.Sites.ToListAsync();
    }


    [Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> AddSiteToAccount([FromBody] Site site)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdString, out var userId))
            return Unauthorized();

        if (site == null || string.IsNullOrWhiteSpace(site.Url))
            return BadRequest("URL is required.");

        if (!Uri.TryCreate(site.Url, UriKind.Absolute, out var uri))
            return BadRequest("Invalid URL.");

        var account = await _dbContext.UserAccounts.FirstOrDefaultAsync(x => x.Id == userId);

        if (account == null)
            return Unauthorized();


        var existingSite = await _dbContext.Sites.FirstOrDefaultAsync(x => x.Url == site.Url);

 
        if (existingSite == null)
        {
            existingSite = new Site
            {
                Url = site.Url
            };

            _dbContext.Sites.Add(existingSite);

            // Save so the database generates the ID
            await _dbContext.SaveChangesAsync();
        }

      
        var relationship = await _dbContext.UserAccountSites
            .FirstOrDefaultAsync(x =>
                x.UserAccountId == userId &&
                x.SiteId == existingSite.Id);

        if (relationship != null)
            return Conflict("Account already monitors this site.");


        var accountSite = new UserAccountSite
        {
            UserAccountId = userId,
            SiteId = existingSite.Id,
            PingIntervalSeconds = 60,
            Enabled = true
        };

        _dbContext.UserAccountSites.Add(accountSite);

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [Authorize]
    [HttpGet("check/{siteId:long}")]
    public async Task<IActionResult> GetSiteUp(long siteId, [FromServices] ISiteMonitoringService monitoring)
    {
        try
        {
            var ping = await monitoring.PingSiteAsync(siteId);

            return Ok(ping);
        }
        catch (ArgumentException)
        {
            return NotFound("Site not found.");
        }
    }

    [Authorize]
    [HttpGet("get_all_sites")]
    public async Task<IActionResult> GetSitesStatus([FromServices] ISiteMonitoringService monitoring)
    {
        
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdString, out var userId))
            return Unauthorized();

        var account = await _dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (account == null)
            return Unauthorized();

        var sites = await _dbContext.Sites
            .Where(x => x.Id == userId)
            .ToListAsync();
        return Ok();
    }
    
    [Authorize]
    [HttpGet("get_sites")]
    public async Task<IActionResult> GetRecentPings()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdString, out var userId))
            return Unauthorized();
        
        var account = await _dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (account == null)
            return Unauthorized();
        
        var sites = await _dbContext.UserAccountSites
            .Where(x => x.UserAccountId == userId)
            .Select(x => new
            {
                SiteId = x.Site.Id,
                Url = x.Site.Url,

                Pings = x.Site.Pings
                    .OrderByDescending(p => p.Timestamp)
                    .Take(20)
                    .ToList()
            })
            .ToListAsync();
        
        return Ok(sites);
    }
    [Authorize]
    [HttpGet("get_last")]
    public async Task<IActionResult> GetLastPings()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdString, out var userId))
            return Unauthorized();
        
        var account = await _dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (account == null)
            return Unauthorized();

        var sites = await _dbContext.UserAccountSites
            .Where(x => x.UserAccountId == userId)
            .Select(x => new
            {
                SiteId = x.Site.Id,
                Url = x.Site.Url,

                Pings = x.Site.Pings
                    .OrderByDescending(p => p.Timestamp)
                    .Take(1)
                    .ToList()
            })
            .ToListAsync();
                
        return Ok(sites);
    }
    
}