using Api.DTOs.Requests;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Message?> ProcessIncomingMessageAsync(TgMessageRequest req, int label, double confidence)
        {
            var user = await GetOrCreateUser(req);

            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.Chat_id_tg == req.ChatIdTg);

            if (chat == null)
            {
                chat = await TryInitChatByAdmin(req);

                if (chat == null) return null;
            }

            var message = new Message
            {
                Text = req.Text,
                Label = label,
                Confidence = confidence,
                Created_at = DateTime.UtcNow,
                Chat_id = chat.Id,
                User_id = user.Id
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return message;
        }

        private async Task<User> GetOrCreateUser(TgMessageRequest req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.User_id_tg == req.UserIdTg);
            if (user == null)
            {
                user = new User
                {
                    User_id_tg = req.UserIdTg,
                    Username = req.Username,
                    First_name = req.FirstName
                };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            return user;
        }

        private async Task<Chat?> TryInitChatByAdmin(TgMessageRequest req)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.TelegramId == req.UserIdTg);

            if (admin == null) return null;

            var newChat = new Chat
            {
                Chat_id_tg = req.ChatIdTg,
                Name = req.ChatName,
                Added_at = DateTime.UtcNow,
                Admin_id = admin.Id
            };

            await _context.Chats.AddAsync(newChat);
            await _context.SaveChangesAsync();

            return newChat;
        }
    }
}
