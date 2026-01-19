using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sanatik.Data;
using Sanatik.Models;

namespace Sanatik.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Admin/Dashboard
    public async Task<IActionResult> Index()
    {
        var viewModel = new AdminDashboardViewModel
        {
            TotalUsers = await _userManager.Users.CountAsync(),
            TotalPhotos = await _context.Photos.CountAsync(),
            TotalLikes = await _context.Likes.CountAsync(),
            RecentPhotos = await _context.Photos.Include(p => p.User).OrderByDescending(p => p.UploadDate).Take(10).ToListAsync(),
            AllUsers = await _userManager.Users.ToListAsync()
        };
        return View(viewModel);
    }

    // POST: Admin/DeletePhoto/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhoto(int id)
    {
        var photo = await _context.Photos.FindAsync(id);
        if (photo != null)
        {
            // Remove file from disk
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photo.ImagePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/DeleteUser/id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            // Optional: Delete user's photos first to clean up files
            var userPhotos = _context.Photos.Where(p => p.UserId == id);
            foreach (var photo in userPhotos)
            {
                 var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photo.ImagePath.TrimStart('/'));
                 if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }
            _context.Photos.RemoveRange(userPhotos);
            
            await _userManager.DeleteAsync(user);
        }
        return RedirectToAction(nameof(Index));
    }
}
