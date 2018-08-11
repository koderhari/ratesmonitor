using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RatesMonitor.Core.Domain;
using RatesMonitor.Domain;

namespace RatesMonitor.Core.Infrastructure
{
    public class ReportService : IReportService
    {
        private IDBContextFactory _contextFactory;

        public ReportService(IDBContextFactory contextFactory)
        {
            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));
              _contextFactory = contextFactory;
        }

        public List<WeekRates> GetWeekRatesReport(int year, int month, string[] currencies)
        {
            var rawData = GetRatesFromDb(year,  month,  currencies);
            var result = new List<WeekRates>();
            //в чехии свои гос праздники
            //в базе у нас только рабочие даты
            //неделю можно определять, у нас номер недели меньше чем предыдущий
            //стандартный DayOfWeek с воскресенья начинается
            var weekNum = -1;
            var prevWeekDay = Int32.MaxValue;
            foreach (var item in rawData)
            {
                var day = item.Key;

                if (day.EuroDayOfWeek()< prevWeekDay)
                {
                    weekNum++;
                    var weekRates = new WeekRates();
                    result.Add(weekRates);
                }
                prevWeekDay = day.EuroDayOfWeek();
                var week = result[weekNum];
                week.Days.Add(day.Day);

                foreach (var rate in item)
                {
                    if (!week.CurrencyRates.TryGetValue(rate.CurrencyCode, out var currencyData))
                    {
                        currencyData = new WeekRateData();
                        currencyData.Max = rate.FinalRate;
                        currencyData.Min = rate.FinalRate;
                        week.CurrencyRates[rate.CurrencyCode] = currencyData;
                    }

                    currencyData.Max = rate.FinalRate > currencyData.Max ? rate.FinalRate : currencyData.Max;
                    currencyData.Min = rate.FinalRate < currencyData.Min ? rate.FinalRate : currencyData.Min;
                    currencyData.AddRates(rate.FinalRate);
                }
               
            }
            return result;
        }

        private List<IGrouping<DateTime,ReportCurrencyRate>> GetRatesFromDb(int year, int month, string[] currencies)
        {
            //https://docs.microsoft.com/ru-ru/ef/core/what-is-new/ef-core-2.1
            //печально что в данном случае не поддерживается groupBy
            //to-do переделать в plain и уже псевдо группировать при переборе
            using (var context = _contextFactory.Create())
            {
                return context.DailyCurrencyRates
                    .Where(x => x.Date.Year == year && x.Date.Month == month && currencies.Contains(x.CurrencyCode))
                    .Select(x => new ReportCurrencyRate { CurrencyCode = x.CurrencyCode, Date = x.Date, FinalRate = x.FinalRate })
                    .GroupBy(x => x.Date)
                    .OrderBy(x => x.Key).ToList();
            }
        }
      
    }
}
