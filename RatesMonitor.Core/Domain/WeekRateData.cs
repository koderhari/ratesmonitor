using System;
using System.Collections.Generic;
using System.Text;

namespace RatesMonitor.Core.Domain
{
    //USD - max: , min: , median: 
    //to-do можно поменять на struct, вынести отсюда RatesValues на уровень выше, по идее меньше аллокаций будет
    //to-do или ratesvalues оставить, но ограничить к ним доступ
    public class WeekRateData
    {


        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Median
        {
            get
            {
                RatesValues.Sort();
                if (RatesValues.Count % 2 == 1) return RatesValues[RatesValues.Count / 2 ];
                return (RatesValues[RatesValues.Count / 2] + RatesValues[RatesValues.Count / 2 - 1]) / 2;
            }
            
        }

        public List<decimal> RatesValues { get; set; } = new List<decimal>();

       
    }
}
