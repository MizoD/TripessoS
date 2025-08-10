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
        public DbSet<TripCart> Carts { get; set; }
        public DbSet<TripWishlist> Wishlists { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<AirCraft> AirCrafts { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Booking> bookings { get; set; }
        public DbSet<Event> Events { get; set; }   
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Hotel> Hotel { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Ticket { get; set; }


    }
}
