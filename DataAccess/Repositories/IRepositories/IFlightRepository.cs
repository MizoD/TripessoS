using Models;

namespace DataAccess.Repositories.IRepositories
{
    public interface IFlightRepository : IRepository<Flight>
    {
        Task AddAsync(Flight flight);
        Task<IEnumerable<Flight>> GetAllFlightsAsync();
        Task<string?> GetByIdAsync(int id);
        Task<string?> GetFirstOrDefaultAsync(Func<object, bool> value, Func<object, object> include);
        Task<Flight?> GetFlightDetailsAsync(int id);
    }
}
