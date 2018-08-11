using System;
using System.Collections.Generic;
using System.Text;

namespace RatesMonitor.Core.Domain
{
    //USD - max: , min: , median: 
    public class WeekRateData
    {
        private decimal? _median;

        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Median
        {
            get
            {
                if (!_median.HasValue)
                {
                    RatesValues.Sort();
                    if (RatesValues.Count % 2 == 1) return RatesValues[RatesValues.Count / 2];
                    _median = (RatesValues[RatesValues.Count / 2] + RatesValues[RatesValues.Count / 2 - 1]) / 2;
                }

                return _median.Value;

            }
            
        }

        private List<decimal> RatesValues { get; set; } = new List<decimal>(5);

       
        public void AddRates(decimal rate)
        {
            _median = null;
            RatesValues.Add(rate);
        }
    }
}
