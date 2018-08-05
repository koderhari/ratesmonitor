using RatesMonitor.Core.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatesMonitor.WebApi.Infrastructure
{
    public static class RatesExporterToCsv
    {

        public static Stream ExportS(List<WeekRates> weeksRates)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            //using (var sw= new StreamWriter(ms))
            //{
                //1...2: USD - max: , min: , median: ; EUR - max: , min: , media: ;

                foreach (var item in weeksRates)
                {
                    var line = new StringBuilder();
                    line.AppendFormat("{0}..{1}: ", item.Days.FirstOrDefault(), item.Days.LastOrDefault());
                    foreach (var currency in item.CurrencyRates)
                    {
                        line.AppendFormat("{0} - max: {1}, min: {2}, median: {3};", 
                            currency.Key, 
                            currency.Value.Max,
                            currency.Value.Min,
                            currency.Value.Median);
                    }
                    sw.WriteLine(line.ToString());
                }
                
            //}
            sw.Flush();
            ms.Seek(0,SeekOrigin.Begin);
            return ms;
        }

        public static string Export(List<WeekRates> weeksRates)
        {

            //using (var sw= new StreamWriter(ms))
            //{
            //1...2: USD - max: , min: , median: ; EUR - max: , min: , media: ;
            var line = new StringBuilder();
            foreach (var item in weeksRates)
            {
                //var line = new StringBuilder();
                line.AppendFormat("{0}..{1}: ", item.Days.FirstOrDefault(), item.Days.LastOrDefault());
                foreach (var currency in item.CurrencyRates)
                {
                    line.AppendFormat("{0} - max: {1}, min: {2}, median: {3};",
                        currency.Key,
                        currency.Value.Max,
                        currency.Value.Min,
                        currency.Value.Median);
                }
                line.AppendLine();
                //sw.WriteLine(line.ToString());
            }

            //}
           // sw.Flush();
            //ms.Seek(0, SeekOrigin.Begin);
            return line.ToString();
        }
    }
}
