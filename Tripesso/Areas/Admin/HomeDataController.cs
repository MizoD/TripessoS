using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Tripesso.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    [ApiController]
    public class HomeDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HomeDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("index")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var bookingsCountTask = _context.Bookings.CountAsync();
            var tripsCountTask = _context.Trips.CountAsync();
            var flightsCountTask = _context.Flights.CountAsync();
            var hotelsCountTask = _context.Hotel.CountAsync();
            var totalBookingsMadeTask = _context.Bookings.SumAsync(b => (decimal?)b.TotalAmount ?? 0);

            await Task.WhenAll(bookingsCountTask, tripsCountTask, flightsCountTask, hotelsCountTask, totalBookingsMadeTask);

            var result = new
            {
                BookingsCount = bookingsCountTask.Result,
                TripsCount = tripsCountTask.Result,
                FlightsCount = flightsCountTask.Result,
                HotelsCount = hotelsCountTask.Result,
                TotalBookingsMade = totalBookingsMadeTask.Result
            };

            return Ok(result);
        }
    }
}
