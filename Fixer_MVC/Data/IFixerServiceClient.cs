using Fixer_MVC.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fixer_MVC.DataModel
{
    public interface IFixerServiceClient
    {
        Task<CurrencyRateDataModel> GetLatestRates(string tarCurr);

        HttpClient Client { get; }
    }
}