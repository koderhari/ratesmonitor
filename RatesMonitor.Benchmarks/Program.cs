using RatesMonitor.Core.Helper;
using RatesMonitor.Core.Infrastructure;
using System;
using Microsoft.Extensions.DependencyInjection;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using RatesMonitor.Core;

namespace RatesMonitor.Benchmarks
{
    class Program
    {

        static void Main(string[] args)
        {
            var (config,serviceProvider) = InstallConsoleEnvironment.Setup();
            //RepServiceBenchmark.repService = serviceProvider.GetService<IReportService>();
            //RepServiceBenchmark.serviceProvider = serviceProvider;
            BenchmarkRunner.Run<RepServiceBenchmark>();
            Console.ReadKey();
        }
    }

    [RankColumn]
    [Config(typeof(Config))]
    public class RepServiceBenchmark
    {
        private ReportService _reportService;

        public RepServiceBenchmark()
        {

        }

        [GlobalSetup]
        public void SetupData()
        {
            _reportService = new ReportService(new DBContextFactory("Server=localhost\\SQLExpress;Database=ratesdb;Trusted_Connection=True;"));
        }

        [Benchmark]
        public void Test()
        {
            //var repService = serviceProvider.GetService<IReportService>();
            _reportService.GetWeekRatesReport(2018, 2, new string[] { "USD" });
        }
    }


    public class Config : ManualConfig
    {
        public Config()
        {
            Add(new MemoryDiagnoser());
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
}
