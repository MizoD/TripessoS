using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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

            // 5. Save changes
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
