using Api.DTOs.Requests;
using Api.DTOs.Responses;
using Api.Models;
using Api.Services.ChatService;
using Api.Services.ClassifyTextService;
using Api.Services.StatisticsServise;
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
        private IMessageStats _messageStats;
        private IChatService _chatService;

        private ApplicationDbContext _context;

        private const int NEGATIVE_SENTIMENT = 1;
        private const int POSITIVE_SENTIMENT = 0;

        public TelegramController(IClassifyText classifyText, IMessageStats messageStats, IChatService chatService, ApplicationDbContext context)
        {
            _classifyText = classifyText;
            _messageStats = messageStats;
            _context = context;
            _chatService = chatService;
        }

        [HttpPost("Message")]
        public async Task<IActionResult> Message(TgMessageRequest req)
        {
            var (result, confidence) = _classifyText.ClassifyWithConfidence(req.Text);

            var message = await _chatService.ProcessIncomingMessageAsync(req, result, confidence);
            
            return Ok(new DTOs.Responses.ClassifyMessage
            {
                Text = req.Text,
                Label = result,
                Sentiment = result == 1 ? "Negative" : "Positive",
                Confidence = confidence
            });
        }


        [HttpGet("MessageStats/{chatId}")]
        public async Task<IActionResult> MessageStats(long chatId)
        {
            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Chat_id_tg == chatId);

            int mesTotal = 0; int mesPos = 0; int mesNeg = 0;
            double percentPos = 0; double percentNeg = 0;
            if (chat != null)
            {
                mesTotal = await _context.Messages.CountAsync(m => m.Chat_id == chat.Id);
                mesPos = await _context.Messages.CountAsync(m => m.Chat_id == chat.Id && m.Label == POSITIVE_SENTIMENT);
                mesNeg = await _context.Messages.CountAsync(m => m.Chat_id == chat.Id && m.Label == NEGATIVE_SENTIMENT);

                percentPos = _messageStats.GetPercent(mesTotal, mesPos);
                percentNeg = _messageStats.GetPercent(mesTotal, mesNeg);
            }            

            return Ok(new DTOs.Responses.MessageStatsInChat
            {
                Total = mesTotal,
                Positive = mesPos,
                Negative = mesNeg,
                PositivePercent = percentPos,
                NegativePercent = percentNeg
            });
        }

        [HttpGet("MessageStatsForDay/{chatId}/{day}")]
        public async Task<IActionResult> MessageStatsForDay(long chatId, DateTime day)
        {
            var utcDay = DateTime.SpecifyKind(day.Date, DateTimeKind.Utc);

            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Chat_id_tg == chatId);

            int mesTotal = 0; int mesPos = 0; int mesNeg = 0;
            double percentPos = 0; double percentNeg = 0;

            if (chat != null)
            {
                var startOfDay = utcDay;
                var endOfDay = utcDay.AddDays(1);

                var query = _context.Messages.Where(m => m.Chat_id == chat.Id
                                                    && m.Created_at >= startOfDay
                                                    && m.Created_at < endOfDay);

                var stats = await query
                    .GroupBy(m => 1)
                    .Select(g => new
                    {
                        Total = g.Count(),
                        Pos = g.Count(m => m.Label == POSITIVE_SENTIMENT),
                        Neg = g.Count(m => m.Label == NEGATIVE_SENTIMENT)
                    })
                    .FirstOrDefaultAsync();

                if (stats != null)
                {
                    mesTotal = stats.Total;
                    mesPos = stats.Pos;
                    mesNeg = stats.Neg;

                    if (mesTotal > 0)
                    {
                        percentPos = _messageStats.GetPercent(mesTotal, mesPos);
                        percentNeg = _messageStats.GetPercent(mesTotal, mesNeg);
                    }
                }
            }

            return Ok(new DTOs.Responses.MessageStatsInChat
            {
                Total = mesTotal,
                Positive = mesPos,
                Negative = mesNeg,
                PositivePercent = percentPos,
                NegativePercent = percentNeg
            });
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
                .Select(c => new ChatInfo { Name = c.Name, Added_at = c.Added_at})
                .ToListAsync();

            return Ok(responses);
        }


        [HttpPost("SetAdmin")]
        public async Task<IActionResult> SetAdmin([FromBody] long UserIdTg)
        {
            Admin admin = await _context.Admins.FirstOrDefaultAsync(a => a.TelegramId == UserIdTg);
            Random random = new Random();

            if(admin != null)
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
    }    
}
