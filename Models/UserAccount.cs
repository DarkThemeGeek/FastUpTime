using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastUpTime.Models;

[Table("user_account")]
public class UserAccount
{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("string")]
    public required int Id { set; get; }
    [Column("user_name")]
    public required string UserName { set; get; }
    [Column("password")]
    public required string Password { set; get; }
}