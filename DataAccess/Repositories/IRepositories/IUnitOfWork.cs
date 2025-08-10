using Microsoft.AspNetCore.Identity;
using Models;

namespace DataAccess.Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IApplicationUserOTPRepository ApplicationUserOTPRepository { get; }
        IAirCraftRepository AirCraftRepository { get; }
        IAirportRepository AirportRepository { get; }
        IBookingRepository BookingRepository { get; }
        ICountryRepository CountryRepository { get; }
        ITripCartRepository TripCartRepository { get; }
        IEventRepository EventRepository { get; }
        IFlightRepository FlightRepository { get; }
        IHotelRepository HotelRepository { get; }
        IReviewRepository ReviewRepository { get; }
        ISeatRepository SeatRepository { get; }
        ITicketRepository TicketRepository { get; }
        ITripRepository TripRepository { get; }
        ITripWishlistRepository TripWishlistRepository { get; }
        UserManager<ApplicationUser> UserManager { get; }
        SignInManager<ApplicationUser> SignInManager { get; }
        Task<bool> CommitAsync();
    }
}
