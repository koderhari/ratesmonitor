using System;
using System.Collections.Generic;
using System.Text;

namespace RatesMonitor.DailyUpdater.Infrastructure
{
    public interface IRatesUpdateScheduleTask : IDisposable
    {
        void Start();
        void Stop();
    }
}
