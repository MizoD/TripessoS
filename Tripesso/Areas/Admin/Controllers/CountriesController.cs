using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Tripesso.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public CountriesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            var countries = await unitOfWork.CountryRepository.GetAsync(includes: c=> c.Include(c=> c.Trips).Include(c=> c.Airports));

            var totalCount = countries.Count();
            int pageSize = 6;

            countries = countries
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

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

        [HttpGet("GetOne/{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var country = await unitOfWork.CountryRepository.GetOneAsync(c=> c.Id == id);

            if (country == null)
                return NotFound("Country not found");

            return Ok(country);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Country request)
        {
            var Created = await unitOfWork.CountryRepository.CreateAsync(request);
            if (!Created)
                return BadRequest("Failed to create country");

            return Ok(new { message = "✅ Country created successfully!", countryId = request.Id });
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Country request)
        {
            var country = await unitOfWork.CountryRepository.GetOneAsync(c=> c.Id == request.Id);
            if (country is null)
                return NotFound("Country not found");

            country.Name = request.Name;
            country.Code = request.Code;
            country.Currency = request.Currency;

            var updated = await unitOfWork.CountryRepository.UpdateAsync(country);
            if (!updated)
                return BadRequest("Failed to update country");

            return Ok(new { message = "✏️ Country updated successfully!" });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var country = await unitOfWork.CountryRepository.GetOneAsync(c=> c.Id == id);
            if (country == null)
                return NotFound("Country not found");

            var deleted =await unitOfWork.CountryRepository.DeleteAsync(country);
            if (!deleted)
                return BadRequest("Failed to delete country");

            return Ok(new { message = "🗑️ Country deleted successfully!" });
        }
    }
}
