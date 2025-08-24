using Mapster;
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
    public class AirportsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public AirportsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            var airports = await unitOfWork.AirportRepository.GetAsync(includes: a=> a.Include(a=> a.Country));

            var totalCount = airports.Count();
            int pageSize = 6;

            airports = airports
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

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

        [HttpGet("GetOne/{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var airport = await unitOfWork.AirportRepository.GetOneAsync(a => a.Id == id, includes: a => a.Include(a => a.Country));

            if (airport == null)
                return NotFound("Airport not found");

            return Ok(airport);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Airport request)
        {

            var Created = await unitOfWork.AirportRepository.CreateAsync(request);
            if (!Created)
                return BadRequest(new { message = "❌ Failed to create airport!" });

            return Ok(new { message = "✅ Airport created successfully!", airportId = request.Id });
        }

        
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Airport request)
        {
            var airport = await unitOfWork.AirportRepository.GetOneAsync(a=> a.Id == request.Id);
            if (airport == null)
                return NotFound("There is something wrong with that airport!as");

            airport.Name = request.Name;
            airport.City = request.City;
            airport.CountryId = request.CountryId;

            await unitOfWork.AirportRepository.UpdateAsync(airport);

            return Ok(new { message = "✏️ Airport updated successfully!" });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var airport = await unitOfWork.AirportRepository.GetOneAsync(a => a.Id == id);
            if (airport == null)
                return NotFound("Airport not found");  

            await unitOfWork.AirportRepository.DeleteAsync(airport);

            return Ok(new { message = "🗑️ Airport deleted successfully!" });
        }
    }
}
