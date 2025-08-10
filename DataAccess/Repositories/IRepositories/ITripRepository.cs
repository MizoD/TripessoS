using Models;

namespace DataAccess.Repositories.IRepositories
{
    public interface ITripRepository : IRepository<Trip>
    {
        Task<IEnumerable<Trip>> GetAllAvailableTripsAsync();
        Task<Trip?> GetTripWithDetailsAsync(int tripId);
        Task<IEnumerable<Trip>> GetRelatedTripsAsync(Trip trip);
    }
}
