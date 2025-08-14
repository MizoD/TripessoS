using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Reflection.Emit;
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
        public DbSet<TripCart> TripCarts { get; set; }
        public DbSet<HotelCart> HotelCarts { get; set; }
        public DbSet<FlightCart> FlightCarts { get; set; }
        public DbSet<TripWishlist> TripWishlists { get; set; }
        public DbSet<HotelWishlist> HotelWishlists { get; set; }
        public DbSet<FlightWishlist> FlightWishlists { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<AirCraft> AirCrafts { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Booking> Bookings { get; set; }   
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Hotel> Hotel { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Ticket { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Hotel>()
                .HasOne(h => h.Trip)
                .WithMany(t => t.Hotels)
                .HasForeignKey(h => h.TripId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<Hotel>()
                .HasOne(h => h.Country)
                .WithMany(c => c.Hotels)
                .HasForeignKey(h => h.CountryId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Flight>()
                .HasOne(f => f.ArrivalAirport)
                .WithMany(a => a.ArrivalFlights)
                .HasForeignKey(f => f.ArrivalAirportId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Flight>()
                .HasOne(f => f.DepartureAirport)
                .WithMany(a => a.DepratureFlights)
                .HasForeignKey(f => f.DepartureAirportId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Flight>()
                .HasOne(f => f.Trip)
                .WithMany(t => t.Flights)
                .HasForeignKey(f => f.TripId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Booking>()
                .HasOne(b => b.Trip)
                .WithMany(t => t.Bookings)
                .HasForeignKey(b => b.TripId)
                .OnDelete(DeleteBehavior.NoAction); 

            builder.Entity<Booking>()
                .HasOne(b => b.Flight)
                .WithMany(f => f.Bookings)
                .HasForeignKey(b => b.FlightId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Booking>()
                .HasOne(b => b.Hotel)
                .WithMany(h => h.Bookings)
                .HasForeignKey(b => b.HotelId)
                .OnDelete(DeleteBehavior.NoAction); 

            builder.Entity<Review>()
                .HasOne(r => r.Trip)
                .WithMany(t => t.Reviews)
                .HasForeignKey(r => r.TripId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Trip>()
                .HasOne(t=> t.Country)
                .WithMany(c=> c.Trips)
                .HasForeignKey(t=> t.CountryId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
