using System.ComponentModel.DataAnnotations.Schema;
namespace Api.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Label {  get; set; } 
        public int Confidence { get; set; }
        public DateTime Created_at {  get; set; }

        [ForeignKey("Chat")]
        public int Chat_id {  get; set; }
        public Chat Chat { get; set; }


        [ForeignKey("User")]
        public int User_id { get; set; }
        public User User { get; set; }
    }
}
