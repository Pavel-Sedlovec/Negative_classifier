using Api.DTOs.Requests;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using static System.Net.Mime.MediaTypeNames;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramController : Controller
    {
        private IClassifyText _classifyText;
        private ApplicationDbContext _context;

        private const int NEGATIVE_SENTIMENT = 1;
        private const int POSITIVE_SENTIMENT = 0;

        public TelegramController(IClassifyText classifyText, ApplicationDbContext context)
        {
            _classifyText = classifyText;
            _context = context;
        }

        [HttpPost("Message")]
        public async Task<IActionResult> Message(TgMessageRequest req)
        {
            var (result, confidence) = _classifyText.ClassifyWithConfidence(req.Text);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.User_id_tg == req.UserIdTg);
            
            if(user == null)
            {
                user = new Models.User { User_id_tg = req.UserIdTg, Username = req.Username, First_name = req.FirstName };
                await _context.Users.AddRangeAsync(user);
                await _context.SaveChangesAsync();
            }

            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Chat_id_tg == req.ChatIdTg);

            if (chat == null)
            {
                chat = new Models.Chat { Chat_id_tg = req.ChatIdTg, Name = req.ChatName, Added_at = DateTime.UtcNow };
                await _context.Chats.AddRangeAsync(chat);
                await _context.SaveChangesAsync();
            }

            var message = new Message
            {
                Text = req.Text,
                Label = result,
                Confidence = confidence,
                Created_at = DateTime.UtcNow,
                Chat_id = chat.Id,
                User_id = user.Id
            };
            await _context.Messages.AddRangeAsync(message);
            await _context.SaveChangesAsync();
            

            return Ok(new DTOs.Responses.ClassifyMessage
            {
                Text = req.Text,
                Label = result,
                Sentiment = result == 1 ? "Negative" : "Positive",
                Confidence = confidence
            });
        }


        [HttpPost("Message")]
        public async Task<IActionResult> StatsMessages(int chatId)
        {
            int totalMess = _context.Messages.Count(m => m.Chat_id == chatId);
            int posMes = _context.Messages.Count(m => m.Chat_id == chatId && m.Label == POSITIVE_SENTIMENT);
            int negMes = _context.Messages.Count(m => m.Chat_id == chatId && m.Label == NEGATIVE_SENTIMENT);
            return Ok();
        }
    }
    
}
