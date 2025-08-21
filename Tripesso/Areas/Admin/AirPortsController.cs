using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Tripesso.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    [Route("admin/[area]/[controller]")]
    [ApiController]
    public class AirportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AirportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: admin/airports?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var query = _context.Airports.Include(a => a.Country);

            var totalCount = await query.CountAsync();

            var airports = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.City,
                    a.CountryId,
                    CountryName = a.Country.Name
                })
                .ToListAsync();

            return Ok(new
            {
                Data = airports,
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }

        // GET: admin/airports/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var airport = await _context.Airports
                .Include(a => a.Country)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (airport == null)
                return NotFound("Airport not found");

            return Ok(new
            {
                airport.Id,
                airport.Name,
                airport.City,
                airport.CountryId,
                CountryName = airport.Country.Name
            });
        }

        // POST: admin/airports
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Airport request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Airports.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Airport created successfully!", airportId = request.Id });
        }

        // PUT: admin/airports/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Airport request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var airport = await _context.Airports.FindAsync(id);
            if (airport == null)
                return NotFound("Airport not found");

            airport.Name = request.Name;
            airport.City = request.City;
            airport.CountryId = request.CountryId;

            _context.Airports.Update(airport);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✏️ Airport updated successfully!" });
        }

        // DELETE: admin/airports/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var airport = await _context.Airports.FindAsync(id);
            if (airport == null)
                return NotFound("Airport not found");

            _context.Airports.Remove(airport);
            await _context.SaveChangesAsync();

            return Ok(new { message = "🗑️ Airport deleted successfully!" });
        }
    }
}
