using Microsoft.AspNetCore.Identity;
using Sanatik.Models;

namespace Sanatik.Data;

public static class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
    {
        var userManager = service.GetService<UserManager<IdentityUser>>();
        var roleManager = service.GetService<RoleManager<IdentityRole>>();
        
        // 1. Rolleri Oluştur
        await roleManager.CreateAsync(new IdentityRole("Admin"));
        await roleManager.CreateAsync(new IdentityRole("User"));

        // 2. Admin Kullanıcısını Oluştur
        var adminEmail = "admin@sanatik.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            
            // Şifre: Admin123! (Güvenli bir şifre seçtik)
            var result = await userManager.CreateAsync(admin, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // 3. Sanatsal Fotograflari Seed Et
        var adminForPhotos = await userManager.FindByEmailAsync(adminEmail);
        if (adminForPhotos != null)
        {
            var context = service.GetService<ApplicationDbContext>();
            if (!context.Photos.Any(p => p.Title == "Neon David"))
            {
                context.Photos.AddRange(
                    new Photo
                    {
                        Title = "Neon David",
                        Description = "A digital reinterpretation of the classic sculpture with cyberpunk aesthetics.",
                        ImagePath = "/uploads/art_neon.png",
                        UserId = adminForPhotos.Id,
                        UploadDate = DateTime.Now.AddDays(-2),
                        IsFeatured = true
                    },
                    new Photo
                    {
                        Title = "Kinetic Void",
                        Description = "Abstract geometric shapes floating in a null space.",
                        ImagePath = "/uploads/art_kinetic.png",
                        UserId = adminForPhotos.Id,
                        UploadDate = DateTime.Now.AddDays(-1),
                        IsFeatured = true
                    }
                );
                await context.SaveChangesAsync();
            }
        }

        // 4. Pinterest Style Photos Seed
        if (adminForPhotos != null)
        {
            var context = service.GetService<ApplicationDbContext>();
            if (!context.Photos.Any(p => p.Title == "Liquid Gold"))
            {
                context.Photos.AddRange(
                    new Photo
                    {
                        Title = "Liquid Gold",
                        Description = "Mesmerizing flow of liquid oil and gold, representing the fluidity of modern art.",
                        ImagePath = "/uploads/pinterest_1.jpg",
                        UserId = adminForPhotos.Id,
                        UploadDate = DateTime.Now.AddDays(-5),
                        IsFeatured = true
                    },
                    new Photo
                    {
                        Title = "Neon Genesis",
                        Description = "A vibrant explosion of neon colors in a fluid, organic form.",
                        ImagePath = "/uploads/pinterest_2.jpg",
                        UserId = adminForPhotos.Id,
                        UploadDate = DateTime.Now.AddDays(-4),
                        IsFeatured = false
                    },
                    new Photo
                    {
                        Title = "Alpine Solitude",
                        Description = "The breathtaking silence of the mountains, captured in a moody atmosphere.",
                        ImagePath = "/uploads/pinterest_3.jpg",
                        UserId = adminForPhotos.Id,
                        UploadDate = DateTime.Now.AddDays(-3),
                        IsFeatured = true
                    },
                    new Photo
                    {
                        Title = "Night City",
                        Description = "Cyberpunk vibes from the heart of a futuristic metropolis.",
                        ImagePath = "/uploads/pinterest_4.jpg",
                        UserId = adminForPhotos.Id,
                        UploadDate = DateTime.Now.AddDays(-2),
                        IsFeatured = false
                    },
                    new Photo
                    {
                        Title = "Industrial Zen",
                        Description = "Minimalist concrete architecture meeting the warmth of sunlight.",
                        ImagePath = "/uploads/pinterest_5.jpg",
                        UserId = adminForPhotos.Id,
                        UploadDate = DateTime.Now.AddDays(-1),
                        IsFeatured = true
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
