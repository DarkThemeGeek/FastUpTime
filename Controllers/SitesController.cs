using System.Net;
using System.Net.NetworkInformation;
using System.Security.Claims;
using FastUpTime.Data;
using FastUpTime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastUpTime.Controllers;

[ApiController]
[Route("[controller]")]
public class SitesController : ControllerBase
{
    private readonly UserDbContext _dbContext;
    public SitesController(UserDbContext dbContext) => _dbContext = dbContext;
    [HttpGet]
    //[Authorize("Admin")]
    public async Task<List<Site>> GetAll()
    {
        return await _dbContext.Sites.ToListAsync();
    }

    [Authorize]
    [HttpGet("get_update")]
    public async Task<IActionResult> GetAllSitesUpdate()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
            return Unauthorized();
        
        var account = await _dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.Id ==userId );
        
        if (account == null)
            return Unauthorized();
        if (account.siteIDs == null)
            return BadRequest("No sites added");

        return Ok();

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

        var account = await _dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (account == null)
            return Unauthorized();

        // Check if the site already exists
        var existingSite = await _dbContext.Sites
            .FirstOrDefaultAsync(x => x.Url == site.Url);

        if (existingSite == null)
        {
            // Site doesn't exist, so add it
            existingSite = new Site
            {
                Url = site.Url,
            };

            await _dbContext.Sites.AddAsync(existingSite);
            await _dbContext.SaveChangesAsync();
        }

        // Make sure the account has a site ID list
        account.siteIDs ??= new List<long>();

        // Don't add the same site twice
        if (!account.siteIDs.Contains(existingSite.Id))
        {
            account.siteIDs.Add(existingSite.Id);
        }

        await _dbContext.SaveChangesAsync();

        return Ok();
    }


    [Authorize]
    [HttpGet("checkId/{x:int}")]
    public async Task<IActionResult> GetSiteUp(int x)
    {
        Site? site = _dbContext.Sites.FirstOrDefaultAsync((site) => site.Id == x).Result;
        if (site == null)
        {
            return BadRequest("Site does not exist");
        }

        var isSiteUp = await IsWebsiteUp(site.Url);
        return Ok(isSiteUp);
    }

    [Authorize]
    [HttpGet("checkUrl/{x:alpha}")]
    public async Task<IActionResult> GetSiteUp(string x)
    {
        if (string.IsNullOrWhiteSpace(x))
        {
            return BadRequest("Invalid string");
        }

        var isSiteUp = await IsWebsiteUp(x);
        return Ok(isSiteUp);
    }

    public async Task<bool> IsWebsiteUp(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return false;
        }

        var hostName = uri.Host;
        var ping = new Ping();
        try
        {
            // SendPingAsync with a timeout (e.g., 3000ms)
            PingReply result = await ping.SendPingAsync(hostName, 3000);
            return result.Status == IPStatus.Success;
        }
        catch (Exception e)
        {
            if (e is PingException)
            {
                Console.Write("Url Down:" + url);
                return false;
            }

            Console.Write(e);
            return false;
        }
    }
}