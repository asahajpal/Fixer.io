using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fixer_MVC.WebServices;
using Fixer_MVC.ViewModels;

namespace Fixer_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFixerServiceClient _client;

        public HomeController(
            ILogger<HomeController> logger, 
            IFixerServiceClient fixerClient
        )
        {            
            _logger = logger;
            _client = fixerClient;
        }

        public async Task<IActionResult> Index( string targetCurr)

        {
            CurrencyRateDataModel currencyRateDataModel = new();
            CurrencyRateViewModel currencyRateViewModel = new();

            if (!string.IsNullOrEmpty(targetCurr))
            {
                try
                {
                    {
                        currencyRateDataModel = await _client.GetLatestRates(targetCurr);
                    }
                }
                catch (Exception ex)
                {
                    currencyRateViewModel.errorInfo = ex.Message;
                    return View(currencyRateViewModel);
                }
            }

            // Mapping from DataModel to ViewModel
            if (currencyRateDataModel != null)
            {
                currencyRateViewModel.BaseCurrency = currencyRateDataModel.BaseCurrency;
                currencyRateViewModel.CurrencyRates = currencyRateDataModel.CurrencyRates.Select(x => x.ToModel()).ToList();
                
            }

            return currencyRateViewModel != null ? View(currencyRateViewModel) : View();

        }

        public async Task<IActionResult> ConvertAmount(string baseCurr, string targetCurr, float amount)

        {
            float convertedAmount = 0;

            if (!(string.IsNullOrEmpty(baseCurr) || string.IsNullOrEmpty(targetCurr) || amount < 0))
            {             
                try
                {
                    convertedAmount = await _client.ConvertAmount(baseCurr, targetCurr, amount);
                    ViewData["convertedAmount"] = convertedAmount;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                }
            }
            return View(convertedAmount);
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
