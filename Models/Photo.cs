using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Sanatik.Models;

public class Photo
{
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string ImagePath { get; set; }
    
    public DateTime UploadDate { get; set; } = DateTime.Now;

    public bool IsFeatured { get; set; } // Admin selection

    // Navigation Properties
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
    
    public List<Comment> Comments { get; set; } = new();
    public List<Like> Likes { get; set; } = new();
    public List<Favorite> Favorites { get; set; } = new();
}
