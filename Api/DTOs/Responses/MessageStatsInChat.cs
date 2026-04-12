namespace Api.DTOs.Responses
{
    public class MessageStatsInChat
    {
        public int Total { get; set; }
        public int Negative { get; set; }
        public int Positive { get; set; }
        public double NegativePercent { get; set; }
        public double PositivePercent { get; set; }
    }
}
