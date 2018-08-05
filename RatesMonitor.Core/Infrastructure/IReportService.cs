using RatesMonitor.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace RatesMonitor.Core.Infrastructure
{
    public interface IReportService
    {
        List<WeekRates> GetWeekRatesReport(int year, int month, string[] currencies);
    }
}
