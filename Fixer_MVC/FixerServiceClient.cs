using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Proff_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Fixer_MVC
{
    public class FixerServiceClient : IFixerServiceClient
    {
        private readonly HttpClient _client;

        public FixerServiceClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<CompanyViewModel> GetCompaniesAsync(string query)
        {
            var lookupResult = await _client.GetAsync(query);
            var searchResults = new List<Company>();
            CompanyViewModel companyViewModel = new CompanyViewModel();

            if (lookupResult.IsSuccessStatusCode)
            {
                var readTask = lookupResult.Content.ReadAsStringAsync();
                readTask.Wait();

                var jsonString = readTask.Result;
                JObject compSearch = JObject.Parse(jsonString);

                // get JSON result objects into a list
                IList<JToken> results = compSearch["companies"].Children().ToList();

                // serialize JSON results into .NET objects

                foreach (JToken result in results)
                {
                    Company companyInfo = result.ToObject<Company>();
                    searchResults.Add(companyInfo);
                }

                companyViewModel.Companies = searchResults;
                companyViewModel.TotalPages = compSearch["pagination"]["NumberOfAvailablePages"].ToObject<int>();
                companyViewModel.PageSize = compSearch["pagination"]["PageSize"].ToObject<int>();
                companyViewModel.PageIndex = compSearch["pagination"]["CurrentPage"].ToObject<int>();

                return companyViewModel;
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
   
   
    

