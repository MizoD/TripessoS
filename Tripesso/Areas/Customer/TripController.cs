using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Request.TripRequest;
using Models.DTOs.Response.TripResponse;

namespace Tripesso.Areas.Customer.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class TripController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITripRepository _tripRepository;
        private readonly ApplicationDbContext _context;
        private readonly ICartRepository _cartRepository;
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IReviewRepository _reviewRepository;


        public TripController(
           UserManager<ApplicationUser> userManager,
           ITripRepository tripRepository,
           ICartRepository cartRepository,
           IWishlistRepository wishlistRepository,
           IReviewRepository reviewRepository,
           ApplicationDbContext context)
        {
            _userManager = userManager;
            _tripRepository = tripRepository;
            _context = context;
            _cartRepository = cartRepository;
            _wishlistRepository = wishlistRepository;
            _reviewRepository = reviewRepository;
        }

        [HttpGet("index")]
        public async Task<IActionResult> GetTrips([FromQuery] int pageNumber = 1)
        {
            const int pageSize = 6;

            var allTrips = await _tripRepository.GetAllAvailableTripsAsync();

            var pagedTrips = allTrips
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TripListResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    CountryName = t.Country.Name,
                    StartDate = t.StartDate,
                    Price = t.Price,
                    ImageUrl = t.ImageUrl,
                    DurationDays = t.DurationDays,
                    IsAvailable = t.IsAvailable,
                    AverageRating = t.Reviews.Any() ? t.Reviews.Average(r => r.Rating) : 0
                })
                .ToList();

            return Ok(pagedTrips);
        }

        [HttpGet("tripdetails/{id}")]
        public async Task<IActionResult> GetTripDetails(int id)
        {
            var trip = await _tripRepository.GetTripWithDetailsAsync(id);
            if (trip == null)
                return NotFound("Trip not found");

            var relatedTrips = await _tripRepository.GetRelatedTripsAsync(trip);

            var tripDetailsDto = new TripDetailsResponse
            {
                Id = trip.Id,
                Title = trip.Title,
                Description = trip.Description,
                CountryName = trip.Country.Name,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Price = trip.Price,
                ImageUrl = trip.ImageUrl,
                DurationDays = (trip.EndDate - trip.StartDate).Days,
                IsAvailable = trip.AvailableSeats > 0,

                RelatedTrips = relatedTrips.Select(t => new TripListResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    CountryName = t.Country.Name,
                    StartDate = t.StartDate,
                    Price = t.Price,
                    ImageUrl = t.ImageUrl,
                    DurationDays = (t.EndDate - t.StartDate).Days,
                    IsAvailable = t.AvailableSeats > 0,
                    AverageRating = t.Reviews.Any() ? t.Reviews.Average(r => r.Rating) : 0
                }).ToList(),

                Reviews = trip.Reviews.Select(r => new ReviewResponse
                {
                    ReviewId = r.Id,
                    UserName = r.User.UserName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList()
            };

            return Ok(tripDetailsDto);
        }
        [HttpPost("addtocart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var trip = await _tripRepository.GetOneAsync(t => t.Id == request.TripId);

            if (trip == null)
                return NotFound("Trip not found.");

            if (trip.AvailableSeats < request.NumberOfPassengers)
                return BadRequest("Not enough available seats.");

            var userId = User.FindFirst("sub")?.Value;
            if (userId == null)
                return Unauthorized();

            // Check if the trip is already in the user's cart
            var existingCartItem = await _cartRepository.GetOneAsync(
                c => c.UserId == userId && c.TripId == request.TripId
            );

            if (existingCartItem != null)
                return BadRequest("This trip is already in your cart.");

            var cartItem = new Cart
            {
                TripId = trip.Id,
                UserId = userId,
                NumberOfPassengers = request.NumberOfPassengers,
                AddedAt = DateTime.UtcNow
            };

            var added = await _cartRepository.CreateAsync(cartItem);
            if (!added)
                return StatusCode(500, "Something went wrong while adding to cart.");

            // Update available seats
            trip.AvailableSeats -= request.NumberOfPassengers;
            await _tripRepository.UpdateAsync(trip);

            // response
            var response = new AddToCartResponse
            {
                CartId = cartItem.Id,
                TripId = trip.Id,
                TripTitle = trip.Title,
                NumberOfPassengers = request.NumberOfPassengers,
                TotalPrice = trip.Price * request.NumberOfPassengers,
                AddedAt = cartItem.AddedAt
            };

            return Ok(response);
        }

        [HttpPost("addtowishlist")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst("sub")?.Value;
            if (userId == null)
                return Unauthorized();

            var trip = await _tripRepository.GetOneAsync(
                t => t.Id == request.TripId,
                includes: q => q.Include(t => t.Country)
            );

            if (trip == null)
                return NotFound("Trip not found.");

            // Check if it's already in wishlist
            var existingWishlistItem = await _wishlistRepository.GetOneAsync(
                w => w.UserId == userId && w.TripId == request.TripId
            );

            if (existingWishlistItem != null)
                return BadRequest("This trip is already in your wishlist.");

            var wishlistItem = new Wishlist
            {
                TripId = request.TripId,
                UserId = userId,
                AddedAt = DateTime.UtcNow
            };

            var added = await _wishlistRepository.CreateAsync(wishlistItem);
            if (!added)
                return StatusCode(500, "Something went wrong while adding to wishlist.");

            var response = new AddToWishlistResponse
            {
                WishlistId = wishlistItem.Id,
                TripId = trip.Id,
                TripTitle = trip.Title,
                CountryName = trip.Country.Name,
                Price = trip.Price,
                ImageUrl = trip.ImageUrl,
                AddedAt = wishlistItem.AddedAt
            };

            return Ok(response);
        }

        [HttpPost("review")]
        public async Task<IActionResult> AddReview([FromBody] AddReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst("sub")?.Value;
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            var trip = await _tripRepository.GetOneAsync(
                t => t.Id == request.TripId,
                includes: q => q.Include(t => t.Reviews)
            );

            if (trip == null)
                return NotFound("Trip not found.");

            if (trip.AvailableSeats == 0)
                return BadRequest("Trip is not available for review.");

            // Optional: Check if user already reviewed this trip
            var alreadyReviewed = trip.Reviews.Any(r => r.UserId == userId);
            if (alreadyReviewed)
                return BadRequest("You have already reviewed this trip.");

            var review = new Review
            {
                TripId = trip.Id,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            var success = await _reviewRepository.CreateAsync(review);
            if (!success)
                return StatusCode(500, "Something went wrong while saving the review.");

            var response = new ReviewResponse
            {
                ReviewId = review.Id,
                TripId = review.TripId,
                UserName = user.UserName,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };

            return Ok(response);
        }

        [HttpGet("tripsearch")]
        public async Task<IActionResult> SearchTrips(
            [FromQuery] TripSearchRequest request,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "price",
            [FromQuery] string sortOrder = "asc")
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var twoWeeksBefore = request.DesiredDate.AddDays(-14);
            var twoWeeksAfter = request.DesiredDate.AddDays(14);

            var trips = await _tripRepository.GetAllAsync(
                filter: t =>
                    t.IsAvailable &&
                    t.AvailableSeats >= request.NumberOfPassengers &&
                    t.StartDate >= twoWeeksBefore &&
                    t.StartDate <= twoWeeksAfter &&
                    t.Country != null &&
                    t.Country.Name.ToLower().Contains(request.CountryName.ToLower()),
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

        [HttpPost("tripsearch")]
        public async Task<IActionResult> PostSearchTrips(
        [FromBody] TripSearchRequest request,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "price",
        [FromQuery] string sortOrder = "asc")
        {
            // Validate request model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // إعادة استخدام منطق البحث الموجود في GET
            var twoWeeksBefore = request.DesiredDate.AddDays(-14);
            var twoWeeksAfter = request.DesiredDate.AddDays(14);

            var trips = await _tripRepository.GetAllAsync(
                filter: t =>
                    t.IsAvailable &&
                    t.AvailableSeats >= request.NumberOfPassengers &&
                    t.StartDate >= twoWeeksBefore &&
                    t.StartDate <= twoWeeksAfter &&
                    t.Country != null &&
                    t.Country.Name.ToLower().Contains(request.CountryName.ToLower()),
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

            return Ok(result);
        }


    }
}

