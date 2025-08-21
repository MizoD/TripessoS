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
    public class CountriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: admin/countries?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var query = _context.Countries;

            var totalCount = await query.CountAsync();

            var countries = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Code,
                    c.Currency
                })
                .ToListAsync();

            return Ok(new
            {
                Data = countries,
                Pagination = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }

        // GET: admin/countries/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);

            if (country == null)
                return NotFound("Country not found");

            return Ok(new
            {
                country.Id,
                country.Name,
                country.Code,
                country.Currency
            });
        }

        // POST: admin/countries
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Country request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Countries.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Country created successfully!", countryId = request.Id });
        }

        // PUT: admin/countries/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Country request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
                return NotFound("Country not found");

            country.Name = request.Name;
            country.Code = request.Code;
            country.Currency = request.Currency;

            _context.Countries.Update(country);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✏️ Country updated successfully!" });
        }

        // DELETE: admin/countries/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
                return NotFound("Country not found");

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return Ok(new { message = "🗑️ Country deleted successfully!" });
        }
    }
}
