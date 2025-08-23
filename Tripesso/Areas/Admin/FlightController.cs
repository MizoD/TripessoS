using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repositories.IRepositories;
using Models;

namespace Tripesso.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Route("admin/[controller]")]
    public class FlightsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public FlightsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: admin/flights
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var flights = await _unitOfWork.FlightRepository.GetAllFlightsAsync();
            return View(flights);
        }

        // GET: admin/flights/details/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var flight = await _unitOfWork.FlightRepository.GetFlightDetailsAsync(id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        // GET: admin/flights/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: admin/flights/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Flight flight)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.FlightRepository.AddAsync(flight);
                await _unitOfWork.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flight);
        }

        // GET: admin/flights/edit/5
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var flight = await _unitOfWork.FlightRepository.GetByIdAsync(id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        // POST: admin/flights/edit/5
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Flight flight)
        {
            if (id != flight.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _unitOfWork.FlightRepository.UpdateAsync(flight);
                await _unitOfWork.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flight);
        }

        // GET: admin/flights/delete/5
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var flight = await _unitOfWork.FlightRepository.GetFlightDetailsAsync(id);
            if (flight == null) return NotFound();
            return View(flight);
        }

        // POST: admin/flights/delete/5
        [HttpPost("delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _unitOfWork.FlightRepository.GetByIdAsync(id);
            if (flight == null) return NotFound();

            _unitOfWork.FlightRepository.DeleteAsync(flight);
            await _unitOfWork.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}