using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Trip -> Reviews (One-to-Many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Trip)
                .WithMany(t => t.Reviews)
                .HasForeignKey(r => r.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            // Trip -> Cart (One-to-Many)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Trip)
                .WithMany(t => t.Carts)
                .HasForeignKey(c => c.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            // Trip -> Wishlist (One-to-Many)
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Trip)
                .WithMany(t => t.Wishlists)
                .HasForeignKey(w => w.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            // Trip -> Country (Many-to-One)
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Country)
                .WithMany(c => c.Trips)
                .HasForeignKey(t => t.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review -> User
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
