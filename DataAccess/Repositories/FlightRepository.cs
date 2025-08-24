using DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Repositories
{
    public class FlightRepository : Repository<Flight>, IFlightRepository
    {
        private readonly ApplicationDbContext _context;

        public FlightRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task AddAsync(Flight flight)
        {
            throw new NotImplementedException();
        }

        // ✅ Get all flights with related data
        public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
        {
            return await _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.Aircraft)
                .ToListAsync();
        }

        public Task<string?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetFirstOrDefaultAsync(Func<object, bool> value, Func<object, object> include)
        {
            throw new NotImplementedException();
        }

        // ✅ Get a single flight with related data
        public async Task<Flight?> GetFlightDetailsAsync(int id)
        {
            return await _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.Aircraft)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
