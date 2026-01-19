using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Sanatik.Models;

public class UserProfile
{
    [Key]
    public string UserId { get; set; }
    
    // Navigation property to IdentityUser
    public IdentityUser User { get; set; }
    
    [Display(Name = "Display Name")]
    [MaxLength(50)]
    public string? DisplayName { get; set; }
    
    [MaxLength(150)]
    public string? Bio { get; set; }
    
    public string? ProfileImageUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    
    [Url]
    public string? WebsiteUrl { get; set; }
    
    public string? Location { get; set; }
    
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
}
