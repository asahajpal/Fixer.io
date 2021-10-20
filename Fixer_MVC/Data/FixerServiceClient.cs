using Microsoft.Extensions.Configuration;
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

namespace Fixer_MVC.DataModel
{
    public class FixerServiceClient : IFixerServiceClient
    {
        private readonly HttpClient _client;
        private readonly FixerServiceSettings _settings;

        public FixerServiceClient(HttpClient client, FixerServiceSettings settings)
        {
            _client = client;
            _settings = settings;
        }

        public HttpClient Client => _client;

        public async Task<CurrencyRateDataModel> GetLatestRates(string targetCurr )
        {
            HttpResponseMessage lookupResult = await GetResultFromApi(targetCurr);

            if (lookupResult.IsSuccessStatusCode)
            {
                return ConvertApiResultToDataModel(lookupResult);
            }
            else //web api sent error response 
            {
                //log response status here..
                return null;
            }
        }

        private async Task<HttpResponseMessage> GetResultFromApi(string sym)
        {
            HttpResponseMessage lookupResult;

            string endpoint = "latest";

            // prepare absolute Service Uri 
            var reqUri = new Uri(string.Format("{0}{1}{2}",
                _client.BaseAddress.OriginalString, endpoint, "?access_key=" + _settings.AccessKey));

            if (sym.Equals("*"))
                lookupResult = await _client.GetAsync(reqUri);
            else
                lookupResult = await _client.GetAsync(reqUri + "&symbols=" + sym);
            return lookupResult;
        }

        private static CurrencyRateDataModel ConvertApiResultToDataModel(HttpResponseMessage lookupResult)
        {
            JObject response;
            string successVal;
           
            CurrencyRateDataModel currencyRateDataModel = new CurrencyRateDataModel();

            ProcessJsonResponse(lookupResult, out response, out successVal);

            if (successVal.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                SerializeJson2DataModel(currencyRateDataModel, response);
            }
            else
            {
                if (response != null)
                {
                    throw (new Exception(message: response["error"].ToString()));
                }
            }

            return currencyRateDataModel;
        }

        private static void ProcessJsonResponse(HttpResponseMessage lookupResult, out JObject response, out string successVal)
        {
            var readTask = lookupResult.Content.ReadAsStringAsync();
            readTask.Wait();

            var jsonString = readTask.Result;
            response = JObject.Parse(jsonString);
            successVal = response["success"].ToString();
        }
        private static void SerializeJson2DataModel(
            CurrencyRateDataModel currencyRateDataModel, JObject response
            )
        {
            var searchResults = new List<CurrencyRate>();
            // get JSON result objects into a list
            IList<JToken> results = response["rates"].Children().ToList();

            // serialize JSON results into .NET objects
            foreach (JToken result in results)
            {
                var currRate = (JProperty)result;
                var currRateObj = new CurrencyRate();
                currRateObj.symbol = currRate.Name;
                currRateObj.value = float.Parse(currRate.Value.ToString(), CultureInfo.CurrentCulture);
                searchResults.Add(currRateObj);
            }

            currencyRateDataModel.CurrencyRates = searchResults;

            currencyRateDataModel.BaseCurrency = response["base"].ToString();
        }
    }
}
   
   
    

