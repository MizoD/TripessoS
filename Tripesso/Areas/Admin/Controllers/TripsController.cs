using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs.Request.TripRequest;
using Models.DTOs.Response.TripResponse;

namespace Tripesso.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public TripsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            var trips = await unitOfWork.TripRepository.GetAsync(includes: t=> t.Include(t=> t.Country).Include(t=> t.Bookings)
                                .Include(t=> t.Flights).Include(t=> t.Hotels).Include(t=> t.Reviews));

            var totalTrips = trips.Count();
            int pageSize = 10;

            trips = trips
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = trips.Select(t => new TripResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description ?? "",
                TripType = t.TripType,
                CountryId = t.CountryId,
                CountryName = t.Country?.Name ?? "",
                ImageUrl = t.ImageUrl,
                DurationDays = (t.EndDate - t.StartDate).Days,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                TotalSeats = t.TotalSeats,
                AvailableSeats = t.AvailableSeats,
                Price = t.Price,
                IsAvailable = t.IsAvailable,
                SecondaryImages = t.SecondryImages?.ToList(),
                VideoUrl = t.VideoUrl,
                AverageRating = t.Reviews.Any() ? t.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = t.Reviews.Count
            });

            return Ok(new
            {
                Data = response,
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalTrips,
                    TotalPages = (int)Math.Ceiling(totalTrips / (double)pageSize)
                }
            });
        }

        [HttpGet("GetOne/{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var trip = await unitOfWork.TripRepository.GetOneAsync(t=> t.Id == id,includes: t => t.Include(t => t.Country).Include(t => t.Bookings)
                                .Include(t => t.Flights).Include(t => t.Hotels).Include(t => t.Reviews));

            if (trip == null)
                return NotFound("Trip not found");

            var response = new TripResponse
            {
                Id = trip.Id,
                Title = trip.Title,
                Description = trip.Description ?? "",
                TripType = trip.TripType,
                CountryId = trip.CountryId,
                CountryName = trip.Country?.Name ?? "",
                ImageUrl = trip.ImageUrl,
                DurationDays = (trip.EndDate - trip.StartDate).Days,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                TotalSeats = trip.TotalSeats,
                AvailableSeats = trip.AvailableSeats,
                Price = trip.Price,
                IsAvailable = trip.IsAvailable,
                SecondaryImages = trip.SecondryImages?.ToList(),
                VideoUrl = trip.VideoUrl,
                AverageRating = trip.Reviews.Any() ? trip.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = trip.Reviews.Count
            };

            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateTripRequest request)
        {
            var trip = new Trip
            {
                Title = request.Title,
                Description = request.Description,
                TripType = request.TripType,
                CountryId = request.CountryId,
                ImageUrl = request.ImageUrl,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalSeats = request.TotalSeats,
                AvailableSeats = request.AvailableSeats,
                Price = request.Price,
                IsAvailable = request.IsAvailable,
                SecondryImages = request.SecondaryImages,
                VideoUrl = request.VideoUrl,
                Rate = 0
            };

            var created = await unitOfWork.TripRepository.CreateAsync(trip);
            if (!created)
                return BadRequest("Failed to create trip");

            return Ok(new { message = "✅ Trip created successfully!", tripId = trip.Id });
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateTripRequest request)
        {
            var trip = await unitOfWork.TripRepository.GetOneAsync(t => t.Id == request.Id, includes: t => t.Include(t => t.Country).Include(t => t.Bookings)
                                .Include(t => t.Flights).Include(t => t.Hotels).Include(t => t.Reviews));
            if (trip == null)
                return NotFound("Trip not found");

            trip.Title = request.Title;
            trip.Description = request.Description;
            trip.TripType = request.TripType;
            trip.CountryId = request.CountryId;
            trip.ImageUrl = request.ImageUrl;
            trip.StartDate = request.StartDate;
            trip.EndDate = request.EndDate;
            trip.TotalSeats = request.TotalSeats;
            trip.AvailableSeats = request.AvailableSeats;
            trip.Price = request.Price;
            trip.IsAvailable = request.IsAvailable;
            trip.SecondryImages = request.SecondaryImages;
            trip.VideoUrl = request.VideoUrl;

            var updated = await unitOfWork.TripRepository.UpdateAsync(trip);
            if (!updated)
                return BadRequest("Failed to update trip");

            return Ok(new { message = "✏️ Trip updated successfully!" });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var trip = await unitOfWork.TripRepository.GetOneAsync(t => t.Id == id);
            if (trip == null)
                return NotFound("Trip not found");

            var deleted = await unitOfWork.TripRepository.DeleteAsync(trip);  
            if (!deleted)
                return BadRequest("Failed to delete trip");

            return Ok(new { message = "🗑️ Trip deleted successfully!" });
        }
    }
}
