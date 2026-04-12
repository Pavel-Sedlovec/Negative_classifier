using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTelegram.DTOs.Responses
{
    public class StatsResponse
    {
        public int Total { get; set; }
        public int Negative { get; set; }
        public int Positive { get; set; }
        public double NegativePercent { get; set; }
        public double PositivePercent { get; set; }
    }
}
