using Massage.Data;
using Massage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Massage.Controllers;

[ApiController]
[Route("[controller]")]
public class MassageController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MassageController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/massage
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MassageV>>> GetMassages()
    {
        return await _context.Massages.ToListAsync();
    }

    // POST: api/massage
    [HttpPost]
    public async Task<ActionResult<MassageV>> CreateMassage(MassageV massage)
    {
        if (_context.Massages.Any(m => m.Name == massage.Name))
        {
            return Conflict($"Тип массажа '{massage.Name}' уже существует.");
        }

        _context.Massages.Add(massage);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMassageById), new { id = massage.Id }, massage);
    }

    // PUT: api/massage/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMassage(int id, MassageV updatedMassage)
    {
        var existingMassage = await _context.Massages.FindAsync(id);
        if (existingMassage == null)
        {
            return NotFound();
        }

        existingMassage.Name = updatedMassage.Name;
        existingMassage.Description = updatedMassage.Description;
        existingMassage.Price = updatedMassage.Price;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new Exception(ex.Message);
        }

        return NoContent();
    }

    // DELETE: api/massage/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMassage(int id)
    {
        var massageToRemove = await _context.Massages.FindAsync(id);
        if (massageToRemove == null)
        {
            return NotFound();
        }

        _context.Massages.Remove(massageToRemove);
        await _context.SaveChangesAsync();
        return Ok(massageToRemove);
    }

    // GET: api/massage/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<MassageV>> GetMassageById(int id)
    {
        var massage = await _context.Massages.FindAsync(id);
        if (massage == null)
        {
            return NotFound();
        }
        return massage;
    }
}