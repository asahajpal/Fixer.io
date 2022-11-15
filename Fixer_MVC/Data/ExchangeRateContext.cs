using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deedle;
using Microsoft.EntityFrameworkCore;

namespace ATGCustReg_MVC.DataModel
{
    public class ExchangeRateContext : DbContext
    {
        public ExchangeRateContext(DbContextOptions<ExchangeRateContext> options) : base(options)
        {
        }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        // default behaviour of creating data table with plural name is overriden 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRate>().ToTable("ExchangeRate");
        }

        // TimeSeries Db
        public void DeedleTest()
        {
            var titanic = Frame.ReadCsv("titanic.csv").GroupRowsBy<int>("Pclass");
        }
    }

}
