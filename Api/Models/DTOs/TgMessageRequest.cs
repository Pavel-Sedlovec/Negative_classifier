
namespace Api.Models.DTOs
{
    public class TgMessageRequest
    {
        public string Text { get; set; }
        public long ChatIdTg { get; set; }
        public string ChatName { get; set; }
        public long UserIdTg { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
    }
}
