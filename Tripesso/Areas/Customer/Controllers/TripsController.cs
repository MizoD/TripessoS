using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Request.TripRequest;
using Models.DTOs.Response.TripResponse;
using System.Security.Claims;

namespace Tripesso.Areas.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class TripsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public TripsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("index")]
        public async Task<IActionResult> GetTrips([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 6)
        {
            // Get all available trips (consider adding filters here if needed)
            var allTrips = await unitOfWork.TripRepository.GetAllAvailableTripsAsync();

            // Get total count before pagination
            int totalCount = allTrips.Count();

            // Apply pagination
            var pagedTrips = allTrips
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TripResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description!,
                    Price = t.Price,
                    ImageUrl = t.ImageUrl,
                    CountryName = t.Country!.Name,
                    AverageRating = t.Reviews.Any() ? t.Reviews.Average(r => r.Rating) : 0,
                    ReviewCount = t.Reviews.Count,
                    IsAvailable = t.IsAvailable,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    DurationDays = t.DurationDays,
                    TripType = t.TripType,
                    TotalSeats = t.TotalSeats,
                    AvailableSeats = t.AvailableSeats,
                })
                .ToList();

            var response = new PaginatedTripResponse
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Trips = pagedTrips
            };

            return Ok(response);
        }

        [HttpGet("TripDetails/{id}")]
        public async Task<IActionResult> TripDetails(int id)
        {
            var trip = await unitOfWork.TripRepository.GetTripWithDetailsAsync(id);
            if (trip == null)
                return NotFound("Trip not found");

            var currentTrip = new TripResponse
            {
                Id = trip.Id,
                Title = trip.Title,
                Description = trip.Description!,
                Price = trip.Price,
                ImageUrl = trip.ImageUrl,
                CountryName = trip.Country!.Name,
                AverageRating = trip.Reviews.Any() ? trip.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = trip.Reviews.Count,
                IsAvailable = trip.IsAvailable,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                DurationDays = trip.DurationDays,
                TripType = trip.TripType,
                TotalSeats = trip.TotalSeats,
                AvailableSeats = trip.AvailableSeats,
            };
            var relatedTrips = await unitOfWork.TripRepository.GetRelatedTripsAsync(trip);

            var tripDetails = new 
            {
                Trip = currentTrip,
                RelatedTrips = relatedTrips
            };
            return Ok(tripDetails);
        }
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user == null)
                return Unauthorized();

            var trip = await unitOfWork.TripRepository.GetOneAsync(t => t.Id == request.TripId);

            if (trip == null)
                return NotFound("Trip not found.");

            if (trip.AvailableSeats < request.NumberOfPassengers)
                return BadRequest("Not enough available seats.");

            // Checks if the trip is already in the user's cart
            var existingCartItem = await unitOfWork.TripCartRepository.GetOneAsync(
                c => c.UserId == user.Id && c.TripId == request.TripId
            );

            if (existingCartItem != null) { 
                existingCartItem.NumberOfPassengers++;
            }    
            else {
                var cartItem = new TripCart
                {
                    TripId = trip.Id,
                    UserId = user.Id,
                    NumberOfPassengers = request.NumberOfPassengers,
                    AddedAt = DateTime.UtcNow
                };

                var added = await unitOfWork.TripCartRepository.CreateAsync(cartItem);
                if (!added)
                    return StatusCode(500, "Something went wrong while adding to cart.");
            }
               
            await unitOfWork.CommitAsync();
            return Ok();
        }

        [HttpPost("AddToWishlist")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistRequest request)
        {

            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user == null)
                return Unauthorized();

            var trip = await unitOfWork.TripRepository.GetOneAsync(t => t.Id == request.TripId);

            if (trip == null)
                return NotFound("Trip not found.");

            // Check if it's already in wishlist
            var existingWishlistItem = await unitOfWork.TripWishlistRepository.GetOneAsync(
                w => w.UserId == user.Id && w.TripId == request.TripId
            );

            if (existingWishlistItem != null)
                return BadRequest("This trip is already in your wishlist.");

            var wishlistItem = new TripWishlist
            {
                TripId = request.TripId,
                UserId = user.Id,
                AddedAt = DateTime.UtcNow
            };

            var added = await unitOfWork.TripWishlistRepository.CreateAsync(wishlistItem);
            if (!added)
                return StatusCode(500, "Something went wrong while adding to wishlist.");

            return Ok();
        }

        [HttpPost("Review")]
        public async Task<IActionResult> AddReview([FromBody] AddReviewRequest request)
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user == null)
                return Unauthorized();

            var trip = await unitOfWork.TripRepository.GetOneAsync(t => t.Id == request.TripId, includes: t=> t.Include(t=> t.Reviews));

            if (trip == null)
                return NotFound("Trip not found.");

            if (trip.AvailableSeats == 0)
                return BadRequest("Trip is not available for review.");

            // Check if user already reviewed this trip
            var alreadyReviewed = trip.Reviews.Any(r => r.UserId == user.Id);
            if (alreadyReviewed)
                return BadRequest("You have already reviewed this trip.");

            var review = new Review
            {
                TripId = trip.Id,
                UserId = user.Id,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            var success = await unitOfWork.ReviewRepository.CreateAsync(review);
            if (!success)
                return StatusCode(500, "Something went wrong while saving the review.");

            return Ok();
        }

        [HttpPost("SearchTrips")]
        public async Task<IActionResult> PostSearchTrips(
        [FromBody] TripSearchRequest request,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "price",
        [FromQuery] string sortOrder = "asc")
        {
            var trips = await unitOfWork.TripRepository.GetAsync(
                    t =>
                    t.IsAvailable &&
                    t.AvailableSeats >= request.NumberOfPassengers &&
                    t.Country != null &&
                    t.Country.Name.ToLower().Contains(request.CountryName ?? "".ToLower()),
                includes: q => q.Include(t => t.Country).Include(t => t.Reviews)
            );

            if (request.DesiredDate.HasValue)
            {
                var twoWeeksBefore = request.DesiredDate.Value.AddDays(-14);
                var twoWeeksAfter = request.DesiredDate.Value.AddDays(14);

                trips = trips.Where(t => t.StartDate >= twoWeeksBefore &&
                    t.StartDate <= twoWeeksAfter);
            }


            var response = trips.Select(t => new TripResponse
            {
                Id = t.Id,
                Title = t.Title,
                CountryName = t.Country!.Name,
                StartDate = t.StartDate,
                Price = t.Price,
                ImageUrl = t.ImageUrl,
                DurationDays = t.DurationDays,
                IsAvailable = t.IsAvailable,
                AverageRating = t.Reviews != null && t.Reviews.Any()
                    ? Math.Round(t.Reviews.Average(r => r.Rating), 1)
                    : 0
            });

            // Sort the response
            response = sortBy.ToLower() switch
            {
                "price" => sortOrder.ToLower() == "desc" ? response.OrderByDescending(r => r.Price) : response.OrderBy(r => r.Price),
                "rating" => sortOrder.ToLower() == "desc" ? response.OrderByDescending(r => r.AverageRating) : response.OrderBy(r => r.AverageRating),
                _ => response.OrderBy(r => r.Price)
            };

            // pagination
            var totalCount = response.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var paginatedData = response
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new TripSearchResponse
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = paginatedData
            };

            //return RedirectToAction(nameof(GetSearchTrips),result);
            return Ok(result);
        }


    }
}

