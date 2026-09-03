using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastUpTime.Models;

[Table("user_account")]
public class UserAccount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { set; get; }
    public required string UserName { set; get; }
    public required string Password { set; get; }
    
    public Roles Role = Roles.User;
    public ICollection<UserAccountSite> Sites { get; set; } = new List<UserAccountSite>();
}