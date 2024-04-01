using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ATGCustReg_MVC.WebServices;
using ATGCustReg_MVC.ViewModels;
using ATGCustReg_MVC.DataModel;

namespace ATGCustReg_MVC.Controllers
{
    //[Route("api/[controller]/[action]")]
    //[ApiController]
    //[Route("api/[controller]")]
    //[Route("api/")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFixerServiceClient _client;
        private readonly ExchangeRateContext _context;

        public HomeController(
            ILogger<HomeController> logger, 
            IFixerServiceClient fixerClient,
            ExchangeRateContext context
        )
        {            
            _logger = logger;
            _client = fixerClient;
            _context = context;
        }


        //[HttpGet("{targetCurr}")]
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

        //[HttpGet("{baseCurr},{targetCurr},{amount}")]
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

        //[HttpGet()]
        public IActionResult Privacy()
        {
            return View();
        }

       // [HttpGet()]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
