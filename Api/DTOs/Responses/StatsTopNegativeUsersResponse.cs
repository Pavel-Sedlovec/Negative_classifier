namespace Api.DTOs.Responses
{
    public class StatsTopNegativeUsersResponse
    {
        public long UserIdTg {  get; set; }
        public int TotalMes { get; set; }
        public int NegativeMes{ get; set; }
        public int PositiveMes {  get; set; }
        public double NegativePercent { get; set; }
        public double PositivePercent { get; set; }
    }
}
