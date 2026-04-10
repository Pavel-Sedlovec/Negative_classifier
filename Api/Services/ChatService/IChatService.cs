using Api.DTOs.Requests;
using Api.Models;

namespace Api.Services.ChatService
{
    public interface IChatService
    {
        public Task<Message> ProcessIncomingMessageAsync(TgMessageRequest req, int label, double confidence);
    }
}
