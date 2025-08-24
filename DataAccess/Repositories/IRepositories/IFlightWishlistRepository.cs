using Models;

namespace DataAccess.Repositories.IRepositories
{
    public interface IFlightWishlistRepository : IRepository<FlightWishlist>
    {
        Task<IEnumerable<FlightWishlist>> GetAllWishlistItemsAsync();
        Task GetFirstOrDefaultAsync(Func<object, bool> value);
        Task<FlightWishlist?> GetWishlistItemByIdAsync(int id);
    }
}
