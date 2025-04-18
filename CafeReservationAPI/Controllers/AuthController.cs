using CafeReservationAPI.Constant;
using CafeReservationAPI.Data;
using CafeReservationAPI.Models;
using CafeReservationAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CafeReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private string? controllerName;
        public AuthController(AppDbContext context, TokenService tokenService)
        {
            controllerName = ControllerContext?.RouteData?.Values["Controller"]?.ToString();
            _context = context;
            _tokenService = tokenService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new ResponseMessages(400, controllerName).Message);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Role = UserRoles.USER
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || user.PasswordHash != HashPassword(request.Password))
                return Unauthorized(new ResponseMessages(401, controllerName).Message);

            var token = _tokenService.CreateToken(user);
            return Ok(new
            {
                token,
                user = new
                {
                    name = user.Name,
                    email = user.Email,
                    role = user.Role

                }
            });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
    public record UserDto(string Name, string Email, string Password);
    public record LoginDto(string Email, string Password);
}
