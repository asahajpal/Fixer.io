using Proff_MVC.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fixer_MVC
{
    public interface IFixerServiceClient
    {
        Task<CompanyViewModel> GetCompaniesAsync(string query);
        HttpClient Client { get; }
    }
}