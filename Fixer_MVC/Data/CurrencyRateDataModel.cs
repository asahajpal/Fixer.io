using Fixer_MVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fixer_MVC.DataModel
{
    public class CurrencyRate
    {
        [Display(Name = "Symbol")]
        [Required]
        public string symbol { get; set; }
        [Display(Name = "ExchangeRateValue")]
        [Required]
        public float value { get; set; }

    }

    public class CurrencyRateDataModel
    {
        public CurrencyRateDataModel(List<CurrencyRate> searchResult = null)
        {
            CurrencyRates = searchResult ?? new List<CurrencyRate>();
        }
        ///<summary>
        /// Gets or sets Customers.
        ///</summary>
        public List<CurrencyRate> CurrencyRates { get; set; }

        public string BaseCurrency { get; set; }

    }



}
