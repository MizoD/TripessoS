using DataAccess.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Models;

namespace DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;

        public UnitOfWork(IApplicationUserOTPRepository applicationUserOTPRepository,IAirCraftRepository airCraftRepository, 
                        IAirportRepository airportRepository, IBookingRepository bookingRepository, ICountryRepository countryRepository,
                        IFlightRepository flightRepository, IHotelRepository hotelRepository, IFlightWishlistRepository flightWishlistRepository,
                        IReviewRepository reviewRepository, ISeatRepository seatRepository, ITicketRepository ticketRepository,
                        ITripRepository tripRepository, ITripCartRepository tripCartRepository,ApplicationDbContext dbContext, 
                        IFlightCartRepository flightCartRepository, IHotelCartRepository hotelCartRepository, ITripWishlistRepository tripWishlistRepository,
                        IHotelWishlistRepository hotelWishlistRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
                        , RoleManager<IdentityRole> roleManager)
        {
            ApplicationUserOTPRepository = applicationUserOTPRepository;
            AirCraftRepository = airCraftRepository;
            AirportRepository = airportRepository;
            BookingRepository = bookingRepository;
            CountryRepository = countryRepository;
            FlightRepository = flightRepository;
            HotelRepository = hotelRepository;
            FlightWishlistRepository = flightWishlistRepository;
            ReviewRepository = reviewRepository;
            SeatRepository = seatRepository;
            TicketRepository = ticketRepository;
            TripRepository = tripRepository;
            TripCartRepository = tripCartRepository;
            this.dbContext = dbContext;
            FlightCartRepository = flightCartRepository;
            HotelCartRepository = hotelCartRepository;
            TripWishlistRepository = tripWishlistRepository;
            HotelWishlistRepository = hotelWishlistRepository;
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public IApplicationUserOTPRepository ApplicationUserOTPRepository { get; }
        public IAirCraftRepository AirCraftRepository { get; }
        public IAirportRepository AirportRepository { get; }
        public IBookingRepository BookingRepository { get; }
        public ICountryRepository CountryRepository { get; }
        public IFlightRepository FlightRepository { get; }
        public IHotelRepository HotelRepository { get; }
        public IFlightWishlistRepository FlightWishlistRepository { get; }
        public IReviewRepository ReviewRepository { get; }
        public ISeatRepository SeatRepository { get; }
        public ITicketRepository TicketRepository { get; }
        public ITripRepository TripRepository { get; }
        public ITripCartRepository TripCartRepository { get; }
        public IFlightCartRepository FlightCartRepository { get; }
        public IHotelCartRepository HotelCartRepository { get; }
        public ITripWishlistRepository TripWishlistRepository { get; }
        public IHotelWishlistRepository HotelWishlistRepository { get; }
        public UserManager<ApplicationUser> UserManager { get; }
        public SignInManager<ApplicationUser> SignInManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        public async Task<bool> CommitAsync()
        {
            try
            {
                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex}");
                return false;
            }
        }
        public void Dispose()
        {
            dbContext.Dispose();
        }

    }
}
