using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sanatik.Data;
using Sanatik.Models;

namespace Sanatik.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Feed()
    {
        // Featured: Admin Selected
        var featured = await _context.Photos
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Where(p => p.IsFeatured)
            .OrderByDescending(p => p.UploadDate)
            .Take(10)
            .ToListAsync();

        // Discovery: Random ordering for the rest
        // Note: Guid.NewGuid() in OrderBy works for SQL Server to generate NEWID()
        var allPhotos = await _context.Photos
            .Include(p => p.User)
            .Include(p => p.Likes)
            .OrderBy(r => Guid.NewGuid()) 
            .Take(20) // Limit to 20 for now to keep it fast
            .ToListAsync();

        var viewModel = new HomeViewModel
        {
            Featured = featured,
            Discovery = allPhotos
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
