using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RatesMonitor.Domain;

namespace RatesMonitor.Core
{
    public class DBContextFactory : IDBContextFactory
    {
        private DbContextOptionsBuilder<RatesContext> _optionsBuilder;

        public DBContextFactory(string connectionString)
        {
            _optionsBuilder = new DbContextOptionsBuilder<RatesContext>();
            _optionsBuilder.UseSqlServer(connectionString);
        }

        public RatesContext Create()
        {
            
            return new RatesContext(_optionsBuilder.Options);
        }
    }
}
