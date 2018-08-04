using System;
using System.Collections.Generic;
using System.Text;
using RatesMonitor.Domain;

namespace RatesMonitor.Core
{
    public class DBContextFactory : IDBContextFactory
    {
        public RatesContext Create()
        {
            return new RatesContext();
        }
    }
}
