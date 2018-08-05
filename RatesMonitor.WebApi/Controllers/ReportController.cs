using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RatesMonitor.Core.Domain;
using RatesMonitor.Core.Infrastructure;
using RatesMonitor.WebApi.Infrastructure;
using RatesMonitor.WebApi.Models;

namespace RatesMonitor.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private ReportSettings _reportSettings;
        private IReportService _reportService;

        public ReportController(IOptions<ReportSettings> reportSettings, IReportService reportService)
        {
            _reportSettings = reportSettings.Value;
            _reportService = reportService;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            return Content("use this url /api/report/{year}/{month}/{format} , {format} -txt or json");
           
        }


        [HttpGet("{year}/{month}/{format?}")]
        public ActionResult<string> Get(int year,int month, string format ="text")
        {
            var reportItems = _reportService.GetWeekRatesReport(year, month, _reportSettings.CurrenciesForReport.ToArray());
            if (format == "json")
            {
                return new JsonResult(reportItems);
            }
            else
            {
                return Content(RatesExporterToCsv.Export(reportItems));
            }
        }
    }
}
