using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTelegram.DTOs.Responses
{
    public class ClassifyResponse
    {
        public string Source { get; set; }
        public int Label { get; set; }
        public string Sentiment { get; set; }
        public double Confidence { get; set; }
    }
}
