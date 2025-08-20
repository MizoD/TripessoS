using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tripesso.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Trip)
                .Include(b => b.Flight)
                .Include(b => b.Hotel)
                .ToListAsync();

            var totalBookingsMade = await _context.Bookings.SumAsync(b => (decimal?)b.TotalAmount ?? 0);

            return Ok(new
            {
                TotalCount = bookings.Count,
                TotalBookingsMade = totalBookingsMade,
                Data = bookings
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingDetails(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Trip)
                .Include(b => b.Flight)
                .Include(b => b.Hotel)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            return Ok(booking);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            booking.BookingDate = DateTime.UtcNow;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Booking created successfully", booking });
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> EditBooking(int id, [FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
                return NotFound(new { message = "Booking not found" });

            // Update fields from incoming data
            existingBooking.TotalAmount = booking.TotalAmount;
            existingBooking.NumberOfTickets = booking.NumberOfTickets;
            existingBooking.PaymentMethod = booking.PaymentMethod;
            existingBooking.PaymentId = booking.PaymentId;
            existingBooking.SessionId = booking.SessionId;
            existingBooking.UserId = booking.UserId;
            existingBooking.TripId = booking.TripId;
            existingBooking.FlightId = booking.FlightId;
            existingBooking.HotelId = booking.HotelId;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Booking updated successfully", existingBooking });
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Booking deleted successfully" });
        }

        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleBookingStatus(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            bool isActive;

            if (booking.PaymentMethod == PaymentMethod.CASHONSITE)
            {
                isActive = false;
            }
            else
            {
                isActive = true;
            }

            return Ok(new
            {
                message = $"Booking {(isActive ? "activated" : "deactivated")} based on payment method",
                bookingId = booking.Id,
                isActive
            });
        }
    }
}
