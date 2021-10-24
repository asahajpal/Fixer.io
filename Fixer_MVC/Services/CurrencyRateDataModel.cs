using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fixer_MVC.WebServices
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
