using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Tripesso.Areas.Customer
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
        public async Task<IActionResult> Index()
        {
            var airportsName = (await unitOfWork.AirportRepository.GetAllAsync()).Select(a=> a.Name);
            var hotelsName = (await unitOfWork.HotelRepository.GetAllAsync()).Select(h=>  h.Name);

            //var roundTrips = (await unitOfWork.FlightRepository.GetAllAsync(f=> f.))
            
            var reviews = (await unitOfWork.ReviewRepository.GetAllAsync()).OrderByDescending(r=> r.CreatedAt);
            var hotels = (await unitOfWork.HotelRepository.GetAllAsync()).OrderByDescending(h => h.Traffic);   

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
