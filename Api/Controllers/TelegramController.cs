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

        private IChatService _chatService;

        private ApplicationDbContext _context;

        private const int NEGATIVE_SENTIMENT = 1;
        private const int POSITIVE_SENTIMENT = 0;

        public TelegramController(IClassifyText classifyText, IChatService chatService, ApplicationDbContext context)
        {
            _classifyText = classifyText;
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
    }
}
