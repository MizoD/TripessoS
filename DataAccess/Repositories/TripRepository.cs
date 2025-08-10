using DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Repositories
{
    public class TripRepository : Repository<Trip>, ITripRepository
    {
        public TripRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Trip>> GetAllAvailableTripsAsync()
        {
            return await _context.Trips
                .Include(t => t.Country)
                .Include(t => t.Reviews)
                .Include(t=> t.Flights)
                .Include(t=> t.Hotels)
                .Where(t => t.IsAvailable)
                .OrderBy(t => t.StartDate)
                .ToListAsync();
        }
        public async Task<Trip?> GetTripWithDetailsAsync(int tripId)
        {
            return await _context.Trips
                .Include(t => t.Country)
                .Include(t => t.Reviews)
                .Include(t => t.Flights)
                .Include(t => t.Hotels)
                .FirstOrDefaultAsync(t => t.Id == tripId);
        }

        public async Task<IEnumerable<Trip>> GetRelatedTripsAsync(Trip trip)
        {
            return await _context.Trips
                .Where(t => t.Id != trip.Id &&
                            t.CountryId == trip.CountryId &&
                            t.IsAvailable &&
                            Math.Abs(EF.Functions.DateDiffDay(t.StartDate, trip.StartDate)) <= 14)
                .Include(t => t.Country)
                .Include(t => t.Reviews)
                .Include(t => t.Flights)
                .Include(t => t.Hotels)
                .OrderBy(t => t.StartDate)
                .Take(3)
                .ToListAsync();
        }
    }
}
