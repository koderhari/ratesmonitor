using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RatesMonitor.Core;
using RatesMonitor.Core.Infrastructure;
using RatesMonitor.DailyUpdater.Infrastructure;
using System;
using System.IO;


namespace RatesMonitor.DailyUpdater
{
    class Program
    {
        private static IConfigurationRoot _configuration;
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            _configuration = builder.Build();
            var settings = new Settings();
            if (!TimeSpan.TryParse(_configuration["scheduler_interval"], out var interval))
                interval = TimeSpan.FromMinutes(1);
            settings.Interval = interval;
            var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IDBContextFactory, DBContextFactory>()
            .AddSingleton<IBankService,BankService>()
            .AddSingleton(settings)
            .AddSingleton<IRatesUpdateScheduleTask, RatesUpdateScheduleTask>()
            .BuildServiceProvider();
            //configure console logging
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);
            Console.WriteLine("Rates daily updater");
            Console.WriteLine("Press Enter for exit");
            
            using (var scheduleTask = serviceProvider.GetService<IRatesUpdateScheduleTask>())
            {
                scheduleTask.Start();
                while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                scheduleTask.Stop();
            }
        }

      
    }
}
