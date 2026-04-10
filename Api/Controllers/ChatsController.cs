using Api.DTOs.Responses;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatsController : Controller
    {
        private ApplicationDbContext _context;

        public ChatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetChats/{adminLogin}")]
        public async Task<IActionResult> GetChats(string adminLogin)
        {
            Admin admin = await _context.Admins.FirstOrDefaultAsync(a => a.Login == adminLogin);

            if (admin == null)
            {
                return NotFound($"Админ с логином {adminLogin} не найден");
            }

            List<ChatInfo> responses = await _context.Chats.Where(c => c.Admin_id == admin.Id)
                .Select(c => new ChatInfo { Name = c.Name, Added_at = c.Added_at })
                .ToListAsync();

            return Ok(responses);
        }
    }
}
