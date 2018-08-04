using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RatesMonitor.Domain
{
    public class RatesContext: DbContext
    {
        public DbSet<CurrencyRate> DailyRates { get; set; }

        public RatesContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyRate>()
                .HasKey(o => new { o.Date, o.CurrencyCode });
            modelBuilder.Entity<CurrencyRate>().Property(p => p.Date).HasColumnType("date");
            //to-do for final rate set precision
            //modelBuilder.Entity<CurrencyRate>().Property(p => p.FinalRate).prHasColumnType("date");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLExpress;Database=ratesdb;Trusted_Connection=True;");
        }
    }
}
