using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Tripesso.Areas.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [Area("Customer")]
    [ApiController]
    public class HomeDataController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeDataController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var airportsName = (await unitOfWork.AirportRepository.GetAsync()).Select(a=> a.Name);
            var hotelsName = (await unitOfWork.HotelRepository.GetAsync()).Select(h=>  h.Name);

            //var roundTrips = (await unitOfWork.FlightRepository.GetAllAsync(f=> f.))
            
            var reviews = (await unitOfWork.ReviewRepository.GetAsync()).OrderByDescending(r=> r.CreatedAt);
            var hotels = (await unitOfWork.HotelRepository.GetAsync()).OrderByDescending(h => h.Traffic);   

            return Ok(new
            {
                AirportsName = airportsName,
                HotelsName = hotelsName,
                Reviews = reviews,
                Hotels = hotels
            });
        }
    }
}
