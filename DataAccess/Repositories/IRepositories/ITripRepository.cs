using Models;

namespace DataAccess.Repositories.IRepositories
{
    public interface ITripRepository : IRepository<Trip>
    {
        Task<IEnumerable<Trip>> GetAllAvailableTripsAsync();
        Task<Trip?> GetTripWithDetailsAsync(int tripId, int v);
        Task<IEnumerable<Trip>> GetRelatedTripsAsync(Trip trip);
        Task<(object trips, double totalCount)> GetPagedTripsAsync(int page, int pageSize);
       Task<Trip?>GetTripByIdAsync(int id);
       Task<IEnumerable<Trip>> GetTripByDepartureAirportAsync(string departureAirport, int excludeTripId);
        Task<(object flight, object relatedFlights)> GetFlightDetailsWithRelatedAsync(int id);
        void Save();
        Task GetFirstOrDefaultAsync(Func<object, bool> value);
        Task SearchAvailableFlightsAsync(string searchTerm, DateTime flyingFrom, DateTime flyingTo, int passengers);
    }
}
