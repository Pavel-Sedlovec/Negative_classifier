using Api.DTOs.Responses;
using Api.Models;
using Api.Services.StatisticsServise;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : Controller
    {
        private ApplicationDbContext _context;
        private IMessageStats _messageStats;
        private const int NEGATIVE_SENTIMENT = 1;
        private const int POSITIVE_SENTIMENT = 0;

        public StatsController(ApplicationDbContext context, IMessageStats messageStats)
        {
            _context = context;
            _messageStats = messageStats;
        }

        [HttpGet("GeneralChat/{chatId}")]
        public async Task<IActionResult> GeneralChat(long chatId)
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

        [HttpGet("ByDay/{chatId}/{day}")]
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

        [HttpGet("TopNegative/{chatId}/{count}")]
        public async Task<IActionResult> GetTopNegativeUsers(long сhatId, int count)
        {
            List<User> responceUsers2 =
                await _context.Messages.Where(m => m.Chat_id == сhatId && m.Label == 1)
                .GroupBy(m => m.User_id)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Join(_context.Users, g => g.Key, u => u.Id, (g, u) => u)
                .ToListAsync();

            var topUsers =
                await _context.Messages.Where(m => m.Chat_id == сhatId && m.Label == 1)
                .GroupBy(m => m.User_id)
                .Select(g => new
                {
                    UserId = g.Key,
                    Total = g.Count(),
                    Negative = g.Count(m => m.Label == NEGATIVE_SENTIMENT),
                    Positive = g.Count(m => m.Label == POSITIVE_SENTIMENT)
                })
                .OrderByDescending(x => x.UserId)
                .Take(count)
                .ToArrayAsync();

            List<DTOs.Responses.StatsTopNegativeUsersResponse> response = new List<StatsTopNegativeUsersResponse>();

            foreach (var item in topUsers)
            {
                var user = await _context.Users.FindAsync(item.UserId);
                response.Add(new DTOs.Responses.StatsTopNegativeUsersResponse
                {
                    UserIdTg = user.User_id_tg,
                    TotalMes = item.Total,
                    NegativeMes = item.Negative,
                    PositiveMes = item.Positive,
                    NegativePercent = _messageStats.GetPercent(item.Total, item.Negative),
                    PositivePercent = _messageStats.GetPercent(item.Total, item.Positive)
                });
            }
            
            return Ok(response);
        }
    }
}
