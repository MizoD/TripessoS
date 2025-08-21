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
    [Route("admin/[area]/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HotelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET: admin/hotels?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var query = _context.Hotel
                .Include(h => h.Country)
                .Include(h => h.Trip)
                .Include(h => h.Bookings);

            var totalHotels = await query.CountAsync();

            var hotels = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = hotels.Select(h => new HotelResponse
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Phone = h.Phone,
                AvailableRooms = h.AvilableRooms,
                PricePerNight = h.PricePerNight,
                City = h.City,
                CountryId = h.CountryId,
                CountryName = h.Country?.Name ?? "",
                TripId = h.TripId,
                TripTitle = h.Trip?.Title ?? "",
                BookingCount = h.Bookings.Count
            });

            return Ok(new
            {
                Data = response,
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalHotels,
                    TotalPages = (int)Math.Ceiling(totalHotels / (double)pageSize)
                }
            });
        }

        // ✅ GET: admin/hotels/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var hotel = await _context.Hotel
                .Include(h => h.Country)
                .Include(h => h.Trip)
                .Include(h => h.Bookings)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null)
                return NotFound("Hotel not found");

            var response = new HotelResponse
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Phone = hotel.Phone,
                AvailableRooms = hotel.AvilableRooms,
                PricePerNight = hotel.PricePerNight,
                City = hotel.City,
                CountryId = hotel.CountryId,
                CountryName = hotel.Country?.Name ?? "",
                TripId = hotel.TripId,
                TripTitle = hotel.Trip?.Title ?? "",
                BookingCount = hotel.Bookings.Count
            };

            return Ok(response);
        }

        // ✅ POST: admin/hotels
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHotelRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hotel = new Hotel
            {
                Name = request.Name,
                Description = request.Description,
                Phone = request.Phone,
                AvilableRooms = request.AvailableRooms,
                PricePerNight = request.PricePerNight,
                City = request.City,
                CountryId = request.CountryId,
                TripId = request.TripId
            };

            _context.Hotel.Add(hotel);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Hotel created successfully!", hotelId = hotel.Id });
        }

        // ✅ PUT: admin/hotels/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHotelRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hotel = await _context.Hotel.FindAsync(id);
            if (hotel == null)
                return NotFound("Hotel not found");

            hotel.Name = request.Name;
            hotel.Description = request.Description;
            hotel.Phone = request.Phone;
            hotel.AvilableRooms = request.AvailableRooms;
            hotel.PricePerNight = request.PricePerNight;
            hotel.City = request.City;
            hotel.CountryId = request.CountryId;
            hotel.TripId = request.TripId;

            _context.Hotel.Update(hotel);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✏️ Hotel updated successfully!" });
        }

        // ✅ DELETE: admin/hotels/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var hotel = await _context.Hotel.FindAsync(id);
            if (hotel == null)
                return NotFound("Hotel not found");

            _context.Hotel.Remove(hotel);
            await _context.SaveChangesAsync();

            return Ok(new { message = "🗑️ Hotel deleted successfully!" });
        }
    }
}
