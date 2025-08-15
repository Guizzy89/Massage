using Microsoft.AspNetCore.Mvc;
using Massage.Data;
using Massage.Models;
using Microsoft.EntityFrameworkCore;

namespace Massage.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class TimeSlotController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TimeSlotController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/timeslot
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlots()
        {
            return await _context.TimeSlots.ToListAsync();
        }
        // POST: api/timeslot
        [HttpPost]
        public async Task<ActionResult<TimeSlot>> CreateTimeSlot(TimeSlot timeSlot)
        {
            if (_context.TimeSlots.Any(ts => ts.StartDateTime == timeSlot.StartDateTime))
            {
                return Conflict($"Временной слот на {timeSlot.StartDateTime} уже существует.");
            }
            _context.TimeSlots.Add(timeSlot);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTimeSlots), new { id = timeSlot.Id }, timeSlot);
        }
        // PUT: api/timeslot/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeSlot(int id, TimeSlot updatedTimeSlot)
        {
            var existingTimeSlot = await _context.TimeSlots.FindAsync(id);
            if (existingTimeSlot == null)
            {
                return NotFound();
            }
            existingTimeSlot.StartDateTime = updatedTimeSlot.StartDateTime;
            existingTimeSlot.Duration = updatedTimeSlot.Duration;
            existingTimeSlot.IsBooked = updatedTimeSlot.IsBooked;
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
        // DELETE: api/timeslot/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeSlot(int id)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot == null)
            {
                return NotFound();
            }
            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        // GET: api/timeslot/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TimeSlot>> GetTimeSlotById(int id)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot == null)
            {
                return NotFound();
            }
            return timeSlot;
        }
    }

}
