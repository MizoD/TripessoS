using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Claims;

namespace Tripesso.Areas.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [Authorize]
    [Area("Customer")]
    [ApiController]
    public class CheckoutsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public CheckoutsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // POST: api/checkout/pay
        [HttpPost("pay")]
        public async Task<IActionResult> Pay()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Get cart items
            var flightItems = await unitOfWork.FlightCartRepository
                .GetAsync(fc => fc.UserId == userId, q => q.Include(x => x.Flight));
            var tripItems = await unitOfWork.TripCartRepository
                .GetAsync(tc => tc.UserId == userId, q => q.Include(x => x.Trip));
            var hotelItems = await unitOfWork.HotelCartRepository
                .GetAsync(hc => hc.UserId == userId, q => q.Include(x => x.Hotel));

            if (!flightItems.Any() && !tripItems.Any() && !hotelItems.Any())
                return BadRequest("Cart is empty.");

            // Stripe line items
            var lineItems = new List<SessionLineItemOptions>();
            decimal totalAmount = 0;

            foreach (var fc in flightItems)
            {
                var price = fc.NumberOfPassengers * fc.Flight.BasePrice;
                totalAmount += price;

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(fc.Flight.BasePrice * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Flight: {fc.Flight.Title}"
                        }
                    },
                    Quantity = fc.NumberOfPassengers
                });
            }

            foreach (var tc in tripItems)
            {
                var price = tc.NumberOfPassengers * tc.Trip.Price;
                totalAmount += price;

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(tc.Trip.Price * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Trip: {tc.Trip.Title}"
                        }
                    },
                    Quantity = tc.NumberOfPassengers
                });
            }

            foreach (var hc in hotelItems)
            {
                var price = hc.NumberOfPassengers * hc.Hotel.PricePerNight;
                totalAmount += price;

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(hc.Hotel.PricePerNight * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Hotel: {hc.Hotel.Name}"
                        }
                    },
                    Quantity = hc.NumberOfPassengers
                });
            }

            // Create Booking (unpaid)
            var booking = new Booking
            {
                UserId = userId,
                BookingDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                PaymentMethod = PaymentMethod.Visa,
                Tickets = flightItems.Sum(x => x.NumberOfPassengers)
                                  + tripItems.Sum(x => x.NumberOfPassengers)
                                  + hotelItems.Sum(x => x.NumberOfPassengers),
                Status = BookingStatus.Pending,
            };

            await unitOfWork.BookingRepository.CreateAsync(booking);
            await unitOfWork.CommitAsync();

            // Create Stripe checkout session
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/api/checkout/success?bookingId={booking.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/api/checkout/cancel",
            };

            var service = new SessionService();
            var session = service.Create(options);

            booking.SessionId = session.Id;
            booking.PaymentId = null;
            await unitOfWork.BookingRepository.UpdateAsync(booking);
            await unitOfWork.CommitAsync();

            return Ok(new { url = session.Url });
        }

        // Payment success
        [HttpGet("success")]
        public async Task<IActionResult> Success(int bookingId)
        {
            var booking = await unitOfWork.BookingRepository.GetOneAsync(b => b.Id == bookingId);
            if (booking == null) return NotFound();

            booking.Status = BookingStatus.Paid;
            await unitOfWork.BookingRepository.UpdateAsync(booking);
            await unitOfWork.CommitAsync();

            // Empty Cart
            var userId = booking.UserId;
            var flights = await unitOfWork.FlightCartRepository.GetAsync(c => c.UserId == userId);
            var trips = await unitOfWork.TripCartRepository.GetAsync(c => c.UserId == userId);
            var hotels = await unitOfWork.HotelCartRepository.GetAsync(c => c.UserId == userId);

            foreach (var f in flights) await unitOfWork.FlightCartRepository.DeleteAsync(f);
            foreach (var t in trips) await unitOfWork.TripCartRepository.DeleteAsync(t);
            foreach (var h in hotels) await unitOfWork.HotelCartRepository.DeleteAsync(h);

            await unitOfWork.CommitAsync();

            return Ok(new { message = "Payment successful. Booking confirmed." });
        }

        // Payment cancel
        [HttpGet("cancel")]
        public IActionResult Cancel()
        {
            return Ok(new { message = "Payment cancelled." });
        }

    }
}
