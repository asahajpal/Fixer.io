﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Fixer_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Globalization;

namespace Fixer_MVC
{
    public class FixerServiceClient : IFixerServiceClient
    {
        private readonly HttpClient _client;

        public FixerServiceClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<CurrencyRateViewModel> GetLatestRates(string sym )
        {
            HttpResponseMessage lookupResult;

            if (sym.Equals("*"))
                lookupResult = await _client.GetAsync(_client.BaseAddress);
            else
                lookupResult = await _client.GetAsync(_client.BaseAddress+"&symbols="+sym);

            var searchResults = new List<CurrencyRate>();
            CurrencyRateViewModel currecyRateViewModel = new CurrencyRateViewModel();

            if (lookupResult.IsSuccessStatusCode)
            {
                var readTask = lookupResult.Content.ReadAsStringAsync();
                readTask.Wait();

                var jsonString = readTask.Result;
                JObject response = JObject.Parse(jsonString);

                var successVal = response["success"].ToString();

                if (successVal.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    // get JSON result objects into a list
                    IList<JToken> results = response["rates"].Children().ToList();

                    // serialize JSON results into .NET objects

                    foreach (JToken result in results)
                    {
                        var currencyRate = result.ToObject(typeof(JObject));

                        var currRate = currencyRate.ToString().Split(':');
                        var currRateObj = new CurrencyRate();
                        currRateObj.symbol = currRate[0];
                        currRateObj.value = float.Parse(currRate[1].Trim(), CultureInfo.InvariantCulture);
                        searchResults.Add(currRateObj);
                    }

                    currecyRateViewModel.BaseCurrency = response["base"].ToString();
                }
                else
                {
                    currecyRateViewModel.errorInfo = response["error"].ToString();
                }

                currecyRateViewModel.CurrencyRates = searchResults;
             
                return currecyRateViewModel;
            }
            else //web api sent error response 
            {
                //log response status here..
                return null;
            }
        }

        public HttpClient Client => _client;
    }
}
   
   
    

