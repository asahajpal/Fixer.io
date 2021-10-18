using Fixer_MVC.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fixer_MVC
{
    public interface IFixerServiceClient
    {
        Task<CurrencyRateViewModel> GetLatestRates(string query);
        HttpClient Client { get; }
    }
}