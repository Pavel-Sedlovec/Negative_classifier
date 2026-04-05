namespace Api.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public long Chat_id_tg { get; set; }
        public string Name { get; set; }       
        public DateTime Added_at { get; set; }

        public List<Message> Messages { get; set; } = new();
    }
}
