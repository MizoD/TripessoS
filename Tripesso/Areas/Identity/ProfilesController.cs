using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace BookStore.Areas.Identity.Controllers
{
    [Route("api/[area]/[controller]")]
    [Area("Identity")]
    [ApiController]
    [Authorize]
    public class ProfilesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public ProfilesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("Details")]
        public async Task<IActionResult> Details()
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user is null) return NotFound();

            return Ok(user);
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit([FromForm] EditProfileRequest editProfileRequest)
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user is null) return NotFound();

            var upuser = await unitOfWork.UserManager.FindByIdAsync(user.Id);
            if (upuser is null) return NotFound();

            if (!string.IsNullOrWhiteSpace(editProfileRequest.FirstName))
            {
                upuser.FirstName = editProfileRequest.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(editProfileRequest.LastName))
            {
                upuser.LastName = editProfileRequest.LastName;
            }

            if (!string.IsNullOrWhiteSpace(editProfileRequest.Email))
            {
                if (!new EmailAddressAttribute().IsValid(editProfileRequest.Email))
                {
                    return BadRequest("Invalid email format");
                }

                // Check if email is already taken by another user
                var emailOwner = await unitOfWork.UserManager.FindByEmailAsync(editProfileRequest.Email);
                if (emailOwner != null && emailOwner.Id != upuser.Id)
                {
                    return BadRequest("Email is already in use by another account");
                }

                upuser.Email = editProfileRequest.Email;
                upuser.NormalizedEmail = editProfileRequest.Email.ToUpper();
            }

            if (!string.IsNullOrWhiteSpace(editProfileRequest.PhoneNumber))
            {
                upuser.PhoneNumber = editProfileRequest.PhoneNumber;
            }

            if (editProfileRequest.Address != null)
            {
                upuser.Address = editProfileRequest.Address;
            }

            if (editProfileRequest.ImageUrl != null && editProfileRequest.ImageUrl.Length > 0)
            {
                // Validate image
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(editProfileRequest.ImageUrl.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("Invalid image format. Only JPG, PNG, and GIF are allowed.");
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                // Ensure directory exists
                Directory.CreateDirectory(imagesPath);

                // Save new image
                var filePath = Path.Combine(imagesPath, fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await editProfileRequest.ImageUrl.CopyToAsync(stream);
                }

                // Delete old image if exists
                if (!string.IsNullOrEmpty(upuser.ImgUrl))
                {
                    var oldFilePath = Path.Combine(imagesPath, upuser.ImgUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                            System.IO.File.Delete(oldFilePath);
                    }
                }

                upuser.ImgUrl = fileName;
            }

            var result = await unitOfWork.UserManager.UpdateAsync(upuser);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            try
            {
                await unitOfWork.CommitAsync();
                return Ok( "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating your profile : {ex}");
            }
        }

        [HttpGet("DashBoard")]
        public async Task<IActionResult> Dashboard()
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user is null) return Unauthorized("Can't Found Such a user!");

            var bookingsCount = (await unitOfWork.BookingRepository.GetAllAsync(b=> b.UserId == user.Id)).Count(); 

            var hotelWishlistCount = (await unitOfWork.HotelWishlistRepository.GetAllAsync(h=> h.UserId == user.Id)).Count();
            var tripWishlistCount = (await unitOfWork.TripWishlistRepository.GetAllAsync(h => h.UserId == user.Id)).Count();
            var flightWishlistCount = (await unitOfWork.FlightWishlistRepository.GetAllAsync(h => h.UserId == user.Id)).Count();

            var allwishlistCount = hotelWishlistCount + tripWishlistCount + flightWishlistCount;
            
            var flightsBookedCount = (await unitOfWork.BookingRepository.GetAllAsync(b=> b.UserId == user.Id && b.FlightId > 0)).Count();
            var userReviewsCount = (await unitOfWork.ReviewRepository.GetAllAsync(r=> r.UserId == user.Id)).Count();

            var bookings = await unitOfWork.BookingRepository.GetAllAsync(b=> b.UserId == user.Id);

            return Ok(new
            {
                Bookings = bookings,
                BookingsCount = bookingsCount,
                AllwishlistCount = allwishlistCount,
                FlightsBookedCount = flightsBookedCount,
                UserReviewsCount = userReviewsCount
            });
        }


        [HttpGet("MyBookings")]
        public async Task<IActionResult> MyBookings()
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user is null) return Unauthorized("Can't Found Such a user!");

            var myBookings = await unitOfWork.BookingRepository.GetAllAsync(b=> b.UserId == user.Id);
            if (myBookings == null) return Ok("There is no Bookings yet!");

            return Ok(myBookings);
        }

        [HttpGet("MyReviews")]
        public async Task<IActionResult> MyReviewsAsync() {

            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user is null) return Unauthorized("Can't Found Such a user!");

            var myReviews = await unitOfWork.ReviewRepository.GetAllAsync(r=> r.UserId == user.Id);
            if (myReviews == null) return Ok("There is no Reviews yet!");

            return Ok(myReviews);
        }

        [HttpGet("MyWishlist")]
        public async Task<IActionResult> MyWishlist()
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user is null) return Unauthorized("Can't Found Such a user!");

            var myTripsWishlist = await unitOfWork.TripWishlistRepository.GetAllAsync(t=> t.UserId == user.Id);

            var myFlightsWishlist = await unitOfWork.FlightWishlistRepository.GetAllAsync(f=> f.UserId == user.Id);

            var myHotelsWishlist = await unitOfWork.HotelWishlistRepository.GetAllAsync(h=> h.UserId == user.Id);

            if (myTripsWishlist == null && myFlightsWishlist == null && myHotelsWishlist == null) return Ok("There is no Wishlists yet!");


            return Ok(new
            {
                MyTripsWishlist = myTripsWishlist,
                MyFlightsWishlist = myFlightsWishlist,
                MyHotelsWishlist = myHotelsWishlist
            });
        }


        [HttpGet("Logout")]
        public async Task<IActionResult> LogoutAsync() {

            await unitOfWork.SignInManager.SignOutAsync();

            return Ok("User Logged Out Successfully!");

        }
    }
}
