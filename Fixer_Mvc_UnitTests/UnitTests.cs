using Fixer_MVC.Controllers;
using Fixer_MVC.DataModel;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Fixer_MVC;
using Fixer_MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Proff_Mvc_UnitTests
{
    public class UnitTests
    {
        [Fact]
        public async Task Controller_Index_ReturnsAViewResult()
        {
            // Arrange
            var mockFixerServiceClient = new Mock<IFixerServiceClient>();
            var mockLogger = new Mock<ILogger<HomeController>>();

            mockFixerServiceClient.Setup(client => client.GetLatestRates(It.IsAny<string>())).ReturnsAsync(GetTestRates());
            var controller = new HomeController(mockLogger.Object, mockFixerServiceClient.Object);

            // Act

            var result = await controller.Index(@"dfg");
            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal<Type>(typeof(ViewResult), result.GetType());
        }

        [Fact]
        public async Task Controller_ConvertAmount_ReturnsCorrectAmount()
        {
            // Arrange
            
            var mockFixerServiceClient = new Mock<IFixerServiceClient>();
            var mockLogger = new Mock<ILogger<HomeController>>();

            mockFixerServiceClient.Setup(
                client => client.ConvertAmount(@"NOK", @"INR", (float)45.78)).ReturnsAsync((float)412.34
            );
            var controller = new HomeController(mockLogger.Object, mockFixerServiceClient.Object);
            

            // Act

           var result = await controller.ConvertAmount(@"NOK", @"INR", (float)45.78);
            // Assert

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<float>(viewResult.ViewData.Model);
            Assert.True(Math.Abs(model - 412.30) < 0.1,
                "Difference between actual and expected exchange rate was above permissible limit !");
        }

        [Fact]
        public async Task FixerServiceClient_ConvertAmount_ReturnsCorrectAmount()
        {
            // Arrange
            var setting = new FixerServiceSettings
            {
                AccessKey = @"03c9a16dd23d33c6fc0fd9b0108df652"
            };
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(@"http://data.fixer.io/api/")
            };

            var fixerServiceClient = new FixerServiceClient(httpClient, setting);
            

            // Act

            var result = await fixerServiceClient.ConvertAmount(@"NOK", @"INR", (float)45.78);
            // Assert

            var viewResult = Assert.IsType<float>(result);
            var model = Assert.IsAssignableFrom<float>(viewResult);
            Assert.True(Math.Abs(model - 412.34) < 0.1,
                "Difference between actual and expected exchange rate was above permissible limit !");
        }

        private static CurrencyRateDataModel GetTestRates()
        {
            var dummyObj = new CurrencyRateDataModel();
            var dummyRates = new List<CurrencyRate>();
            dummyRates.Add(new CurrencyRate
            {
                symbol = @"INR",
                value = (float)22.34
            });
            dummyRates.Add(new CurrencyRate
            {
                symbol = @"NOK",
                value = (float)20.34
            });
            dummyObj.BaseCurrency = @"EUR";
            dummyObj.CurrencyRates = dummyRates;

            return dummyObj;
        }

        private static CurrencyRateDataModel GetTestRatesBaseCurrency()
        {
            var dummyObj = new CurrencyRateDataModel();
            var dummyRates = new List<CurrencyRate>();
            dummyRates.Add(new CurrencyRate
            {
                symbol = @"NOK",
                value = (float)9.707
            });
            
            dummyObj.BaseCurrency = @"EUR";
            dummyObj.CurrencyRates = dummyRates;

            return dummyObj;
        }

        private static CurrencyRateDataModel GetTestRatesTargetCurrency()
        {
            var dummyObj = new CurrencyRateDataModel();
            var dummyRates = new List<CurrencyRate>();
            dummyRates.Add(new CurrencyRate
            {
                symbol = @"INR",
                value = (float)87.155
            });

            dummyObj.BaseCurrency = @"EUR";
            dummyObj.CurrencyRates = dummyRates;

            return dummyObj;
        }
    }
}
