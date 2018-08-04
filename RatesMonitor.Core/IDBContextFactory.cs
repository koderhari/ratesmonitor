using RatesMonitor.Domain;
using System;

namespace RatesMonitor.Core
{
    public interface IDBContextFactory
    {
        RatesContext Create();
    }
}
