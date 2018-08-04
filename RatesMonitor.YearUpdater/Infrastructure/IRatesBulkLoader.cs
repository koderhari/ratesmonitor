using System;
using System.Collections.Generic;
using System.Text;

namespace RatesMonitor.YearUpdater.Infrastructure
{
    public interface IRatesBulkLoader
    {
        void LoadData(int year);
    }
}
