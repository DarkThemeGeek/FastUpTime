using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastUpTime.Models;

public class SitePing
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public long SiteId { get; set; }
    public Site Site { get; set; }
    
    public DateTime Timestamp { get; set; }

    public bool Success { get; set; }

    public int? StatusCode { get; set; }

    public long? ResponseTimeMs { get; set; }

    public string? Error { get; set; }
}