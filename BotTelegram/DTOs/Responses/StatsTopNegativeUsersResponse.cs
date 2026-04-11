using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTelegram.DTOs.Responses
{
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
