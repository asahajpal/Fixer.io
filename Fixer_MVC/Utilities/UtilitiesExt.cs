﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ATGCustReg_MVC.DataModel;
using ATGCustReg_MVC.ViewModels;
using ATGCustReg_MVC.WebServices;

namespace ATGCustReg_MVC
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == string.Empty) ||
                   (token.Type == JTokenType.Null);
        }

        public static CurrencyRateView ToModel(this CurrencyRate entity)
        {
            var currencyRateView = new CurrencyRateView
            {
                symbol = entity.symbol,
                value = entity.value
            };
            return currencyRateView;
        }
    }
}
