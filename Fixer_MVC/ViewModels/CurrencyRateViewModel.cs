using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ATGCustReg_MVC.ViewModels
{
    public class CurrencyRateView
    {
        [Display(Name = "Symbol")]
        [Required]
        public string symbol { get; set; }
        [Display(Name = "ExchangeRateValue")]
        [Required]
        public float value { get; set; }
    }

    public class CurrencyRateViewModel
    {
        public CurrencyRateViewModel()
        {
            
        }

        ///<summary>
        /// Gets or sets Customers.
        ///</summary>
        public List<CurrencyRateView> CurrencyRates { get; set; }
        public string errorInfo { get; set; }

        public string BaseCurrency { get; set; }

    }

}
