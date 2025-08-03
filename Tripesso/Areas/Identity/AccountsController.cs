using Mapster;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Tripesso.Areas.Identity
{
    [Route("api/[area]/[controller]")]
    [Area("Identity")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IEmailSender emailSender;
        private readonly IUnitOfWork unitOfWork;

        public AccountsController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            this.emailSender = emailSender;
            this.unitOfWork = unitOfWork;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest registerRequest)
        {
            var user = registerRequest.Adapt<ApplicationUser>();

            var result = await unitOfWork.UserManager.CreateAsync(user, registerRequest.Password);

            if (result.Succeeded)
            {
                await unitOfWork.UserManager.AddToRoleAsync(user, SD.Customer);

                // Send Confirmation Email
                var token = await unitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action("ConfirmEmail", "Accounts", new { userId = user.Id, token = token, area = "Identity" }, Request.Scheme);

                await emailSender.SendEmailAsync(user!.Email ?? "", "Confirm Your Account's Email", @$"
                        <div style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 30px; text-align: center;"">
                            <div style=""max-width: 500px; margin: auto; background-color: #ffffff; padding: 40px; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.1);"">
                                <h1 style=""color: #333333; margin-bottom: 20px;"">Confirm Your Email</h1>
                                <p style=""font-size: 16px; color: #666666; margin-bottom: 30px;"">
                                    Please confirm your account by clicking the button below.
                                </p>
                                <a href=""{link}"" style=""display: inline-block; padding: 14px 28px; background-color: #4CAF50; color: #ffffff; text-decoration: none; border-radius: 6px; font-weight: bold; transition: opacity 0.3s ease;""
                                    onmouseover=""this.style.opacity='0.8';"" onmouseout=""this.style.opacity='1';"">
                                    Confirm Email
                                </a>
                                <p style=""font-size: 12px; color: #aaaaaa; margin-top: 40px;"">
                                    If you did not request this email, you can safely ignore it.
                                </p>
                            </div>
                        </div>
                        ");

                return Ok("User Created Successfully");
            }
            return BadRequest(result.Errors);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await unitOfWork.UserManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var result = await unitOfWork.UserManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Your Email Confirmed Successfully");
            }
            else
            {
                return BadRequest($"{String.Join(",", result.Errors)}");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest loginRequest)
        {
            var user = await unitOfWork.UserManager.FindByEmailAsync(loginRequest.Email);
            if (user is null) return NotFound("Invalid Email");

            var result = await unitOfWork.SignInManager.PasswordSignInAsync(user, loginRequest.Password, loginRequest.RememberMe, true);

            if (result.Succeeded)
            {
                if (!user.EmailConfirmed)
                {
                    return BadRequest("Confirm Your Email, Please");
                }

                if (!user.LockoutEnabled)
                {
                    return BadRequest($"You have been blocked untill {user.LockoutEnd}");
                }
                var roles = await unitOfWork.UserManager.GetRolesAsync(user);

                var claims = new List<Claim> {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName!),
                        new Claim(ClaimTypes.Email, user.Email!),
                        new Claim(ClaimTypes.Role, String.Join(" ", roles))
                };

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TourismAPIForGradProject1stPTourismAPIForGradProject1stP"));

                var signInCredential = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "https://localhost:7072",
                    audience: "https://localhost:4200,https://localhost:5000",
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(3),
                    signingCredentials: signInCredential
                    );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else if (result.IsLockedOut)
            {
                return BadRequest("Too Many attempts, try again after 5 min");
            }

            return BadRequest("Invalid Email OR Password");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null!)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Accounts", new { returnUrl });
            var properties = unitOfWork.SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null!, string remoteError = null!)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return BadRequest();
            }

            var info = await unitOfWork.SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest();
            }

            // Try signing in with an external login
            var signInResult = await unitOfWork.SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl ?? "/");
            }

            // If the user cannot log in, try finding them by email
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            
            if (email != null)
            {
                var user = await unitOfWork.UserManager.FindByEmailAsync(email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name)!;

                if (user == null)
                {
                    // Create a new user if they do not exist
                    user = new ApplicationUser
                    {
                        FirstName = name,
                        LastName = string.Empty,
                        UserName = name.Replace(" ","") + Guid.NewGuid().ToString(),
                        Email = email
                    };
                    var createUserResult = await unitOfWork.UserManager.CreateAsync(user);
                    if (!createUserResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error creating user.");
                        return BadRequest();
                    }

                    // Add external login for new user
                    var addLoginResult = await unitOfWork.UserManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error linking external login.");
                        return BadRequest();
                    }
                }

                // Ensure the external login is linked
                var existingLogins = await unitOfWork.UserManager.GetLoginsAsync(user);
                var hasGoogleLogin = existingLogins.Any(l => l.LoginProvider == info.LoginProvider);

                if (!hasGoogleLogin)
                {
                    var addLoginResult = await unitOfWork.UserManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error linking external login.");
                        return Conflict("Email already in use with a different login method.");
                    }
                }

                // Sign in the user
                await unitOfWork.SignInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl ?? "/");
            }

            return BadRequest("Couldn't find User Email");
        }

        [HttpPost("SignOut")]
        public new async Task<IActionResult> SignOut()
        {
            await unitOfWork.SignInManager.SignOutAsync();
            return Ok("Logged out Successfully");
        }

        [HttpPost("ResendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirmation([FromForm] ResendEmailConfirmationRequest resendEmailConfirmationRequest)
        {

            var user = await unitOfWork.UserManager.FindByEmailAsync(resendEmailConfirmationRequest.Email);

            if (user is null)
                return BadRequest("Invalid Email");


            // Send Confirmation Email
            var token = await unitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Accounts", new { userId = user.Id, token = token, area = "Identity" }, Request.Scheme);

            await emailSender.SendEmailAsync(user!.Email ?? "", "Confirm Your Account's Email", @$"
                        <div style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 30px; text-align: center;"">
                            <div style=""max-width: 500px; margin: auto; background-color: #ffffff; padding: 40px; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.1);"">
                                <h1 style=""color: #333333; margin-bottom: 20px;"">Confirm Your Email</h1>
                                <p style=""font-size: 16px; color: #666666; margin-bottom: 30px;"">
                                    Please confirm your account by clicking the button below.
                                </p>
                                <a href=""{link}"" style=""display: inline-block; padding: 14px 28px; background-color: #4CAF50; color: #ffffff; text-decoration: none; border-radius: 6px; font-weight: bold; transition: opacity 0.3s ease;""
                                    onmouseover=""this.style.opacity='0.8';"" onmouseout=""this.style.opacity='1';"">
                                    Confirm Email
                                </a>
                                <p style=""font-size: 12px; color: #aaaaaa; margin-top: 40px;"">
                                    If you did not request this email, you can safely ignore it.
                                </p>
                            </div>
                        </div>
                        ");

            return Ok("Sent Email Successfully");
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromForm] ForgetPasswordRequest forgetPasswordRequest)
        {
            var user = await unitOfWork.UserManager.FindByEmailAsync(forgetPasswordRequest.Email);

            if (user is null)
                return BadRequest("Invalid Email");

            // Send OTP Email
            var otpNumber = new Random().Next(0, 999999).ToString("D6");

            var totalNumberOfOTPs = (await unitOfWork.ApplicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == user.Id && DateTime.UtcNow.Day == e.SendDate.Day));

            if (totalNumberOfOTPs.Count() > 5)
            {
                return BadRequest("Many OTP Requests");
            }

            await unitOfWork.ApplicationUserOTPRepository.CreateAsync(new()
            {
                ApplicationUserId = user.Id,
                OTPNumber = otpNumber,
                Reason = "ForgetPassword",
                SendDate = DateTime.UtcNow,
                Status = false,
                ValidTo = DateTime.UtcNow.AddMinutes(30)
            });

            await emailSender.SendEmailAsync(user!.Email ?? "", "Reset Password OTP", @$"
                        <div style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 30px; text-align: center;"">
                            <div style=""max-width: 500px; margin: auto; background-color: #ffffff; padding: 40px; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.1);"">
                                <h1 style=""color: #333333; margin-bottom: 20px;"">Reset Your Password</h1>
                                <p style=""font-size: 16px; color: #666666; margin-bottom: 30px;"">
                                    Use the OTP below to reset your password:
                                </p>
                                <div style=""display: inline-block; font-size: 32px; letter-spacing: 8px; background-color: #f9f9f9; color: #333333; padding: 12px 24px; border-radius: 8px; font-weight: bold; margin-bottom: 30px; box-shadow: inset 0 1px 3px rgba(0,0,0,0.1);"">
                                    {otpNumber}
                                </div>
                                <p style=""font-size: 14px; color: #aaaaaa; margin-top: 30px;"">
                                    This OTP is valid for a limited time. If you did not request this, please ignore this email.
                                </p>
                            </div>
                        </div>
                        ");

            return Ok("Send OTP to your Email Successfully");
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordRequest resetPasswordRequest)
        {
            var user = await unitOfWork.UserManager.FindByIdAsync(resetPasswordRequest.UserId);

            if (user is null)
                return NotFound();

            var lastOTP = (await unitOfWork.ApplicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == resetPasswordRequest.UserId)).OrderBy(e => e.Id).LastOrDefault();

            if (lastOTP is not null)
            {
                if (lastOTP.OTPNumber == resetPasswordRequest.OTP && (lastOTP.ValidTo - DateTime.UtcNow).TotalMinutes < 30 && !lastOTP.Status)
                {
                    var token = await unitOfWork.UserManager.GeneratePasswordResetTokenAsync(user);
                    var result = await unitOfWork.UserManager.ResetPasswordAsync(user, token, resetPasswordRequest.Password);

                    if (result.Succeeded)
                    {
                        lastOTP.Status = true;
                        await unitOfWork.ApplicationUserOTPRepository.CommitAsync();
                        return Ok("Your Password Resetted Successfully");
                    }
                    else
                    {
                        return BadRequest($"{String.Join(",", result.Errors)}");
                    }
                }
            }

            return BadRequest("Invalid OR Expired OTP");
        }
    }
}
