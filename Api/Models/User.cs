namespace Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public long User_id_tg { get; set; }
        public string Username { get; set; }
        public string First_name { get; set; }

        public List<Message> Messages { get; set; } = new();
    }
}
