using Sanatik.Models;
using Microsoft.AspNetCore.Identity;

namespace Sanatik.Models;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalPhotos { get; set; }
    public int TotalLikes { get; set; }
    public List<Photo> RecentPhotos { get; set; }
    public List<IdentityUser> AllUsers { get; set; }
}
