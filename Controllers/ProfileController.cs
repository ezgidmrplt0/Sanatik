using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sanatik.Data;
using Sanatik.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Sanatik.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Profile/id
        public async Task<IActionResult> Index(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            IdentityUser targetUser = null;

            if (string.IsNullOrEmpty(id))
            {
                targetUser = currentUser;
            }
            else
            {
                targetUser = await _userManager.FindByIdAsync(id) ?? await _userManager.FindByNameAsync(id);
            }

            if (targetUser == null) return NotFound();

            var photos = await _context.Photos
                .Where(p => p.UserId == targetUser.Id)
                .OrderByDescending(p => p.UploadDate)
                .ToListAsync();

            var followerIds = await _context.Follows
                .Where(f => f.FolloweeId == targetUser.Id)
                .Select(f => f.FollowerId)
                .ToListAsync();

            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == targetUser.Id)
                .Select(f => f.FolloweeId)
                .ToListAsync();

            // Check who the CURRENT user follows
            var myFollowingIds = new List<string>();
            if (currentUser != null)
            {
                myFollowingIds = await _context.Follows
                    .Where(f => f.FollowerId == currentUser.Id)
                    .Select(f => f.FolloweeId)
                    .ToListAsync();
            }

            var followersList = await _context.Users
                .Where(u => followerIds.Contains(u.Id))
                .ToListAsync();

            var followingList = await _context.Users
                .Where(u => followingIds.Contains(u.Id))
                .ToListAsync();

            var model = new UserProfileViewModel
            {
                UserId = targetUser.Id,
                UserName = targetUser.UserName,
                Email = targetUser.Email,
                PostCount = photos.Count,
                FollowersCount = followerIds.Count,
                FollowingCount = followingIds.Count,
                Photos = photos,
                Followers = followersList.Select(u => new UserFollowViewModel 
                { 
                    UserId = u.Id, 
                    UserName = u.UserName, 
                    IsFollowing = myFollowingIds.Contains(u.Id) 
                }).ToList(),
                Following = followingList.Select(u => new UserFollowViewModel 
                { 
                    UserId = u.Id, 
                    UserName = u.UserName, 
                    IsFollowing = myFollowingIds.Contains(u.Id) 
                }).ToList(),
                IsCurrentUser = currentUser != null && currentUser.Id == targetUser.Id,
                IsFollowing = currentUser != null && myFollowingIds.Contains(targetUser.Id)
            };

            return View(model);
        }
    }
}
