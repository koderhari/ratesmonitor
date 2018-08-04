using Microsoft.Extensions.Logging;
using RatesMonitor.Core;
using RatesMonitor.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace RatesMonitor.DailyUpdater.Infrastructure
{
    public class RatesUpdateScheduleTask : IRatesUpdateScheduleTask
    {
        private ILogger<RatesUpdateScheduleTask> _logger;
        private IBankService _bankService;
        private IDBContextFactory _contextFactory;
        private Timer _timer;

        public RatesUpdateScheduleTask(
            Settings settigns,
            IBankService bankService,
            IDBContextFactory contextFactory,
            ILogger<RatesUpdateScheduleTask> logger)
        {
            if (bankService == null)
                throw new ArgumentNullException(nameof(_bankService));
            _logger = logger;
            _bankService = bankService;
            _contextFactory = contextFactory;
            _timer = new Timer(settigns.Interval.TotalMilliseconds);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void Start()
        {
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer?.Stop();
            try
            {
                _logger.LogInformation("Start update");
                DoUpdate();
                _logger.LogInformation("Stop update");
            }
            catch (Exception error)
            {
                _logger.LogError($"Error when execute task {error}");
            }
            _timer?.Start();
        }

        private void DoUpdate()
        {
            var date = DateTime.Now.Date;
            using (var dbContext = _contextFactory.Create())
            {
                var dailyRates = _bankService.GetDailyRates(DateTime.Now).Result;
                var ratesInDb = dbContext.DailyRates.Where(x => x.Date == date).ToList();
                //в базе может быть а может не быть какой то валюты надо поправить этот case
                if (ratesInDb.Count != 0)
                {
                    var ratesInDbByCurrency = ratesInDb.ToDictionary(x => x.CurrencyCode);
                    foreach (var dailyRate in dailyRates)
                    {
                        var rateInDb = ratesInDbByCurrency[dailyRate.CurrencyCode];
                        rateInDb.OriginalRate = dailyRate.OriginalRate;
                        rateInDb.Amount = dailyRate.Amount;
                        rateInDb.FinalRate = dailyRate.FinalRate;
                    }
                    _logger.LogInformation($"Updated {ratesInDbByCurrency.Count} records");
                }
                else
                {
                    dbContext.AddRange(dailyRates);
                    _logger.LogInformation($"Inserted {dailyRates.Count} records");
                }

                dbContext.SaveChanges();
            }
        }


        public void Stop()
        {
            _timer?.Stop();
        }
    }
}
