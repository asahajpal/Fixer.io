using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Proff_MVC.Models;
using System.Net.Http;
using System.Net.Http.Headers;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Fixer_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFixerServiceClient _client;
        //private readonly IHttpClientFactory _factory;
        //private readonly IConfiguration _config;

        public HomeController(
            ILogger<HomeController> logger, 
            IFixerServiceClient fixerClient
        //  IHttpClientFactory factory,
       //   IConfiguration  config
            )
        {            
            _logger = logger;
            //_factory = factory;
            //_config = config;
            _client = fixerClient;
        }

        public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)

        {
            CompanyViewModel companyViewModel = new();

            if (!String.IsNullOrEmpty(searchString))
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                try
                {
                    {
                        ViewData["CurrentFilter"] = searchString;

                        //var client = _factory.CreateClient("ProffApi");
                        //var httpClient = _client.Client;

                        companyViewModel = await _client.GetCompaniesAsync(string.Format("?industry={0}&pageNumber={1}&pageSize={2}",
                            searchString, pageNumber, 10));

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                }
            }

            return companyViewModel != null ? View(companyViewModel) : throw new Exception("No Hit \\ Ingen treff ! ");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
