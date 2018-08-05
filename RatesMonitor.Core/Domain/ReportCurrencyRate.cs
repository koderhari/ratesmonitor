using System;
/*
 * Country|Currency|Amount|Code|Rate
 */

namespace RatesMonitor.Domain
{
    /// <summary>
    /// Year and Month добавил для экспериментов с индексами
    /// </summary>
    public class ReportCurrencyRate
    {
        public DateTime Date { get; set; }
        public string CurrencyCode { get; set; }
  
        public decimal FinalRate { get; set; }
    }
}
