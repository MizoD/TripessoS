using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tripesso.Areas.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public BookingsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await unitOfWork.BookingRepository.GetAsync(includes: b => b.Include(b => b.User).Include(b => b.Trip)
                            .Include(b => b.Flight).Include(b => b.Hotel));

            return Ok(new
            {
                TotalCount = bookings.Count(),
                Data = bookings
            });
        }

        [HttpGet("GetDetails/{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var booking = await unitOfWork.BookingRepository.GetOneAsync(b=> b.Id == id,includes: b => b.Include(b => b.User).Include(b => b.Trip)
                            .Include(b => b.Flight).Include(b => b.Hotel));

            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            return Ok(booking);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Booking booking)
        {
            booking.BookingDate = DateTime.UtcNow;
            var Created = await unitOfWork.BookingRepository.CreateAsync(booking);
            if (!Created)
                return BadRequest(new { message = "Failed to create booking" });

            return Ok(new { message = "Booking created successfully", BookingId = booking.Id });
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit([FromBody] Booking booking)
        {
            var existingBooking = await unitOfWork.BookingRepository.GetOneAsync(b=> b.Id == booking.Id);
            if (existingBooking == null)
                return NotFound(new { message = "Booking not found" });

            existingBooking.TotalAmount = booking.TotalAmount;
            existingBooking.Tickets = booking.Tickets;
            existingBooking.PaymentMethod = booking.PaymentMethod;
            existingBooking.PaymentId = booking.PaymentId;
            existingBooking.SessionId = booking.SessionId;
            existingBooking.UserId = booking.UserId;
            existingBooking.TripId = booking.TripId;
            existingBooking.FlightId = booking.FlightId;
            existingBooking.HotelId = booking.HotelId;

            await unitOfWork.BookingRepository.UpdateAsync(existingBooking);

            return Ok(new { message = "Booking updated successfully"});
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await unitOfWork.BookingRepository.GetOneAsync(b=> b.Id == id);
            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            await unitOfWork.BookingRepository.DeleteAsync(booking);

            return Ok(new { message = "Booking deleted successfully" });
        }

        [HttpPost("Status/{id}")]
        public async Task<IActionResult> ToggleBookingStatus(int id)
        {
            var booking = await unitOfWork.BookingRepository.GetOneAsync(b => b.Id == id);
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
                message = $"Booking {(isActive ? "activated" : "Unactivated")} based on payment method",
                bookingId = booking.Id,
                isActive
            });
        }
    }
}
