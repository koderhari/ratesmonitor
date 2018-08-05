using System;
using System.Collections.Generic;
using System.Text;

namespace RatesMonitor.Core.Domain
{
    //USD - max: , min: , median: 
    public class WeekRateData
    {
        public string Currency { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Median { get; set; } //add auto calc prop
        public List<decimal> Values { get; set; } = new List<decimal>();
    }
}
