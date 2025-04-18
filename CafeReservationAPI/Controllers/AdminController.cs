using CafeReservationAPI.Constant;
using CafeReservationAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.ADMIN)]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private  string? controllerName;
        public AdminController(AppDbContext context)
        {
            controllerName = ControllerContext?.RouteData?.Values["Controller"]?.ToString();
            _context = context;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new { u.Id, u.Name, u.Email, u.Role })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("role")]
        public async Task<IActionResult> ChangeUserRole(RoleChangeDto dto)
        {

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return NotFound(new ResponseMessages(404, controllerName).Message);

            if (dto.Role != UserRoles.ADMIN && dto.Role != UserRoles.USER)
                return BadRequest(new ResponseMessages(400, controllerName).Message);

            user.Role = dto.Role;
            await _context.SaveChangesAsync();

            return Ok(new { message = new ResponseMessages(200, controllerName).Message, user.Id, user.Role });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new ResponseMessages(404, controllerName).Message);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new ResponseMessages(200, controllerName).Message);


        }
    }

    public record RoleChangeDto(int UserId, string Role);
}
