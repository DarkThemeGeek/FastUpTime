using System.Diagnostics;
using FastUpTime.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastUpTime.Services.BackgroundServices;

public class SitePingWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SitePingWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            var monitoring = scope.ServiceProvider
                .GetRequiredService<ISiteMonitoringService>();

            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var sites = await db.Sites
                .Select(x => x.Id)
                .ToListAsync(stoppingToken);

            foreach (var siteId in sites)
            {
                await monitoring.PingSiteAsync(siteId);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}