using RatesMonitor.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RatesMonitor.Core.Infrastructure
{
    public interface IBankService
    {
        Task<List<DailyCurrencyRate>> GetDailyRates(DateTime date);

        Task<List<DailyCurrencyRate>> GetYearRates(int year);

        Task<List<DailyCurrencyRate>> GetYearRatesByDay(int year, Action<IEnumerable<DailyCurrencyRate>> proccessDay);
    }
}
