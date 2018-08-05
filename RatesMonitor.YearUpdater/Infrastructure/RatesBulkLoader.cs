using Microsoft.Extensions.Logging;
using RatesMonitor.Core;
using RatesMonitor.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RatesMonitor.YearUpdater.Infrastructure
{
    public class RatesBulkLoader : IRatesBulkLoader
    {
        private ILogger<RatesBulkLoader> _logger;
        private IBankService _bankService;
        private IDBContextFactory _contextFactory;

        public RatesBulkLoader(
           IBankService bankService,
           IDBContextFactory contextFactory,
           ILogger<RatesBulkLoader> logger)
        {
            if (bankService == null)
                throw new ArgumentNullException(nameof(bankService));
            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger;
            _bankService = bankService;
            _contextFactory = contextFactory;
        }

        public void LoadData(int year)
        {
            try
            {
                using (var dbContext = _contextFactory.Create())
                {
                    if (dbContext.DailyCurrencyRates.Any(x => x.Date.Year == year))
                    {
                        _logger.LogWarning($"Current year {year} already in db");
                    }

                    var yearRates = _bankService.GetYearRates(year).Result;
                    var bulkSize = 500;
                    for (int i = 0; i < yearRates.Count; i+=bulkSize)
                    {
                        var forInsert = yearRates.Skip(i).Take(bulkSize);
                        dbContext.DailyCurrencyRates.AddRange(forInsert);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception error)
            {
                _logger.LogError($"Error while LoadData for year {year} {error}");
            }
        }
    }
}
