using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Tripesso.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public WishlistsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user is null) return Unauthorized("Please Login first!");

            var wishlistItems = await unitOfWork.FlightWishlistRepository.GetAsync(w => w.UserId == user.Id,
                includes: w => w.Include(w => w.Flight)
                    .ThenInclude(f => f.DepartureAirport)
                    .Include(x => x.Flight)
                    .ThenInclude(f => f.ArrivalAirport)
                    .Include(x => x.Flight)
                    .ThenInclude(f => f.Aircraft)
            );

            return Ok(wishlistItems);
        }

        [HttpPost("remove/{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user is null) return Unauthorized("Please Login first!");

            var wishlistItem = await unitOfWork.FlightWishlistRepository.GetOneAsync(
                                                w => w.UserId == user.Id && w.FlightId == id);

            if (wishlistItem == null)  return NotFound();
            
            var removed = await unitOfWork.FlightWishlistRepository.DeleteAsync(wishlistItem);
            if (!removed)
                return StatusCode(500, "Something went wrong while removing the item from wishlist.");

            return Ok("Flight removed successfully!");
        }
    }
}