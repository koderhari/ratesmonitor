using System;
using System.Collections.Generic;
using System.Text;

namespace RatesMonitor.Core.Domain
{

    //1...2: USD - max: , min: , median: ; EUR - max: , min: , media: ;

    public class WeekRates
    {
        public List<int> Days { get; set; } = new List<int>();
        public Dictionary<string, WeekRateData> CurrencyRates { get; set; } = new Dictionary<string, WeekRateData>();
    }
}
