using Massage.Data;
using Massage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Massage.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings.Include(b => b.TimeSlot).Include(b => b.SelectedMassage).ToListAsync();
        }

        [HttpGet("SelectTime")]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetAvailableTimeSlots()
        {
            var availableTimeSlots = await _context.TimeSlots
                .Where(ts => !ts.IsBooked)
                .ToListAsync();
            return Ok(availableTimeSlots);
        }


        // POST: api/booking
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(Booking booking)
        {
            if (_context.Bookings.Any(b => b.TimeSlot.Id == booking.TimeSlot.Id && b.SelectedMassage.Id == booking.SelectedMassage.Id))
            {
                return Conflict($"Бронирование на {booking.TimeSlot.StartDateTime} для массажа '{booking.SelectedMassage.Name}' уже существует.");
            }
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, booking);
        }
        // PUT: api/booking/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, Booking updatedBooking)
        {
            var existingBooking = await _context.Bookings.Include(b => b.TimeSlot).Include(b => b.SelectedMassage).FirstOrDefaultAsync(b => b.Id == id);
            if (existingBooking == null)
            {
                return NotFound();
            }
            existingBooking.ClientName = updatedBooking.ClientName;
            existingBooking.PhoneNumber = updatedBooking.PhoneNumber;
            existingBooking.Age = updatedBooking.Age;
            existingBooking.Weight = updatedBooking.Weight;
            existingBooking.Notes = updatedBooking.Notes;
            existingBooking.BookingTime = updatedBooking.BookingTime;
            existingBooking.TimeSlot = updatedBooking.TimeSlot;
            existingBooking.SelectedMassage = updatedBooking.SelectedMassage;
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
        // DELETE: api/booking/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        // GET: api/booking/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            var booking = await _context.Bookings.Include(b => b.TimeSlot).Include(b => b.SelectedMassage).FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            return booking;
        }
        // GET: api/booking/timeslot/{timeslotId}
        [HttpGet("timeslot/{timeslotId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByTimeSlot(int timeslotId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.TimeSlot.Id == timeslotId)
                .ToListAsync();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для временного слота с ID {timeslotId}.");
            }
            return bookings;
        }
        // GET: api/booking/massage/{massageId}
        [HttpGet("massage/{massageId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByMassage(int massageId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.SelectedMassage.Id == massageId)
                .ToListAsync();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для массажа с ID {massageId}.");
            }
            return bookings;
        }
        // GET: api/booking/client/{clientName}
        [HttpGet("client/{clientName}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByClient(string clientName)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.ClientName.Contains(clientName, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для клиента с именем '{clientName}'.");
            }
            return bookings;
        }
        // GET: api/booking/date/{date}
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByDate(DateTime date)
        {
            var bookings = await _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.BookingTime.Date == date.Date)
                .ToListAsync();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований на дату {date.ToShortDateString()}.");
            }
            return bookings;
        }
        // GET: api/booking/phone/{phoneNumber}
        [HttpGet("phone/{phoneNumber}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByPhoneNumber(string phoneNumber)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.PhoneNumber == phoneNumber)
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований с номером телефона '{phoneNumber}'.");
            }
            return bookings;
        }
        // GET: api/booking/age/{age}
        [HttpGet("age/{age}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByAge(int age)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.Age == age)
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для возраста {age}.");
            }
            return bookings;
        }
        // GET: api/booking/weight/{weight}
        [HttpGet("weight/{weight}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByWeight(float weight)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.Weight == weight)
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для веса {weight}.");
            }
            return bookings;
        }
        // GET: api/booking/notes/{notes}
        [HttpGet("notes/{notes}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByNotes(string notes)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.Notes.Contains(notes, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований с заметками, содержащими '{notes}'.");
            }
            return bookings;
        }
        // GET: api/booking/timeslot/{timeslotId}/massage/{massageId}
        [HttpGet("timeslot/{timeslotId}/massage/{massageId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByTimeSlotAndMassage(int timeslotId, int massageId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.TimeSlot.Id == timeslotId && b.SelectedMassage.Id == massageId)
                .ToListAsync();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для временного слота с ID {timeslotId} и массажа с ID {massageId}.");
            }
            return bookings;
        }
        // GET: api/booking/timeslot/{timeslotId}/client/{clientName}
        [HttpGet("timeslot/{timeslotId}/client/{clientName}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByTimeSlotAndClient(int timeslotId, string clientName)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.TimeSlot.Id == timeslotId && b.ClientName.Contains(clientName, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для временного слота с ID {timeslotId} и клиента с именем '{clientName}'.");
            }
            return bookings;
        }
        // GET: api/booking/timeslot/{timeslotId}/date/{date}
        [HttpGet("timeslot/{timeslotId}/date/{date}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByTimeSlotAndDate(int timeslotId, DateTime date)
        {
            var bookings = await _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.TimeSlot.Id == timeslotId && b.BookingTime.Date == date.Date)
                .ToListAsync();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для временного слота с ID {timeslotId} на дату {date.ToShortDateString()}.");
            }
            return bookings;
        }
        // GET: api/booking/timeslot/{timeslotId}/phone/{phoneNumber}
        [HttpGet("timeslot/{timeslotId}/phone/{phoneNumber}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByTimeSlotAndPhoneNumber(int timeslotId, string phoneNumber)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.TimeSlot.Id == timeslotId && b.PhoneNumber == phoneNumber)
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для временного слота с ID {timeslotId} и номера телефона '{phoneNumber}'.");
            }
            return bookings;
        }
        // GET: api/booking/timeslot/{timeslotId}/age/{age}
        [HttpGet("timeslot/{timeslotId}/age/{age}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByTimeSlotAndAge(int timeslotId, int age)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.TimeSlot.Id == timeslotId && b.Age == age)
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для временного слота с ID {timeslotId} и возраста {age}.");
            }
            return bookings;
        }
        // GET: api/booking/timeslot/{timeslotId}/weight/{weight}
        [HttpGet("timeslot/{timeslotId}/weight/{weight}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByTimeSlotAndWeight(int timeslotId, float weight)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.TimeSlot.Id == timeslotId && b.Weight == weight)
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для временного слота с ID {timeslotId} и веса {weight}.");
            }
            return bookings;
        }
        // GET: api/booking/timeslot/{timeslotId}/notes/{notes}
        [HttpGet("timeslot/{timeslotId}/notes/{notes}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByTimeSlotAndNotes(int timeslotId, string notes)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.TimeSlot.Id == timeslotId && b.Notes.Contains(notes, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для временного слота с ID {timeslotId} и заметок, содержащих '{notes}'.");
            }
            return bookings;
        }
        // GET: api/booking/massage/{massageId}/client/{clientName}
        [HttpGet("massage/{massageId}/client/{clientName}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByMassageAndClient(int massageId, string clientName)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.SelectedMassage.Id == massageId && b.ClientName.Contains(clientName, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для массажа с ID {massageId} и клиента с именем '{clientName}'.");
            }
            return bookings;
        }
        // GET: api/booking/massage/{massageId}/date/{date}
        [HttpGet("massage/{massageId}/date/{date}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByMassageAndDate(int massageId, DateTime date)
        {
            var bookings = await _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.SelectedMassage.Id == massageId && b.BookingTime.Date == date.Date)
                .ToListAsync();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для массажа с ID {massageId} на дату {date.ToShortDateString()}.");
            }
            return bookings;
        }
        // GET: api/booking/massage/{massageId}/phone/{phoneNumber}
        [HttpGet("massage/{massageId}/phone/{phoneNumber}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByMassageAndPhoneNumber(int massageId, string phoneNumber)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.SelectedMassage.Id == massageId && b.PhoneNumber == phoneNumber)
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для массажа с ID {massageId} и номера телефона '{phoneNumber}'.");
            }
            return bookings;
        }
        // GET: api/booking/massage/{massageId}/age/{age}
        [HttpGet("massage/{massageId}/age/{age}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByMassageAndAge(int massageId, int age)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.SelectedMassage.Id == massageId && b.Age == age)
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для массажа с ID {massageId} и возраста {age}.");
            }
            return bookings;
        }
        // GET: api/booking/massage/{massageId}/weight/{weight}
        [HttpGet("massage/{massageId}/weight/{weight}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByMassageAndWeight(int massageId, float weight)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.SelectedMassage.Id == massageId && b.Weight == weight)
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для массажа с ID {massageId} и веса {weight}.");
            }
            return bookings;
        }
        // GET: api/booking/massage/{massageId}/notes/{notes}
        [HttpGet("massage/{massageId}/notes/{notes}")]
        public ActionResult<IEnumerable<Booking>> GetBookingsByMassageAndNotes(int massageId, string notes)
        {
            var bookings = _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.SelectedMassage)
                .Where(b => b.SelectedMassage.Id == massageId && b.Notes.Contains(notes, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (bookings == null || !bookings.Any())
            {
                return NotFound($"Нет бронирований для массажа с ID {massageId} и заметок, содержащих '{notes}'.");
            }
            return bookings;
        }
    }
}