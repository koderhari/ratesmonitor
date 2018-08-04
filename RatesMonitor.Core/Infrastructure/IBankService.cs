using RatesMonitor.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RatesMonitor.Core.Infrastructure
{
    public interface IBankService
    {
        Task<List<CurrencyRate>> GetDailyRates(DateTime date);

        Task<List<CurrencyRate>> GetYearRates(int year);
    }
}
