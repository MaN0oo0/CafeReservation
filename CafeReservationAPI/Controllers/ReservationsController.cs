using CafeReservationAPI.Constant;
using CafeReservationAPI.Data;
using CafeReservationAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CafeReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyReservations()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var reservations = await _context.Reservations
                .Include(r => r.Table)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return Ok(reservations);
        }

        [HttpPost]
        public async Task<IActionResult> AddReservation(ReservationDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var reservation = new Reservation
            {
                UserId = userId,
                TableId = dto.TableId,
                Date = dto.Date,
                Time = dto.Time,
                Status = ReservationStatus.WAITING
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return Ok(reservation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null || reservation.UserId != userId)
                return NoContent();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [Authorize(Roles = UserRoles.ADMIN)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Table)
                .ToListAsync();

            return Ok(reservations);
        }
        [Authorize(Roles = UserRoles.ADMIN)]
        [HttpPut("confirm/{id}")]
        public async Task<IActionResult> Confirm(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            reservation.Status = ReservationStatus.CONFIRMED;
            await _context.SaveChangesAsync();
            return Ok(reservation);
        }
    }
    public record ReservationDto(int TableId, DateTime Date, string Time);
}
