using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace Tripesso
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Custom Services & Repositories
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IApplicationUserOTPRepository, ApplicationUserOTPRepository>();
            builder.Services.AddScoped<IAirCraftRepository, AirCraftRepository>();
            builder.Services.AddScoped<IAirportRepository, AirportRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<ICountryRepository, CountryRepository>();
            builder.Services.AddScoped<IFlightRepository, FlightRepository>();
            builder.Services.AddScoped<IHotelRepository, HotelRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<ISeatRepository, SeatRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<ITripRepository, TripRepository>();
            builder.Services.AddScoped<ITripCartRepository, TripCartRepository>();
            builder.Services.AddScoped<IFlightCartRepository, FlightCartRepository>();
            builder.Services.AddScoped<IHotelCartRepository, HotelCartRepository>();
            builder.Services.AddScoped<ITripWishlistRepository, TripWishlistRepository>();
            builder.Services.AddScoped<IHotelWishlistRepository, HotelWishlistRepository>();
            builder.Services.AddScoped<IFlightWishlistRepository, FlightWishlistRepository>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Google Authentication
            builder.Services.AddAuthentication()
                .AddGoogle("google", opt =>
                {
                    var googleAuth = builder.Configuration.GetSection("Authentication:Google");
                    opt.ClientId = googleAuth["ClientId"]!;
                    opt.ClientSecret = googleAuth["ClientSecret"]!;
                    opt.SignInScheme = IdentityConstants.ExternalScheme;
                });

            // Application Cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Accounts/Login";
                options.AccessDeniedPath = "/Identity/Accounts/AccessDenied";
            });

            // JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = "http://localhost:4200", // Angular/React Frontend
                    ValidIssuer = "https://localhost:7200",  // Backend API
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("TourismAPIForGradProject1stPTourismAPIForGradProject1stP")
                    ),
                    ValidateLifetime = true
                };
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:4200", "http://localhost:5500")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Stripe
            Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();

            // Middleware pipeline
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // DB Initializer
            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                dbInitializer.Initialize();
            }

            app.Run();
        }
    }
}
