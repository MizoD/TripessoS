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
        IFlightCartRepository FlightCartRepository { get; }
        IHotelCartRepository HotelCartRepository { get; }
        IFlightRepository FlightRepository { get; }
        IHotelRepository HotelRepository { get; }
        IReviewRepository ReviewRepository { get; }
        ISeatRepository SeatRepository { get; }
        ITicketRepository TicketRepository { get; }
        ITripRepository TripRepository { get; }
        ITripWishlistRepository TripWishlistRepository { get; }
        IHotelWishlistRepository HotelWishlistRepository { get; }
        IFlightWishlistRepository FlightWishlistRepository { get; }
        UserManager<ApplicationUser> UserManager { get; }
        SignInManager<ApplicationUser> SignInManager { get; }
        RoleManager<IdentityRole> RoleManager { get; }
        Task<bool> CommitAsync();
    }
}
