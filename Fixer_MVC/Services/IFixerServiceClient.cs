using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


namespace Fixer_MVC.WebServices
{
    public interface IFixerServiceClient
    {
        Task<CurrencyRateDataModel> GetLatestRates(string tarCurr);

        Task<float> ConvertAmount(string targetCurr1, string targetCurr2, float amount);

        HttpClient Client { get; }
    }
}