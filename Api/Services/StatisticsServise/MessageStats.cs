namespace Api.Services.StatisticsServise
{
    public class MessageStats : IMessageStats
    {
        public double GetPercent(int total, int selection)
        {
            if (total == 0) return 0;
            return ((double)selection / total) * 100;
        }
    }
}
