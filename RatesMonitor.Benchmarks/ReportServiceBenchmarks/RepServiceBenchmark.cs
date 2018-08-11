using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using Microsoft.EntityFrameworkCore;
using RatesMonitor.Core.Domain;
using RatesMonitor.Core.Infrastructure;
using RatesMonitor.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RatesMonitor.Benchmarks.ReportServiceBenchmarks
{
    [RankColumn]
    [Config(typeof(Config))]
    public class RepServiceBenchmark
    {
      
        List<IGrouping<DateTime, ReportCurrencyRate>> data;
        public RepServiceBenchmark()
        {

        }

        [GlobalSetup]
        public void SetupData()
        {
            var connectionString = "Server=localhost\\SQLExpress;Database=ratesdb;Trusted_Connection=True;";
            var year = 2018;
            var month = 2;
            var rawCurrencies = "1 AUD|1 BGN|1 BRL|1 CAD|1 CHF|1 CNY|1 DKK|1 EUR|1 GBP|1 HKD|1 HRK|100 HUF|1000 IDR|1 ILS|100 INR|100 ISK|100 JPY|100 KRW|1 MXN|1 MYR|1 NOK|1 NZD|100 PHP|1 PLN|1 RON|100 RUB|1 SEK|1 SGD|100 THB|1 TRY|1 USD|1 XDR|1 ZAR";

            var currencies = rawCurrencies.Split("|").Select(x => x.Split(" ")[1]).ToArray();// new string[] { "USD", "RUB", "AUD", "BGN", "CAD", "CHF" , "CNY" , "GBP", "HKD", "INR", "ISK" };
            //_reportService = new ReportService(new DBContextFactory());
            var optionsBuilder = new DbContextOptionsBuilder<RatesContext>();
            optionsBuilder.UseSqlServer(connectionString);
            using (var context = new RatesContext(optionsBuilder.Options))
            {
                data = context.DailyCurrencyRates
                    .Where(x => x.Date.Year == year && x.Date.Month == month && currencies.Contains(x.CurrencyCode))
                    .Select(x => new ReportCurrencyRate { CurrencyCode = x.CurrencyCode, Date = x.Date, FinalRate = x.FinalRate })
                    .GroupBy(x => x.Date)
                    .OrderBy(x => x.Key).ToList();
            }
        }

        public void Clear()
        { }

        [Benchmark]
        public void TestVesion1()
        {
            var rawData = data;
            var result = new List<WeekRates>();
            var weekNum = -1;
            var prevWeekDay = Int32.MaxValue;
            foreach (var item in rawData)
            {
                var day = item.Key;

                if (day.EuroDayOfWeek() < prevWeekDay)
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
                    //currencyData.ad
                    currencyData.AddRates(rate.FinalRate);
                    //currencyData.RatesValues.Add(rate.FinalRate);
                }

            }
        }

        [Benchmark]
        public void TestVesionWithStruct()
        {
            var rawData = data;
            var result = new List<WeekRatesStruct>();
            var weekNum = -1;
            var prevWeekDay = Int32.MaxValue;
            foreach (var item in rawData)
            {
                var day = item.Key;

                if (day.EuroDayOfWeek() < prevWeekDay)
                {
                    weekNum++;
                    var weekRates = new WeekRatesStruct();
                    result.Add(weekRates);
                }
                prevWeekDay = day.EuroDayOfWeek();
                var week = result[weekNum];
                week.Days.Add(day.Day);

                foreach (var rate in item)
                {
                    if (!week.CurrencyRates.TryGetValue(rate.CurrencyCode, out var currencyData))
                    {
                        currencyData = new WeekRateDataStruct();
                        currencyData.Max = rate.FinalRate;
                        currencyData.Min = rate.FinalRate;
                       // currencyData.RatesValues = new List<decimal>(7);
                        week.CurrencyRates[rate.CurrencyCode] = currencyData;
                        
                    }

                    currencyData.Max = rate.FinalRate > currencyData.Max ? rate.FinalRate : currencyData.Max;
                    currencyData.Min = rate.FinalRate < currencyData.Min ? rate.FinalRate : currencyData.Min;
                    //currencyData.AddRatesValue();//rate.FinalRate
                   // currencyData.RatesValues.Add(rate.FinalRate);
                }

            }
        }
    }


    public class Config : ManualConfig
    {
        public Config()
        {
            Add(MemoryDiagnoser.Default);
            //Add(new ORMColum());
            //Add(new ReturnColum());
            //Add(Job.Default
            //    .WithUnrollFactor(BenchmarkBase.Iterations)
            //    //.WithIterationTime(new TimeInterval(500, TimeUnit.Millisecond))
            //    .WithLaunchCount(1)
            //    .WithWarmupCount(0)
            //    .WithTargetCount(5)
            //    .WithRemoveOutliers(true)
            //);
        }
    }

    public struct WeekRateDataStruct
    {
       

        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Median
        {
            get
            {
                RatesValues.Sort();
                if (RatesValues.Count % 2 == 1) return RatesValues[RatesValues.Count / 2];
                return (RatesValues[RatesValues.Count / 2] + RatesValues[RatesValues.Count / 2 - 1]) / 2;
            }

        }

        public void AddRatesValue()
        {
            //if (RatesValues == null)
            //  RatesValues = new List<decimal>(7);
            // RatesValues.Add(val);
            //var t = RatesValues.Count;
        }

        public List<decimal> RatesValues { get; set; }


    }

    public class WeekRatesStruct
    {
        public List<int> Days { get; set; } = new List<int>();
        public Dictionary<string, WeekRateDataStruct> CurrencyRates { get; set; } = new Dictionary<string, WeekRateDataStruct>();
    }
}
