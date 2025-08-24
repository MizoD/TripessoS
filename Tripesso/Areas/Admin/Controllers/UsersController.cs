using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Request.UserRequest;
using Models.DTOs.Response.UserResponse;

namespace Tripesso.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            var query = unitOfWork.UserManager.Users.AsQueryable();

            var totalUsers = await query.CountAsync();
            int pageSize = 10;
            var users = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var userResponses = new List<UserResponse>();

            foreach (var user in users)
            {
                var roles = await unitOfWork.UserManager.GetRolesAsync(user);
                userResponses.Add(new UserResponse
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = user.Address ?? "",
                    Roles = roles.ToList()
                });
            }

            var response = new PagedUserResponse
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalUsers = totalUsers,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                Users = userResponses
            };

            return Ok(response);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var user = await unitOfWork.UserManager.FindByIdAsync(id);

            if (user == null) return NotFound("User not found");

            var roles = await unitOfWork.UserManager.GetRolesAsync(user);

            var response = new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address ?? "",
                Roles = roles.ToList()
            };

            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                Email = request.Email,
                Address = request.Address,
                RegistrationDate = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };

            var result = await unitOfWork.UserManager.CreateAsync(user, request.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            if (!await unitOfWork.RoleManager.RoleExistsAsync(request.Role))
                return BadRequest("Invalid role");

            await unitOfWork.UserManager.AddToRoleAsync(user, request.Role);

            return Ok(new { message = "User created successfully", userId = user.Id });
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
        {
            var user = await unitOfWork.UserManager.FindByIdAsync(request.Id);

            if (user == null) return NotFound("User not found");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.UserName = request.UserName;
            user.Email = request.Email;
            user.Address = request.Address;

            var updated = await unitOfWork.UserManager.UpdateAsync(user);
            if (!updated.Succeeded) return BadRequest(updated.Errors);

            var userRoles = await unitOfWork.UserManager.GetRolesAsync(user);
            await unitOfWork.UserManager.RemoveFromRolesAsync(user, userRoles);
            await unitOfWork.UserManager.AddToRolesAsync(user, request.Roles);

            return Ok(new { message = "User updated successfully" });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await unitOfWork.UserManager.FindByIdAsync(id);

            if (user == null) return NotFound("User not found");

            var deleted =  await unitOfWork.UserManager.DeleteAsync(user);
            if (!deleted.Succeeded) return BadRequest(deleted.Errors);

            return Ok(new { message = "User deleted successfully" });
        }

        [HttpPost("Block/{id}")]
        public async Task<IActionResult> Block(string id)
        {
            var user = await unitOfWork.UserManager.FindByIdAsync(id);

            if (user is not null)
            {
                if (user.LockoutEnabled)
                {
                    user.LockoutEnabled = false;
                    user.LockoutEnd = DateTime.UtcNow.AddMonths(1000);
                }
                else
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = null;
                }

                var blocked =  await unitOfWork.UserManager.UpdateAsync(user);
                if (!blocked.Succeeded) return BadRequest();
            }

            return NotFound();
        }
    }
}
