using System.Diagnostics;
using FastUpTime.Data;
using FastUpTime.Models;
using Microsoft.EntityFrameworkCore;

namespace FastUpTime.Services;
public class SiteMonitoringService : ISiteMonitoringService
{
    private readonly AppDbContext _dbContext;
    private readonly HttpClient _httpClient;

    public SiteMonitoringService(
        AppDbContext dbContext,
        HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
    }

    public async Task<SitePing> PingSiteAsync(long siteId)
    {
        var site = await _dbContext.Sites
            .FirstOrDefaultAsync(x => x.Id == siteId);

        if (site == null)
            throw new ArgumentException("Site not found.");

        var stopwatch = Stopwatch.StartNew();

        bool isUp = false;
        long? responseTime = null;

        try
        {
            using var response = await _httpClient.GetAsync(
                site.Url,
                HttpCompletionOption.ResponseHeadersRead);

            stopwatch.Stop();

            responseTime = stopwatch.ElapsedMilliseconds;
            isUp = response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            stopwatch.Stop();
        }

        var ping = new SitePing
        {
            SiteId = site.Id,
            Timestamp = DateTime.UtcNow,
            Success = isUp,
            ResponseTimeMs = responseTime
        };

        await _dbContext.SitePings.AddAsync(ping);
        await _dbContext.SaveChangesAsync();
 
        return ping;
    }
}
