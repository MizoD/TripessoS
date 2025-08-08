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
                        IEventRepository eventRepository, IFlightRepository flightRepository, IHotelRepository hotelRepository,
                        IReviewRepository reviewRepository, ISeatRepository seatRepository, ITicketRepository ticketRepository,
                        ITripRepository tripRepository, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager,
                        SignInManager<ApplicationUser> signInManager)
        {
            ApplicationUserOTPRepository = applicationUserOTPRepository;
            AirCraftRepository = airCraftRepository;
            AirportRepository = airportRepository;
            BookingRepository = bookingRepository;
            CountryRepository = countryRepository;
            EventRepository = eventRepository;
            FlightRepository = flightRepository;
            HotelRepository = hotelRepository;
            ReviewRepository = reviewRepository;
            SeatRepository = seatRepository;
            TicketRepository = ticketRepository;
            TripRepository = tripRepository;
            this.dbContext = dbContext;
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public IApplicationUserOTPRepository ApplicationUserOTPRepository { get; }
        public IAirCraftRepository AirCraftRepository { get; }
        public IAirportRepository AirportRepository { get; }
        public IBookingRepository BookingRepository { get; }
        public ICountryRepository CountryRepository { get; }
        public IEventRepository EventRepository { get; }
        public IFlightRepository FlightRepository { get; }
        public IHotelRepository HotelRepository { get; }
        public IReviewRepository ReviewRepository { get; }
        public ISeatRepository SeatRepository { get; }
        public ITicketRepository TicketRepository { get; }
        public ITripRepository TripRepository { get; }
        public UserManager<ApplicationUser> UserManager { get; }
        public SignInManager<ApplicationUser> SignInManager { get; }

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
