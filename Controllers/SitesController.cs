using System.Net;
using System.Net.NetworkInformation;
using FastUpTime.Data;
using FastUpTime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastUpTime.Controllers;

[Route("/")]
[ApiController]
public class SitesController : ControllerBase
{
    // private readonly AppDbContext _dbContext;
    // public SitesController(AppDbContext dbContext) => _dbContext = dbContext;

 
[HttpGet]
    public async Task<IActionResult> GetSiteUp(int x)
    {
        
            var isSiteUp = await IsWebsiteUp("http://www.sda.fgesdfg/");
            return Ok(isSiteUp);    
        
    }
    
    public async Task<bool> IsWebsiteUp(string url)
    {
    
        var hostName = new Uri(url).Host;
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
                Console.Write("Url Down:"+url);
                return false;
            }

            Console.Write(e);
            return false;
        }
    }
 

}