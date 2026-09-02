using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastUpTime.Models;

public class Site
{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    public long Id { get; set; }
    [Column("Url")]
    public string Url { get; set; }
}