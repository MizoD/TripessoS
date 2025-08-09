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
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<ApplicationUser> Users { get; set; } 
        public DbSet<Booking> Bookings { get; set; } 


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

            // Hotel -> Rooms (One-to-Many)
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Hotel -> Reviews (One-to-Many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Reviews)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Hotel -> Cart (One-to-Many)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Hotel)
                .WithMany(h => h.Carts)
                .HasForeignKey(c => c.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Hotel -> Wishlist (One-to-Many)
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Hotel)
                .WithMany(h => h.Wishlists)
                .HasForeignKey(w => w.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Room -> Reviews (One-to-Many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Room)
                .WithMany(room => room.Reviews)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Room -> Cart (One-to-Many)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Room)
                .WithMany(room => room.Carts)
                .HasForeignKey(c => c.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Room -> Wishlist (One-to-Many)
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Room)
                .WithMany(room => room.Wishlists)
                .HasForeignKey(w => w.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            // Room -> Wishlist (One-to-Many)
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Room)
                .WithMany(room => room.Wishlists)
                .HasForeignKey(w => w.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Room)
                .WithMany()
                .HasForeignKey(ci => ci.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // WishlistItem
            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.Wishlist)
                .WithMany(w => w.Items)
                .HasForeignKey(wi => wi.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.Hotel)
                .WithMany()
                .HasForeignKey(wi => wi.HotelId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
