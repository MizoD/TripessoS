using DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Repositories
{
    public class TripRepository : Repository<Trip>, ITripRepository
    {
        public TripRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Trip>> GetAllAvailableTripsAsync()
        {
            return await _context.Trips
                .Include(t => t.Country)
                .Include(t => t.Reviews)
                .Where(t => t.IsAvailable)
                .OrderBy(t => t.StartDate)
                .ToListAsync();
        }

        public async Task<Trip?> GetTripWithDetailsAsync(int tripId)
        {
            return await _context.Trips
                .Include(t => t.Country)
                .Include(t => t.Reviews)
                .FirstOrDefaultAsync(t => t.Id == tripId);
        }

        public async Task<IEnumerable<Trip>> GetRelatedTripsAsync(Trip trip)
        {
            return await _context.Trips
                .Where(t => t.Id != trip.Id &&
                            t.CountryId == trip.CountryId && // same airport/country
                            t.IsAvailable &&
                            Math.Abs(EF.Functions.DateDiffDay(t.StartDate, trip.StartDate)) <= 14)
                .OrderBy(t => t.StartDate)
                .Take(5)
                .ToListAsync();
        }

        //Flight details
        public async Task<(Trip? Flight, IEnumerable<Trip> RelatedFlights)> GetFlightDetailsWithRelatedAsync(int tripId)
        {
            var flight = await GetTripWithDetailsAsync(tripId);
            if (flight == null) return (null, Enumerable.Empty<Trip>());

            var relatedFlights = await GetRelatedTripsAsync(flight);
            return (flight, relatedFlights);
        }

        // Pagination
        public async Task<(IEnumerable<Trip> Trips, int TotalCount)> GetPagedTripsAsync(int page, int pageSize)
        {
            var query = _context.Trips
                .Include(t => t.Country)
                .Include(t => t.Reviews)
                .Where(t => t.IsAvailable);

            var totalCount = await query.CountAsync();

            var trips = await query
                .OrderBy(t => t.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (trips, totalCount);
        }
    }
}
