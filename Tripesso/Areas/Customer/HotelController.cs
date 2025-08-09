using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Request.HotelRequest;
using Models.DTOs.Response.HotelResponse;
using Models.DTOs.Response.TripResponse;
using System.Security.Claims;

namespace Tripesso.Areas.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHotelRepository _hotelRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IWishlistRepository _wishlistRepository;


        public HotelController(IHotelRepository hotelRepository, 
           ICartRepository cartRepository,
           IRoomRepository roomRepository,
           UserManager<ApplicationUser> userManager,
           IWishlistRepository wishlistRepository,
           IReviewRepository reviewRepository,
           ApplicationDbContext context
            )
        {
            _hotelRepository = hotelRepository;
            _cartRepository = cartRepository;
            _roomRepository = roomRepository;
            _wishlistRepository = wishlistRepository;
            _userManager = userManager;
            _reviewRepository = reviewRepository;
            _context = context;

        }

        [HttpGet]
        public async Task<IActionResult> GetHotels([FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 6,
            [FromQuery] string sortBy = "price", 
            [FromQuery] string sortOrder = "asc")
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 6;

            var hotels = await _hotelRepository.GetAllAsync(
                filter: h => h.Rooms != null && h.Rooms.Any(r => r.AvailableRooms > 0),
                includes: q => q.Include(h => h.Rooms).Include(h => h.Reviews),
                tracked: false
            );

            var mapped = hotels.Select(h =>
            {
                var rooms = h.Rooms ?? Enumerable.Empty<Models.Room>();
                decimal minPrice = rooms.Any() ? rooms.Min(r => r.PricePerNight) : 0;
                decimal maxPrice = rooms.Any() ? rooms.Max(r => r.PricePerNight) : 0;
                bool hasAvailability = rooms.Any(r => r.AvailableRooms > 0);
                double avgRating = h.Reviews != null && h.Reviews.Any() ? Math.Round(h.Reviews.Average(r => r.Rating), 1) : 0;

                return new HotelListResponse
                {
                    Id = h.Id,
                    Name = h.Name,
                    Location = h.Country?.Name ?? "", 
                    Rating = avgRating,
                    MinPricePerNight = minPrice,
                    MaxPricePerNight = maxPrice,
                    ThumbnailImageUrl = null, 
                    HasAvailability = hasAvailability
                };
            }).AsQueryable();

            // Sorting
            mapped = (sortBy.ToLower(), sortOrder.ToLower()) switch
            {
                ("rating", "desc") => mapped.OrderByDescending(x => x.Rating),
                ("rating", "asc") => mapped.OrderBy(x => x.Rating),
                ("price", "desc") => mapped.OrderByDescending(x => x.MinPricePerNight),
                _ => mapped.OrderBy(x => x.MinPricePerNight)
            };

            // pagination 
            var totalCount = mapped.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var data = mapped.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            var result = new PaginatedHotelResponse
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = data
            };

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotelDetails(int id)
        {
            var hotel = await _hotelRepository.GetFirstOrDefaultAsync(
                filter: h => h.Id == id,
                includes: q => q
                    .Include(h => h.Rooms)
                    .Include(h => h.Reviews)
                        .ThenInclude(r => r.User)
                    .Include(h => h.Country),
                tracked: false
            );

            if (hotel == null)
                return NotFound(new { message = "Hotel not found" });

            var response = new HotelDetailsResponse
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Location = hotel.Country?.Name ?? "",
                Rating = hotel.Reviews != null && hotel.Reviews.Any()
                            ? Math.Round(hotel.Reviews.Average(r => r.Rating), 1)
                            : 0,
                Rooms = hotel.Rooms.Select(r => new RoomListResponse
                {
                    Id = r.Id,
                    RoomName = r.Name,
                    Price = r.Price
                }).ToList()
            };

            return Ok(response);
        }
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(int hotelId, int roomId, DateTime checkInDate, DateTime checkOutDate, int numberOfGuests)
        {
            // 1. Check Date
            if (checkInDate >= checkOutDate || checkInDate < DateTime.Today)
            {
                return BadRequest("Invalid check-in/check-out dates.");
            }

            // 2. Check Room in Hotel
            var room = await _roomRepository.GetFirstOrDefaultAsync(r => r.Id == roomId && r.HotelId == hotelId);
            if (room == null)
            {
                return BadRequest("Room does not belong to the specified hotel.");
            }

            // 3. Check Availabilty
            bool isAvailable = _roomRepository.CheckAvailability(roomId, checkInDate, checkOutDate);
            if (!isAvailable)
            {
                return BadRequest("Room is not available for the selected dates.");
            }

            // 4. Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not logged in.");
            }

            // 5. Add to Cart
            var cartItem = new Cart
            {
                UserId = user.Id,
                HotelId = hotelId,
                RoomId = roomId,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                NumberOfGuests = numberOfGuests,
                AddedAt = DateTime.Now
            };

            _context.Carts.Add(cartItem);
            await _context.SaveChangesAsync();

            return Ok("Room added to cart successfully.");
        }

        [HttpPost("AddToWishlist")]
        public async Task<IActionResult> AddToWishlist(int hotelId)
        {
            // check if hotel has empty rooms
            var hotel = await _hotelRepository.GetFirstOrDefaultAsync(
                filter: h => h.Id == hotelId && h.Rooms.Any(r => r.AvailableRooms > 0),
                includes: q => q.Include(h => h.Rooms),
                tracked: false
            );

            if (hotel == null)
            {
                return BadRequest("No available rooms for this hotel.");
            }

            // 2.get current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not logged in.");
            }

            // 3. Check if the hotel in wishlist
            var existingWishlist = await _wishlistRepository.GetAllAsync(
                filter: w => w.HotelId == hotelId && w.UserId == userId
            );

            if (existingWishlist.Any())
            {
                return BadRequest("Hotel already in wishlist.");
            }

            // 4. Save to Wishlist
            var wishlistItem = new Wishlist
            {
                HotelId = hotelId,
                UserId = userId,
                AddedAt = DateTime.UtcNow
            };

            await _wishlistRepository.CreateAsync(wishlistItem);
            await _wishlistRepository.CommitAsync();

            return Ok("Hotel added to wishlist successfully.");
        }

        [HttpPost("Review")]
        [Authorize]
        public async Task<IActionResult> AddHotelReview([FromBody] AddHotelReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User not logged in.");

            string userId = user.Id;

            // 2. Check if user booked at the hotel 
            bool hasCart = await _cartRepository
                .AnyAsync(c => c.UserId == userId && c.HotelId == request.HotelId);

            bool hasBooking = await _context.Bookings
                .AnyAsync(b =>
                    (b.User != null && b.User.Id == userId) &&
                    (b.Hotels.Any(h => h.Id == request.HotelId) ||
                     b.Rooms.Any(r => r.HotelId == request.HotelId))
                );

            if (!hasCart && !hasBooking)
                return Forbid("You can only review hotels you have booked or stayed at.");

            // 3. Stop duplicate reviews
            bool alreadyReviewed = await _reviewRepository
                .AnyAsync(r => r.HotelId == request.HotelId && r.UserId == userId);

            if (alreadyReviewed)
                return BadRequest("You have already reviewed this hotel.");

            // 4. Create review
            var review = new Review
            {
                HotelId = request.HotelId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.CreateAsync(review);
            await _reviewRepository.CommitAsync();

            // 5. Response (dynamic TripId / HotelId)
            var response = new HotelReviewResponse
            {
                Id = review.Id,
                HotelId = review.HotelId,
                UserName = user.UserName,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };

            return Ok(response);
        }

        [HttpGet("Hotels/Search")]
        public IActionResult SearchHotels(string keyword, int guests)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Keyword (country or hotel name) is required.");

            if (guests <= 0)
                return BadRequest("Number of guests must be greater than zero.");

            keyword = keyword.ToLower();

            var hotels = _context.Hotels
                .Where(h =>
                    (h.Country.Name != null && h.Country.Name.ToLower().Contains(keyword)) ||
                    h.Name.ToLower().Contains(keyword)
                )
                .Select(h => new
                {
                    HotelId = h.Id,
                    HotelName = h.Name,
                    Country = h.Country.Name,
                    Rooms = h.Rooms
                        .Where(r => r.IsAvailable && r.AvailableRooms > 0 && r.Capacity >= guests)
                        .Select(r => new
                        {
                            RoomId = r.Id,
                            r.Name,
                            r.Capacity,
                            r.PricePerNight,
                            r.AvailableRooms
                        })
                        .ToList()
                })
                .Where(h => h.Rooms.Any()) 
                .ToList();

            if (!hotels.Any())
                return NotFound("No hotels match the search criteria.");

            return Ok(hotels);
        }

        [HttpPost("Hotels/Search")]
        public async Task<IActionResult> PostSearchHotels(
    [FromBody] HotelSearchRequest request,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string sortBy = "price",
    [FromQuery] string sortOrder = "asc")
        {
            // Validate request model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // حساب فترة البحث ± 14 يوم من تاريخ الدخول المطلوب
            var twoWeeksBefore = request.CheckInDate.AddDays(-14);
            var twoWeeksAfter = request.CheckInDate.AddDays(14);

            // جلب الفنادق المتوافقة
            var hotels = await _hotelRepository.GetAllAsync(
                filter: h =>
                    h.Rooms.Any(r =>
                        r.IsAvailable &&
                        r.AvailableRooms > 0 &&
                        r.Capacity >= request.NumberOfGuests
                    ) &&
                    h.Country != null &&
                    h.Country.Name.ToLower().Contains(request.LocationOrCountry.ToLower()),
                includes: q => q
                    .Include(h => h.Country)
                    .Include(h => h.Rooms)
                    .Include(h => h.Reviews)
            );

            // تحويل البيانات لـ DTO
            var response = hotels.Select(h => new HotelListResponse
            {
                Id = h.Id,
                Name = h.Name,
                Location = h.Country?.Name ?? string.Empty,
                Rating = h.Reviews != null && h.Reviews.Any()
                    ? Math.Round(h.Reviews.Average(r => r.Rating), 1)
                    : 0,
                MinPricePerNight = h.Rooms.Any() ? h.Rooms.Min(r => r.PricePerNight) : 0,
                MaxPricePerNight = h.Rooms.Any() ? h.Rooms.Max(r => r.PricePerNight) : 0,
                ThumbnailImageUrl = h.ThumbnailImageUrl,
                HasAvailability = h.Rooms.Any(r => r.AvailableRooms > 0)
            });

            // الترتيب
            response = sortBy.ToLower() switch
            {
                "price" => sortOrder.ToLower() == "desc"
                    ? response.OrderByDescending(r => r.MinPricePerNight)
                    : response.OrderBy(r => r.MinPricePerNight),
                "rating" => sortOrder.ToLower() == "desc"
                    ? response.OrderByDescending(r => r.Rating)
                    : response.OrderBy(r => r.Rating),
                _ => response.OrderBy(r => r.MinPricePerNight)
            };

            // Pagination
            var totalCount = response.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var paginatedData = response
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new HotelSearchResponse
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
