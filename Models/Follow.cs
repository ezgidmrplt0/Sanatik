using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Sanatik.Models;

public class Follow
{
    public int Id { get; set; }

    [Required]
    public string FollowerId { get; set; }
    [ForeignKey("FollowerId")]
    public IdentityUser Follower { get; set; }

    [Required]
    public string FolloweeId { get; set; }
    [ForeignKey("FolloweeId")]
    public IdentityUser Followee { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
