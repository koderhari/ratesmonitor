using System;
/*
 * Country|Currency|Amount|Code|Rate
 */

namespace RatesMonitor.Domain
{
    public class CurrencyRate
    {
        public string CurrencyCode { get; set; }
        public decimal OriginalRate { get; set; }
        public decimal Amount { get; set; }
        public decimal FinalRate { get; set; }
        public DateTime Date {get;set;}

        public void CalculateFinalRate()
        {
            FinalRate = OriginalRate / Amount;
        }
    }
}
