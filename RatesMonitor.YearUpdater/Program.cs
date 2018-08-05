using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RatesMonitor.Core;
using RatesMonitor.Core.Infrastructure;
using RatesMonitor.YearUpdater.Infrastructure;
using System;
using System.IO;

namespace RatesMonitor.YearUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IDBContextFactory>((sp) =>
            new DBContextFactory(configuration.GetConnectionString("RatesDB")))
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
                Console.WriteLine("Start loading");
                scheduleTask.LoadData(year);
                Console.WriteLine("Loading finish");
            }
        }
    }
}
