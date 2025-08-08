using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var upuser = editProfileRequest.Adapt<ApplicationUser>();
            if (editProfileRequest.ImageUrl is not null && editProfileRequest.ImageUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(editProfileRequest.ImageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await editProfileRequest.ImageUrl.CopyToAsync(stream);
                }

                // Delete old Img in wwwroot
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", user.ImgUrl ?? " ");
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Update Img in DB
                upuser.ImgUrl = fileName;
            }
            else
            {
                upuser.ImgUrl = user.ImgUrl;
            }

            await unitOfWork.UserManager.UpdateAsync(upuser);
            return Ok("User Updated Succssefully");
        }
        [HttpGet("Bookings")]
        public async Task<IActionResult> Bookings()
        {
            var user = await unitOfWork.UserManager.GetUserAsync(User);
            if (user is null)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                user = await unitOfWork.UserManager.FindByIdAsync(userId);
            }
            if (user is null) return NotFound();

            return Ok();
        }
    }
}
