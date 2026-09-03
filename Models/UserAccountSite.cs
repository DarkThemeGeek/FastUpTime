using System.ComponentModel.DataAnnotations.Schema;

namespace FastUpTime.Models;

public class UserAccountSite
{
    [ForeignKey("UserAccountID")]
    public long UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; }
    [ForeignKey("SiteId")]
    public long SiteId { get; set; }
    public Site Site { get; set; }

    public int PingIntervalSeconds { get; set; }
    public bool Enabled { get; set; }
}