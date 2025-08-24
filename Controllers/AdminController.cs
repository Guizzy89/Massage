using Massage.Data;
using Massage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Massage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ManageServices
        [HttpGet("ManageServices")]
        public async Task<IActionResult> ManageServices()
        {
            var services = await _context.Massages.ToListAsync();
            return View(services);
        }

        // GET: Admin/CreateService
        [HttpGet("CreateService")]
        public IActionResult CreateService()
        {
            return View();
        }

        // POST: Admin/CreateService
        [HttpPost("CreateService")]
        public async Task<IActionResult> CreateService(MassageV massage)
        {
            if (ModelState.IsValid)
            {
                _context.Massages.Add(massage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageServices));
            }
            return View(massage);
        }

        // GET: Admin/EditService/{id}
        [HttpGet("EditService/{id}")]
        public async Task<IActionResult> EditService(int id)
        {
            var massage = await _context.Massages.FindAsync(id);
            if (massage == null)
            {
                return NotFound();
            }
            return View(massage);
        }

        // POST: Admin/EditService/{id}
        [HttpPost("EditService/{id}")]
        public async Task<IActionResult> EditService(int id, MassageV massage)
        {
            if (id != massage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(massage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MassageExists(massage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageServices));
            }
            return View(massage);
        }

        // GET: Admin/DeleteService/{id}
        [HttpGet("DeleteService/{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var massage = await _context.Massages.FindAsync(id);
            if (massage == null)
            {
                return NotFound();
            }
            return View(massage);
        }

        // POST: Admin/DeleteService/{id}
        [HttpPost("DeleteService/{id}")]
        public async Task<IActionResult> DeleteServiceConfirmed(int id)
        {
            var massage = await _context.Massages.FindAsync(id);
            if (massage != null)
            {
                _context.Massages.Remove(massage);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageServices));
        }

        private bool MassageExists(int id)
        {
            return _context.Massages.Any(e => e.Id == id);
        }
    }
}