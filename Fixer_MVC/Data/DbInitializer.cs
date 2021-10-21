using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fixer_MVC.DataModel
{
    public static class DbInitializer
    {
        public static void Initialize(ExchangeRateContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.ExchangeRates.Any())
            {
                return; // DB has been seeded
            }

            var exchangeRate = new ExchangeRate
            {
                SymbolBaseCurr = @"NOK",
                SymbolTargetCurr = @"INR",
                ExchangeRateValue = (float)9.007,
                ExchangeRatetDateTime = DateTime.Now
            };

            context.ExchangeRates.Add(exchangeRate);

            context.SaveChanges();
        }
    }
}
   

