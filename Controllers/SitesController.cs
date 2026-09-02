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
[Route("sites/[controller]")]
public class SitesController : ControllerBase
{
    private readonly UserDbContext _dbContext;
    public SitesController(UserDbContext dbContext) => _dbContext = dbContext;

    [Authorize]
    [HttpPost("/add")]
    public async Task<IActionResult> AddSiteToAccout([FromBody] Site site)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdString, out var userId))
            return Unauthorized();

        if (string.IsNullOrEmpty(site.Url) )
        {
            return BadRequest("body empty");
        }
        
        if (!Uri.TryCreate(site.Url, UriKind.Absolute, out var uri))
        {
            return BadRequest("Invalid URL.");
        }
        
        
        var InDbsite = _dbContext.Sites.FirstOrDefault(x => x.Url == site.Url);
        var account = _dbContext.UserAccounts.FirstOrDefault(x => x.Id == userId);
        
        if (account == null)
            return Unauthorized();


        if (InDbsite != null)
        {
            await _dbContext.Sites.AddAsync(InDbsite);
            account.siteIDs?.Add(InDbsite.Id);
        }
        else
        {
            account.siteIDs?.Add(site.Id);
        }

        _dbContext.UserAccounts.Update(account);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [Authorize]
    [Route("/checkId/{x:int}")]
    [HttpGet]
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
    [Route("/checkUrl/{x:alpha}")]
    [HttpGet]
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