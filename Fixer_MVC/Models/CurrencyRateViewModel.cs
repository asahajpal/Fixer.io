﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fixer_MVC.Models
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
