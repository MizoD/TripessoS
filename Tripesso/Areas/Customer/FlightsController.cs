using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Request.HotelRequest;
using Models.DTOs.Response.HotelResponse;
using System.Security.Claims;

namespace Tripesso.Areas.Customer
{
    [Route("api/[area]/[controller]")]
    [Area("Customer")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;


        public FlightsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetFlights([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 6,
            [FromQuery] string sortBy = "price",
            [FromQuery] string sortOrder = "asc")
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 6;

            var flights = await unitOfWork.FlightRepository.GetAllAsync(includes: f=> f.Include(f=> f.Aircraft).Include(f=> f.ArrivalAirport)
                            .Include(f=> f.DepartureAirport).Include(f=> f.Trip).Include(f=> f.Bookings).Include(f=> f.Seats));

            // Sorting
            flights = (sortBy.ToLower(), sortOrder.ToLower()) switch
            {
                ("price", "desc") => flights.OrderByDescending(f=> f.BasePrice),
                _ => flights.OrderBy(x => x.BasePrice)
            };

            // pagination 
            var totalCount = flights.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var data = flights.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            var result = new PaginatedFlightResponse
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = data
            };

            return Ok(result);
        }
        [HttpGet("FlightDetails/{id}")]
        public async Task<IActionResult> GetFlightDetails(int id)
        {
            var flight = await unitOfWork.FlightRepository.GetOneAsync(f => f.Id == id,
                includes: f => f.Include(f => f.Aircraft).Include(f => f.ArrivalAirport).Include(f => f.DepartureAirport)
                            .Include(f => f.Trip).Include(f => f.Bookings).Include(f => f.Seats));

            if (flight == null)
                return NotFound(new { message = "Flight not found" });

            return Ok(flight);
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(int flightId, DateTime travelDate, int numberOfPassengers)
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user == null)
                return Unauthorized();

            if (travelDate < DateTime.Today)
            {
                return BadRequest("Invalid Travel Date.");
            }


            var flight = await unitOfWork.FlightRepository.GetOneAsync(t => t.Id == flightId);

            if (flight == null)
                return NotFound("Flight not found.");

            if (flight.AvailableSeats < numberOfPassengers)
                return BadRequest("Not enough available Seats.");
            
            // Checks if the Flight is already in the user's cart
            var existingCartItem = await unitOfWork.FlightCartRepository.GetOneAsync(
                c => c.UserId == user.Id && c.FlightId == flightId);

            if (existingCartItem != null)
            {
                existingCartItem.NumberOfPassengers++;
            }
            else
            {
                var cartItem = new FlightCart
                {
                    FlightId = flight.Id,
                    UserId = user.Id,
                    NumberOfPassengers = numberOfPassengers,
                    AddedAt = DateTime.UtcNow
                };

                var added = await unitOfWork.FlightCartRepository.CreateAsync(cartItem);
                if (!added)
                    return StatusCode(500, "Something went wrong while adding to cart.");
            }

            await unitOfWork.CommitAsync();
            return Ok("Flight added to cart successfully.");
        }

        [HttpPost("AddToWishlist")]
        public async Task<IActionResult> AddToWishlist(int flightId)
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user == null)
                return Unauthorized();

            var flight = await unitOfWork.FlightRepository.GetOneAsync(t => t.Id == flightId);

            if (flight == null)
                return NotFound("Flight not found.");

            // Check if it's already in wishlist
            var existingWishlistItem = await unitOfWork.FlightWishlistRepository.GetOneAsync(
                w => w.UserId == user.Id && w.FlightId == flightId
            );

            if (existingWishlistItem != null)
                return BadRequest("This Flight is already in your wishlist.");

            var wishlistItem = new FlightWishlist
            {
                FlightId = flightId,
                UserId = user.Id,
                AddedAt = DateTime.UtcNow
            };

            var added = await unitOfWork.FlightWishlistRepository.CreateAsync(wishlistItem);
            if (!added)
                return StatusCode(500, "Something went wrong while adding to wishlist.");

            return Ok("Flight added to wishlist successfully.");
        }

        [HttpPost("SearchFlights")]
        public async Task<IActionResult> SearchFlights(
    [FromBody] FlightSearchRequest request,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 6,
    [FromQuery] string sortBy = "price",
    [FromQuery] string sortOrder = "asc")
        {
            var flights = await unitOfWork.FlightRepository.GetAllAsync(f =>
                    f.AvailableSeats >= request.NumberOfPassengers &&
                    f.DepartureTime > DateTime.UtcNow &&
                    f.Status == FlightStatus.Scheduled &&
                    f.ArrivalAirport.Name.ToLower().Contains(request.Country ?? "".ToLower()),
                includes: f => f
                    .Include(f => f.Aircraft).Include(f => f.ArrivalAirport).Include(f => f.DepartureAirport)
                            .Include(f => f.Trip).Include(f => f.Bookings).Include(f => f.Seats));


            if(request.TravelDate.HasValue)
            {
                var dayBefore = request.TravelDate.Value.AddDays(-14);
                var dayAfter = request.TravelDate.Value.AddDays(14);
                flights = flights.Where(f=> f.DepartureTime >= request.TravelDate && f.DepartureTime <= dayAfter && f.DepartureTime >= dayBefore);
            }
            // sort
            flights = sortBy.ToLower() switch
            {
                "price" => sortOrder.ToLower() == "desc"
                    ? flights.OrderByDescending(r => r.BasePrice)
                    : flights.OrderBy(r => r.BasePrice),
                _ => flights.OrderBy(r => r.BasePrice)
            };

            // Pagination
            var totalCount = flights.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var paginatedData = flights
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new FlightSerachResponse
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = paginatedData
            };

            return Ok(result);
        }

    }
}
