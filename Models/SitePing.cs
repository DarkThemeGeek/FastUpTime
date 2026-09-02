namespace FastUpTime.Models;

public class PingResult
{
    public long Id { get; set; }

    public long SiteId { get; set; }
    public Site Site { get; set; }

    public DateTime Timestamp { get; set; }

    public bool Success { get; set; }

    public long? ResponseTimeMs { get; set; }
}