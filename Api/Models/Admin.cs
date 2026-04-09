using System.ComponentModel.DataAnnotations.Schema;
namespace Api.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public long TelegramId {  get; set; }

        public List<Chat> Chats { get; set; } = new();
    }
}
