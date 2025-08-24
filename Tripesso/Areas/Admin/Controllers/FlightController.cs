using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Tripesso.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public FlightsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index(int page = 1)
        {
            var flights = await unitOfWork.FlightRepository.GetAsync();

            var totalFlights = flights.Count();
            int pageSize = 6;

            flights = flights.OrderBy(f => f.DepartureTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                Data = flights,
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalFlights,
                    TotalPages = (int)Math.Ceiling(totalFlights / (double)pageSize)
                }
            });
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var flight = await unitOfWork.FlightRepository.GetOneAsync(f=> f.Id == id);

            if (flight == null) return NotFound();
            return Ok(flight);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]Flight flight)
        {
            var Created = await unitOfWork.FlightRepository.CreateAsync(flight);
            if (!Created) return BadRequest("Failed to create flight");

            return CreatedAtAction(nameof(Details), flight.Id);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit([FromBody]Flight flight)
        {
            var flightFromDb = await unitOfWork.FlightRepository.GetOneAsync(f=> f.Id == flight.Id);
            if (flightFromDb == null) return NotFound("There is no such a flight!");
            flightFromDb.Title = flight.Title;
            flightFromDb.Status = flight.Status;
            flightFromDb.BasePrice = flight.BasePrice;
            flightFromDb.DepartureTime = flight.DepartureTime;
            flightFromDb.ArrivalTime = flight.ArrivalTime;
            flightFromDb.AirCraftId = flight.AirCraftId;
            flightFromDb.DepartureAirportId = flight.DepartureAirportId;
            flightFromDb.ArrivalAirportId = flight.ArrivalAirportId;
            flightFromDb.TripId = flight.TripId;
            
            await unitOfWork.CommitAsync();

            var updated = await unitOfWork.FlightRepository.UpdateAsync(flight);
            if (!updated) return BadRequest("Can't update this flight!");

            return Ok(flight);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var flight = await unitOfWork.FlightRepository.GetOneAsync(f=> f.Id == id);
            
            if (flight == null) return NotFound("There is no such a flight!");
            
            var deleted = await unitOfWork.FlightRepository.DeleteAsync(flight);
            
            if (!deleted) return BadRequest("Can't delete this flight!");

            return Ok(new { message = "🗑️ Flight deleted successfully!" });
        }

    }
}