using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Tripesso.Areas.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    [ApiController]
    public class HomeDataController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeDataController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var bookingsCount = (await unitOfWork.BookingRepository.GetAsync()).Count();
            var tripsCount = (await unitOfWork.TripRepository.GetAsync()).Count();
            var flightsCount = (await unitOfWork.FlightRepository.GetAsync()).Count();
            var hotelsCount = (await unitOfWork.HotelRepository.GetAsync()).Count();
            var totalBookingsMade = (await unitOfWork.BookingRepository.GetAsync()).Sum(b => (decimal?)b.TotalAmount ?? 0);

            var result = new
            {
                BookingsCount = bookingsCount,
                TripsCount = tripsCount,
                FlightsCount = flightsCount,
                HotelsCount = hotelsCount,
                TotalBookingsMade = totalBookingsMade
            };

            return Ok(result);
        }
    }
}
