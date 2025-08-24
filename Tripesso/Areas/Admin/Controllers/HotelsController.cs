using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs.Request.HotelRequest;
using Models.DTOs.Response.HotelResponse;

namespace Tripesso.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public HotelsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            var hotels = await unitOfWork.HotelRepository.GetAsync(includes: h=> h.Include(h=> h.Trip).Include(h=> h.Country).Include(h=> h.Bookings));

            var totalHotels = hotels.Count();
            int pageSize = 6;

            hotels = hotels
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                Data = hotels,
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalHotels,
                    TotalPages = (int)Math.Ceiling(totalHotels / (double)pageSize)
                }
            });
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var hotel = await unitOfWork.HotelRepository.GetOneAsync(h=> h.Id == id,includes: h => h.Include(h => h.Trip).Include(h => h.Country).Include(h => h.Bookings));
                
            if (hotel == null)
                return NotFound("Hotel not found");

            return Ok(hotel);
        }

        
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateHotelRequest request)
        {
            var hotel = new Hotel
            {
                Name = request.Name,
                Description = request.Description,
                Phone = request.Phone,
                AvailableRooms = request.AvailableRooms,
                PricePerNight = request.PricePerNight,
                City = request.City,
                CountryId = request.CountryId,
                TripId = request.TripId
            };

            var created = await unitOfWork.HotelRepository.CreateAsync(hotel);
            if (!created)
                return BadRequest("Failed to create hotel");

            return CreatedAtAction(nameof(Details), hotel.Id );
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateHotelRequest request)
        {
            var hotel = await unitOfWork.HotelRepository.GetOneAsync(h=> h.Id == request.Id);
            if (hotel == null)
                return NotFound("Hotel not found");

            hotel.Name = request.Name;
            hotel.Description = request.Description;
            hotel.Phone = request.Phone;
            hotel.AvailableRooms = request.AvailableRooms;
            hotel.PricePerNight = request.PricePerNight;
            hotel.City = request.City;
            hotel.CountryId = request.CountryId;
            hotel.TripId = request.TripId;

            var updated = await unitOfWork.HotelRepository.UpdateAsync(hotel);
            if (!updated)
                return BadRequest("Failed to update hotel");

            return Ok(new { message = "✏️ Hotel updated successfully!" });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var hotel = await unitOfWork.HotelRepository.GetOneAsync(h => h.Id == id);
            if (hotel == null)
                return NotFound("Hotel not found");

            var deleted = await unitOfWork.HotelRepository.DeleteAsync(hotel);
            if (!deleted)
                return BadRequest("Failed to delete hotel");

            return Ok(new { message = "🗑️ Hotel deleted successfully!" });
        }
    }
}
