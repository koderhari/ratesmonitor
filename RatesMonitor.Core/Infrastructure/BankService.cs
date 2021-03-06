﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RatesMonitor.Domain;

namespace RatesMonitor.Core.Infrastructure
{
    public class BankService : IBankService
    {
        private ILogger<BankService> _logger;

        public BankService(ILogger<BankService> logger)
        {
            _logger = logger;
        }

        public async Task<List<DailyCurrencyRate>> GetDailyRates(DateTime date)
        {
            var result = new List<DailyCurrencyRate>();
            using (var client = GetHttpClient())
            {
                var dailyResponse = await client.GetAsync($"/en/financial_markets/foreign_exchange_market/exchange_rate_fixing/daily.txt?date={date.ToString("dd.MM.yyyy")}");
                var (valid, rawResponse) = await IsValidStringResponse(dailyResponse);
                if (valid)
                {
                    result = ParseDailyRates(rawResponse, date.Date);
                }
            }

            return result;

        }

        private async Task<(bool valid,string responseString)> IsValidStringResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var rawResponse = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(rawResponse))
                {
                    return (true, rawResponse);
                }
                else
                {
                    _logger.LogError($"Bank return empty string");
                    return (false, string.Empty);
                }
            }
            else
            {
                string rawResponse =string.Empty;
                try
                {
                    rawResponse = await response.Content.ReadAsStringAsync();
                }
                catch (Exception)
                {
                }
                _logger.LogError($"Not successful response from Bank:{response.StatusCode} {response.ReasonPhrase} {rawResponse}");
                return (false, string.Empty);
            }
        }

        private async Task<bool> IsValidResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                string rawResponse = string.Empty;
                try
                {
                    rawResponse = await response.Content.ReadAsStringAsync();
                }
                catch (Exception)
                {
                }
                _logger.LogError($"Not successful response from Bank:{response.StatusCode} {response.ReasonPhrase} {rawResponse}");
                return false;
            }
        }

        private List<DailyCurrencyRate> ParseDailyRates(string rawResponse, DateTime date)
        {
            var rows = rawResponse.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            var result = new List<DailyCurrencyRate>();
            var dtFromResponse = DateTime.Parse(rows[0].Split(" ")[0]);
            var isHoliday = dtFromResponse.Date != date;
            if (isHoliday) return new List<DailyCurrencyRate>();
            for (var i = 2; i < rows.Length; i++)
            {
                try
                {
                    var parts = rows[i].Split("|", StringSplitOptions.RemoveEmptyEntries);
                    var newItem = new DailyCurrencyRate();
                    newItem.CurrencyCode = parts[3];
                    newItem.Amount = parts[2].DecimalParse();
                    newItem.Date = date;
                    newItem.OriginalRate = parts[4].DecimalParse();
                    newItem.Year = date.Year;
                    newItem.Month = date.Month;
                    newItem.CalculateFinalRate();
                    result.Add(newItem);
                }
                catch (Exception error)
                {
                    _logger.LogError($"Error while parse {rows[i]} {error}");
                }
            }

            return result;
        }

        //to-do можно сделать еще версию с чтением из потока
        //будет работать пооптимальней
        public async Task<List<DailyCurrencyRate>> GetYearRates(int year)
        {
            var result = new List<DailyCurrencyRate>();
            using (var client = GetHttpClient())
            {
                var yearResponse = await client.GetAsync($"/en/financial_markets/foreign_exchange_market/exchange_rate_fixing/year.txt?year={year}");
                var (valid, rawResponse) = await IsValidStringResponse(yearResponse);
                if (valid)
                {
                    result = ParseYearRates(rawResponse);
                }
            }

            return result;
            
        }

        private List<DailyCurrencyRate> ParseYearRates(string rawResponse)
        {
            var result = new List<DailyCurrencyRate>();
            var rows = rawResponse.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            var ratesTemplate = ParseHeader(rows[0]);
            for (var i = 1; i < rows.Length; i++)
            {
                try
                {
                    result.AddRange(ParseYearRowRates(ratesTemplate, rows[i]));
                }
                catch (Exception error)
                {
                    _logger.LogError($"Error while parse {rows[i]} {error}");
                }
            }

            return result;
        }


        public async Task<List<DailyCurrencyRate>> GetYearRatesByDay(int year, Action<IEnumerable<DailyCurrencyRate>> proccessDay)
        {
            if (proccessDay == null)
                throw new ArgumentNullException(nameof(proccessDay));
            var result = new List<DailyCurrencyRate>();
            using (var client = GetHttpClient())
            {
                var yearResponse = await client.GetAsync($"/en/financial_markets/foreign_exchange_market/exchange_rate_fixing/year.txt?year={year}");
                var valid = await IsValidResponse(yearResponse);
                if (valid)
                {
                    using (var stream = await yearResponse.Content.ReadAsStreamAsync())
                    {
                        using (var readStream = new StreamReader(stream))
                        {
                            await ParseYearStream(readStream, proccessDay);
                        }
                    }
                }

                return result;

            }
        }

        private async Task ParseYearStream(StreamReader readStream, Action<IEnumerable<DailyCurrencyRate>> proccessDay)
        {
            int lineNumber = 0;
            List<(string code, int amount)> ratesTemplate = null;
            while (!readStream.EndOfStream)
            {
                var line = await readStream.ReadLineAsync();
                if (lineNumber == 0)
                {
                    ratesTemplate = ParseHeader(line);
                }
                else
                {
                    try
                    {
                        var item = ParseYearRowRates(ratesTemplate, line);
                        proccessDay(item);
                    }
                    catch (Exception error)
                    {

                        _logger.LogError($"Error while parse {line} {error}");
                    }
                }

                lineNumber++;
            }
        }

        

        private List<(string code, int amount)> ParseHeader(string row)
        {
            try
            {
                var result = new List<(string code, int amount)>();
                var parts = row.Split("|", StringSplitOptions.RemoveEmptyEntries);
                for (int i = 1; i < parts.Length; i++)
                {
                    var itemParts = parts[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    var amount = int.Parse(itemParts[0]);
                    result.Add((itemParts[1], amount));
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while parse header {row} {e}");
                throw;
            }

        }

        private IEnumerable<DailyCurrencyRate> ParseYearRowRates(List<(string code, int amount)> rowHeaderTemplates,string row)
        {
            var result = new List<DailyCurrencyRate>();
            var parts = row.Split("|", StringSplitOptions.RemoveEmptyEntries);
            var date = DateTime.Parse(parts[0]);
            for (int i = 0; i < rowHeaderTemplates.Count; i++)
            {
                var rate = parts[i + 1].DecimalParse();
                var newItem = new DailyCurrencyRate();
                newItem.CurrencyCode = rowHeaderTemplates[i].code;
                newItem.Amount = rowHeaderTemplates[i].amount;
                newItem.Date = date;
                newItem.OriginalRate = rate;
                newItem.Year = date.Year;
                newItem.Month = date.Month;
                newItem.CalculateFinalRate();
                result.Add(newItem);
            }
            return result;
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://www.cnb.cz");
            return client;
        }
    }
}
