using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RatesMonitor.Core;
using RatesMonitor.Core.Infrastructure;
using RatesMonitor.YearUpdater.Infrastructure;
using System;

namespace RatesMonitor.YearUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IDBContextFactory, DBContextFactory>()
            .AddSingleton<IBankService, BankService>()
            .AddSingleton<IRatesBulkLoader, RatesBulkLoader>()
            .BuildServiceProvider();
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);
            var scheduleTask = serviceProvider.GetService<IRatesBulkLoader>();
            while (true)
            {
                Console.WriteLine("Please enter year and press enter (or q for exit)");
                var line = Console.ReadLine();
                if (line.Trim().ToLowerInvariant() == "q")
                    break;
                //to-do add more checks for years
                if (!int.TryParse(line, out var year))
                {
                    Console.WriteLine("Invalid year, try again");
                    continue;
                }
                scheduleTask.LoadData(year);
            }
        }
    }
}
