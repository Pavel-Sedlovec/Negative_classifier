using System.ComponentModel.DataAnnotations.Schema;
namespace Api.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public long Chat_id_tg { get; set; }
        public string Name { get; set; }
        public DateTime Added_at { get; set; }


        [ForeignKey("Admin")]
        public int Admin_id {  get; set; }
        public Admin Admin { get; set; }

        public List<Message> Messages { get; set; } = new();
    }
}
