using Microsoft.Extensions.Configuration;
using RatesMonitor.Core;
using RatesMonitor.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace TestPrototype
{
    class Program
    {
        private static Timer _timer;
        private static IConfigurationRoot _configuration;

        public static async Task Main(string[] args)
        {
            var dateStr = "02.Jan 2018";
            var dt = DateTime.Parse(dateStr);
            var res = getWorkingDaysByWeek(dt);
            OutputWorkingDays(res);
            var res2 = getWorkingDaysByWeek(new DateTime(2018,2,1));
            OutputWorkingDays(res2);
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            _configuration = builder.Build();
            Console.WriteLine("Hello World!");
            TestTimer();
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
            StopTimer();

        }

        static void OutputWorkingDays(Dictionary<int, List<DateTime>> weeks)
        {
            foreach (var item in weeks)
            {
                Console.WriteLine($"week {item.Key}");
                Console.WriteLine("");
                foreach (var itemValue in item.Value)
                {
                    Console.Write($"{itemValue.ToString("dd")} ");
                }
                Console.WriteLine("");
            }
        }

        //to-do test if any time
        static Dictionary<int,List<DateTime>> getWorkingDaysByWeek(DateTime date)
        {
            var weekNum = 0;
            var result = new Dictionary<int, List<DateTime>>();
            foreach (var day in Enumerable.Range(1, DateTime.DaysInMonth(date.Year, date.Month)).Select(d => new DateTime(date.Year, date.Month,d)))
            {
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday) continue;
                if (!result.TryGetValue(weekNum, out var week))
                {
                    week = new List<DateTime>();
                    result[weekNum] = week;
                }

                if (day.DayOfWeek == DayOfWeek.Monday)
                {
                    weekNum = weekNum == 0 && week.Count == 0 ? weekNum : weekNum + 1;
                }


                if (!result.TryGetValue(weekNum, out week))
                {
                    week = new List<DateTime>();
                    result[weekNum] = week;
                }
                week.Add(day);
            }

            return result;
        }


        static void StopTimer()
        {
            //need check if data write
            _timer?.Stop();
            _timer?.Dispose();
        }

        static void TestTimer()
        {
            //TimeSpan interval;
            if (!TimeSpan.TryParse(_configuration["scheduler_interval"], out var interval))
                interval = TimeSpan.FromMinutes(1);
            _timer = new Timer(interval.TotalMilliseconds);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine(DateTime.Now);
        }

        static async Task TestInsert()
        {
            var newItem = new DailyCurrencyRate();
            newItem.Amount = 10;
            newItem.CurrencyCode = "RUB";
           // newItem.Rate = 1;
            newItem.Date = DateTime.Now;
              
            using (var dbContext = new RatesContext())
            {
                await dbContext.DailyCurrencyRates.AddAsync(newItem);
                await dbContext.SaveChangesAsync();
            }
        }

        static async Task GetRates()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://www.cnb.cz");
            var dt = DateTime.Now;
            var dailyResponse =  await client.GetAsync($"/en/financial_markets/foreign_exchange_market/exchange_rate_fixing/daily.txt?date={dt.ToString("dd.MM.yyyy")}");
            //byte by byte
            if (dailyResponse.IsSuccessStatusCode)
            {

                using (var stream = await dailyResponse.Content.ReadAsStreamAsync())
                {
                    using (var readStream = new StreamReader(stream))
                    {
                        int lineNumber = 0;
                        while (!readStream.EndOfStream)
                        {
                            var line = await readStream.ReadLineAsync();
                            if (++lineNumber < 3) continue;
                            //maps by dictionary
                            Console.WriteLine(line);

                        }
                    }
                }

            }
            //var yearResponse  = await client.GetAsync($"/year.txt?date={dt.ToString("yyyy")}");
        }
    }
}
