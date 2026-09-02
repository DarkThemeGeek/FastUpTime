using FastUpTime.Models;

namespace FastUpTime.Services;

public interface ISiteMonitoringService
{
    Task<SitePing> PingSiteAsync(long siteId);
}
