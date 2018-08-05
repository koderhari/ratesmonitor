using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RatesMonitor.Core;
using RatesMonitor.Core.Infrastructure;
using RatesMonitor.WebApi.Models;

namespace RatesMonitor.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Required to use the Options<T> pattern
            services.AddOptions();

            // Add settings from configuration
            //services.Configure<SampleWebSettings>(Configuration);

            // Uncomment to add settings from code
            services.Configure<ReportSettings>(settings =>
            {
                

                settings.CurrenciesForReport = Configuration.GetSection("CurrenciesForReport").Get<List<string>>();
            });

            services.AddSingleton<IDBContextFactory>((sp) =>
            new DBContextFactory(Configuration.GetConnectionString("RatesDB")));
            services.AddSingleton<IReportService, ReportService>();

            var builder = services.AddMvcCore();
            builder.AddApiExplorer();
            builder.AddFormatterMappings();
            builder.AddJsonFormatters();
            builder.AddCors();

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
