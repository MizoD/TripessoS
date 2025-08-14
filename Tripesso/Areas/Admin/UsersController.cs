using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs.Request.UserRequest;
using Models.DTOs.Response.UserResponse;

namespace Tripesso.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Route("admin/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ✅ GET: admin/users?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 10)
        {
            var query = _userManager.Users.AsQueryable();

            var totalUsers = await query.CountAsync();
            var users = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var userResponses = new List<UserResponse>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
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

        // ✅ GET: admin/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound("User not found");

            var roles = await _userManager.GetRolesAsync(user);

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

        // ✅ POST: admin/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                Email = request.Email,
                Address = request.Address,
                RegistraionDate = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            if (!await _roleManager.RoleExistsAsync(request.Role))
                return BadRequest("Invalid role");

            await _userManager.AddToRoleAsync(user, request.Role);

            return Ok(new { message = "User created successfully", userId = user.Id });
        }

        // ✅ PUT: admin/users
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user == null) return NotFound("User not found");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.UserName = request.UserName;
            user.Email = request.Email;
            user.Address = request.Address;

            await _userManager.UpdateAsync(user);

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRolesAsync(user, request.Roles);

            return Ok(new { message = "User updated successfully" });
        }

        // ✅ DELETE: admin/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound("User not found");

            await _userManager.DeleteAsync(user);

            return Ok(new { message = "User deleted successfully" });
        }

        // ✅ POST: admin/users/block/{id}
        [HttpPost("block/{id}")]
        public async Task<IActionResult> BlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound("User not found");

            user.LockoutEnd = DateTime.UtcNow.AddYears(100); // block "forever"
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "User blocked successfully" });
        }

        // ✅ POST: admin/users/unblock/{id}
        [HttpPost("unblock/{id}")]
        public async Task<IActionResult> UnblockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound("User not found");

            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "User unblocked successfully" });
        }
    }
}
