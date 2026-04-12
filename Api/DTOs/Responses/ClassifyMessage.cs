namespace Api.DTOs.Responses
{
    public class ClassifyMessage
    {
        public string Text { get; set; }
        public int Label{ get; set; }
        public string Sentiment { get; set; }
        public double Confidence { get; set; }
    }
}
