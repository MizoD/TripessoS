using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repositories.IRepositories;
using Models;
using System.Security.Claims;

namespace Tripesso.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    [Route("customer/[controller]")]
    public class WishlistsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WishlistsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: customer/wishlists
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var wishlistItems = await _unitOfWork.FlightWishlistRepository.GetAllAsync(
                filter: w => w.UserId == userId,
                include: w => w.Include(x => x.Flight)
                    .ThenInclude(f => f.DepartureAirport)
                    .Include(x => x.Flight)
                    .ThenInclude(f => f.ArrivalAirport)
                    .Include(x => x.Flight)
                    .ThenInclude(f => f.Aircraft)
            );

            return View(wishlistItems);
        }

        // POST: customer/wishlists/remove/5
        [HttpPost("remove/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlistItem = await _unitOfWork.FlightWishlistRepository.GetFirstOrDefaultAsync(
                w => w.Id == id && w.UserId == userId
            );

            if (wishlistItem == null)
            {
                return NotFound();
            }

            _unitOfWork.FlightWishlistRepository.Remove(wishlistItem);
            await _unitOfWork.CommitAsync();

            TempData["Success"] = "Item removed from wishlist successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}