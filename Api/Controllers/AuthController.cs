using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private ApplicationDbContext _context;
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("SetAdmin")]
        public async Task<IActionResult> SetAdmin([FromBody] long UserIdTg)
        {
            Admin admin = await _context.Admins.FirstOrDefaultAsync(a => a.TelegramId == UserIdTg);
            Random random = new Random();

            if (admin != null)
            {
                return Ok(new DTOs.Responses.AdminRegistrationResponse
                {
                    Message = "Аккаунт уже существует",
                    Login = admin.Login,
                    Password = admin.Password
                });
            }

            var newAdmin = new Admin
            {
                Login = UserIdTg.ToString(),
                Password = random.Next(100, 10000).ToString(),
                TelegramId = UserIdTg
            };

            _context.Admins.Add(newAdmin);
            await _context.SaveChangesAsync();

            return Ok(new DTOs.Responses.AdminRegistrationResponse
            {
                Message = "Аккаунт успешно создан",
                Login = newAdmin.Login,
                Password = newAdmin.Password
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Api.DTOs.Requests.LoginRequest req)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Login == req.Login && a.Password == req.Password);

            if (admin == null)
            {
                return Unauthorized(new { Message = "Неверный логин или пароль" });
            }

            return Ok(new DTOs.Responses.AdminRegistrationResponse
            {
                Message = "Успешный вход",
                Login = admin.Login,
            });
        }
    }
}
