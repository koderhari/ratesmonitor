using RatesMonitor.Core.Helper;
using RatesMonitor.Core.Infrastructure;
using System;
using Microsoft.Extensions.DependencyInjection;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using RatesMonitor.Core;
using RatesMonitor.Domain;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using RatesMonitor.Core.Domain;
using RatesMonitor.Benchmarks.ReportServiceBenchmarks;

namespace RatesMonitor.Benchmarks
{
    class Program
    {

        static void Main(string[] args)
        {
            var (config,serviceProvider) = InstallConsoleEnvironment.Setup();
            //RepServiceBenchmark.repService = serviceProvider.GetService<IReportService>();
            //RepServiceBenchmark.serviceProvider = serviceProvider;
           // var bench = new RepServiceBenchmark();
           // bench.SetupData();
            //bench.TestVesionWithStruct();
            BenchmarkRunner.Run<RepServiceBenchmark>();
            //Console.WriteLine();
            Console.ReadKey();
        }
    }

   
}
