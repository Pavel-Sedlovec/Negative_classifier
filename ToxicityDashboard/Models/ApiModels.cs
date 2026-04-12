using System;

namespace ToxicityDashboard.Models
{
    public class LoginRequest
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AdminRegistrationResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ChatInfo
    {
        public long ChatIdTg { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Added_at { get; set; }
    }

    public class MessageStatsInChat
    {
        public int Total { get; set; }
        public int Negative { get; set; }
        public int Positive { get; set; }
        public double NegativePercent { get; set; }
        public double PositivePercent { get; set; }
    }

    public class StatsTopNegativeUsersResponse
    {
        public long UserIdTg { get; set; }
        public int TotalMes { get; set; }
        public int NegativeMes { get; set; }
        public int PositiveMes { get; set; }
        public double NegativePercent { get; set; }
        public double PositivePercent { get; set; }
    }
}