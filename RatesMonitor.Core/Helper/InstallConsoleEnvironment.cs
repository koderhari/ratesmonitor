using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RatesMonitor.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RatesMonitor.Core.Helper
{
    public static class InstallConsoleEnvironment
    {
        public static (IConfigurationRoot configuration, ServiceProvider serviceProvider) Setup(string settingsFileName = "appsettings.json", string connectionStringNameParam = "RatesDB")
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(settingsFileName);
            var configuration = builder.Build();
            var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IDBContextFactory>((sp) =>
            new DBContextFactory(configuration.GetConnectionString(connectionStringNameParam)))
            .AddSingleton<IBankService, BankService>()
            .AddSingleton<IReportService, ReportService>()
            .BuildServiceProvider();
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            return (configuration, serviceProvider);
        }
    }
}
