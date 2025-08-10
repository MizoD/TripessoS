using Microsoft.AspNetCore.Identity;
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
    public class TripController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public TripController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("index")]
        public async Task<IActionResult> GetTrips([FromQuery] int pageNumber = 1)
        {
            const int pageSize = 6;

            var allTrips = await unitOfWork.TripRepository.GetAllAvailableTripsAsync();

            int tripsCount = allTrips.Count();
            var pagedTrips = allTrips
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var tripHome = new { PagedTrips = pagedTrips, CurrentPage = pageNumber, TotalPages = (int)Math.Ceiling((double)tripsCount / pageSize)};
            return Ok(tripHome);
        }

        [HttpGet("TripDetails/{id}")]
        public async Task<IActionResult> TripDetails(int id)
        {
            var trip = await unitOfWork.TripRepository.GetTripWithDetailsAsync(id);
            if (trip == null)
                return NotFound("Trip not found");

            var relatedTrips = await unitOfWork.TripRepository.GetRelatedTripsAsync(trip);

            var tripDetails = new 
            {
                Trip = trip,
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

        [HttpGet("SearchTrips")]
        public async Task<IActionResult> GetSearchTrips(
            [FromQuery] TripSearchRequest request,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 6,
            [FromQuery] string sortBy = "price",
            [FromQuery] string sortOrder = "asc")
        {

            var twoWeeksBefore = request.DesiredDate.AddDays(-14);
            var twoWeeksAfter = request.DesiredDate.AddDays(14);

            var trips = await unitOfWork.TripRepository.GetAllAsync(
                filter: t =>
                    t.IsAvailable &&
                    t.AvailableSeats >= request.NumberOfPassengers &&
                    t.StartDate >= twoWeeksBefore &&
                    t.StartDate <= twoWeeksAfter &&
                    t.Country != null &&
                    t.Country.Name.ToLower().Contains(request.CountryName ?? "".ToLower()),
                includes: t => t.Include(t => t.Country).Include(t => t.Reviews).Include(t=> t.Hotels).Include(t=> t.Flights)
            );

            var response = trips.Select(t => new TripListResponse
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

            response = sortBy.ToLower() switch
            {
                "price" => sortOrder.ToLower() == "desc" ? response.OrderByDescending(r => r.Price) : response.OrderBy(r => r.Price),
                "rating" => sortOrder.ToLower() == "desc" ? response.OrderByDescending(r => r.AverageRating) : response.OrderBy(r => r.AverageRating),
                _ => response.OrderBy(r => r.Price)
            };

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

            return Ok(result);
        }

        [HttpPost("SearchTrips")]
        public async Task<IActionResult> PostSearchTrips(
        [FromBody] TripSearchRequest request,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "price",
        [FromQuery] string sortOrder = "asc")
        {
            var twoWeeksBefore = request.DesiredDate.AddDays(-14);
            var twoWeeksAfter = request.DesiredDate.AddDays(14);

            var trips = await unitOfWork.TripRepository.GetAllAsync(
                filter: t =>
                    t.IsAvailable &&
                    t.AvailableSeats >= request.NumberOfPassengers &&
                    t.StartDate >= twoWeeksBefore &&
                    t.StartDate <= twoWeeksAfter &&
                    t.Country != null &&
                    t.Country.Name.ToLower().Contains(request.CountryName ?? "".ToLower()),
                includes: q => q.Include(t => t.Country).Include(t => t.Reviews)
            );

            var response = trips.Select(t => new TripListResponse
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
            return Ok();
        }


    }
}

