using System.Data;

namespace Api.Models
{
    public class Mesage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Label {  get; set; } 
        public string Confidence { get; set; }
        public DateTime Created_at {  get; set; }
        public int Chat_id {  get; set; }
        public int User_id { get; set; }
    }
}
