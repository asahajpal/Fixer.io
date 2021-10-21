using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Fixer_MVC.DataModel
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
    }

}
