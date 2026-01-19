using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sanatik.Data;
using Sanatik.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Sanatik.Controllers;

public class PhotosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public PhotosController(ApplicationDbContext context, IWebHostEnvironment environment, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _context = context;
        _environment = environment;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET: Photos/Create
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Photos/Create
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Photo photo, IFormFile imageFile)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        if (imageFile != null && imageFile.Length > 0)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(_environment.WebRootPath, "uploads");
            
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            
            using (var stream = new FileStream(Path.Combine(filePath, fileName), FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            
            photo.ImagePath = "/uploads/" + fileName;
            photo.UserId = user.Id;
            photo.UploadDate = DateTime.Now;

            // Simple validation workaround for User navigation property
            ModelState.Remove("User");
            ModelState.Remove("UserId");
            ModelState.Remove("ImagePath");

            if (ModelState.IsValid)
            {
                _context.Add(photo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }
        }
        return View(photo);
    }

    // GET: Photos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var photo = await _context.Photos
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (photo == null) return NotFound();

        return View(photo);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddComment(int photoId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return RedirectToAction(nameof(Details), new { id = photoId });

        var comment = new Comment
        {
            PhotoId = photoId,
            Content = content,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAt = DateTime.Now
        };
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = photoId });
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite(int photoId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existingFav = await _context.Favorites.FirstOrDefaultAsync(f => f.PhotoId == photoId && f.UserId == userId);
        
        if (existingFav == null)
        {
            var fav = new Favorite { PhotoId = photoId, UserId = userId };
            _context.Favorites.Add(fav);
        }
        else
        {
            _context.Favorites.Remove(existingFav);
        }
        await _context.SaveChangesAsync();
        
        var count = await _context.Favorites.CountAsync(f => f.PhotoId == photoId);
        return Json(new { success = true, isFavorited = (existingFav == null), count = count });
    }

    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> GetPhotoDetails(int id)
    {
        try
        {
            var photo = await _context.Photos
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Favorites)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (photo == null) return NotFound(new { message = "Photo not found" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Null coalesce just in case
            var userName = photo.User?.UserName ?? "Unknown";

            var viewModel = new PhotoDetailsViewModel
            {
                Id = photo.Id,
                Title = photo.Title ?? "Untitled",
                Description = photo.Description ?? "",
                ImagePath = photo.ImagePath,
                UploadDate = photo.UploadDate,
                UserName = userName,
                LikeCount = photo.Likes?.Count ?? 0,
                FavoriteCount = photo.Favorites?.Count ?? 0,
                IsLiked = userId != null && (photo.Likes?.Any(l => l.UserId == userId) ?? false),
                IsFavorited = userId != null && (photo.Favorites?.Any(f => f.UserId == userId) ?? false),
                IsFollowing = userId != null && await _context.Follows.AnyAsync(f => f.FollowerId == userId && f.FolloweeId == photo.UserId),
                PhotoUserId = photo.UserId,
                Comments = photo.Comments?
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentViewModel
                    {
                        UserName = c.User?.UserName ?? "Unknown",
                        UserId = c.UserId, // Map User Id
                        Content = c.Content ?? "",
                        TimeAgo = GetTimeAgo(c.CreatedAt)
                    })?.ToList() ?? new List<CommentViewModel>()
            };

            return Json(viewModel);
        }
        catch (Exception ex)
        {
            // For debugging purposes, return the error
            return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
        }
    }
    
    // AJAX Follow Toggle
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ToggleFollow(string followeeId)
    {
        var followerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (followerId == followeeId) return BadRequest("Cannot follow yourself");

        var existingFollow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

        bool isFollowing = false;

        if (existingFollow == null)
        {
            var follow = new Follow { FollowerId = followerId, FolloweeId = followeeId, CreatedDate = DateTime.Now };
            _context.Follows.Add(follow);
            isFollowing = true;
        }
        else
        {
            _context.Follows.Remove(existingFollow);
            isFollowing = false;
        }
        
        await _context.SaveChangesAsync();
        return Json(new { success = true, isFollowing = isFollowing });
    }
    
    // GET: Photos/MyGallery
    // Force Rebuild Trigger: 1
    [Authorize]
    public async Task<IActionResult> MyGallery()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Index", "Home");

        var myPhotos = await _context.Photos
            .Include(p => p.Likes)
            .Where(p => p.UserId == user.Id)
            .OrderByDescending(p => p.UploadDate)
            .ToListAsync();

        var savedPhotos = await _context.Favorites
            .Where(f => f.UserId == user.Id)
            .Include(f => f.Photo)
            .ThenInclude(p => p.User)
            .Select(f => f.Photo)
            .OrderByDescending(p => p.UploadDate)
            .ToListAsync();

        var model = new MyGalleryViewModel
        {
            UploadedPhotos = myPhotos,
            SavedPhotos = savedPhotos
        };

        return View(model);
    }
    
    // AJAX Like
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AjaxLike(int photoId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.PhotoId == photoId && l.UserId == userId);
        
        if (existingLike == null)
        {
            var like = new Like { PhotoId = photoId, UserId = userId };
            _context.Likes.Add(like);
        }
        else
        {
            _context.Likes.Remove(existingLike);
        }
        await _context.SaveChangesAsync();

        var count = await _context.Likes.CountAsync(l => l.PhotoId == photoId);
        return Json(new { success = true, isLiked = (existingLike == null), count = count });
    }

    // AJAX Comment
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AjaxComment(int photoId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return BadRequest("Content required");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var comment = new Comment
        {
            PhotoId = photoId,
            Content = content,
            UserId = userId,
            CreatedAt = DateTime.Now
        };
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Json(new { 
            success = true, 
            userId = userId, // Add User Id
            userName = User.Identity.Name?.Split('@')[0], 
            content = content,
            timeAgo = "Just now"
        });
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var span = DateTime.Now - dateTime;
        if (span.TotalHours < 1) return $"{span.Minutes}m ago";
        if (span.TotalHours < 24) return $"{span.Hours}h ago";
        return $"{span.Days}d ago";
    }
}
