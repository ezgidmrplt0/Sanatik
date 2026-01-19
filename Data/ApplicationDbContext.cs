using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Sanatik.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Sanatik.Models.Photo> Photos { get; set; }
    public DbSet<Sanatik.Models.Comment> Comments { get; set; }
    public DbSet<Sanatik.Models.Like> Likes { get; set; }
    public DbSet<Sanatik.Models.UserProfile> UserProfiles { get; set; }
    public DbSet<Sanatik.Models.Follow> Follows { get; set; }
    public DbSet<Sanatik.Models.Favorite> Favorites { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Prevent cascade delete cycle for Follows
        builder.Entity<Sanatik.Models.Follow>()
            .HasOne(f => f.Follower)
            .WithMany()
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Sanatik.Models.Follow>()
            .HasOne(f => f.Followee)
            .WithMany()
            .HasForeignKey(f => f.FolloweeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevent cascade delete cycle for Favorites
        builder.Entity<Sanatik.Models.Favorite>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Sanatik.Models.Favorite>()
            .HasOne(f => f.Photo)
            .WithMany(p => p.Favorites)
            .HasForeignKey(f => f.PhotoId)
            .OnDelete(DeleteBehavior.Cascade); // Deleting photo deletes favorite, but deleting user won't cascade via User->Favorite

        builder.Entity<Sanatik.Models.Comment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Sanatik.Models.Like>()
            .HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure UserProfile relationship
        builder.Entity<Sanatik.Models.UserProfile>()
            .HasKey(up => up.UserId);

         builder.Entity<Sanatik.Models.UserProfile>()
            .HasOne(up => up.User)
            .WithOne()
            .HasForeignKey<Sanatik.Models.UserProfile>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Follow relationship
        builder.Entity<Sanatik.Models.Follow>()
            .HasOne(f => f.Follower)
            .WithMany()
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.NoAction); // Prevent cycles

        builder.Entity<Sanatik.Models.Follow>()
            .HasOne(f => f.Followee)
            .WithMany()
            .HasForeignKey(f => f.FolloweeId)
            .OnDelete(DeleteBehavior.NoAction); // Prevent cycles
    }
}
