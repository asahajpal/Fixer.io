using Fixer_MVC.Controllers;
using Fixer_MVC.DataModel;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fixer_MVC;
using Fixer_MVC.WebServices;
using Microsoft.AspNetCore.Mvc;
using Moq.Protected;

namespace Fixer_Mvc_UnitTests
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

        // An example of a fragile Unit test, something which we do not need 
        // The fragile test is corrected with  Moq by mock the HttpClient dependency in the following unit-test
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

            var taskResult = await fixerServiceClient.ConvertAmount(@"NOK", @"INR", (float)45.78);
            // Assert

            var result = Assert.IsType<float>(taskResult);
            var model = Assert.IsAssignableFrom<float>(result);
            Assert.True(Math.Abs(model - 412.34) < 0.1,
                "Difference between actual and expected exchange rate was above permissible limit !");
        }

        [Fact]
        public async Task FixerServiceClient_ConvertAmount_ReturnsCorrectAmount_withMoq()
        {
            // Arrange
            const string testContent_base = "{\"success\": true,\"timestamp\": 1634936465,\"base\": \"EUR\",\"date\": \"2021-10-22\",\"rates\": {\"NOK\": 9.743821}}";
            const string testContent_target = "{\"success\": true,\"timestamp\": 1634936465,\"base\": \"EUR\",\"date\": \"2021-10-22\",\"rates\": {\"NOK\": 87.350212}}";

            var httpResponses = new Queue<HttpResponseMessage>();
            httpResponses.Enqueue(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(testContent_base)
            });
            httpResponses.Enqueue(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(testContent_target)
            });


            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponses.Dequeue);

            var mockFixerServiceSettings = new Mock<IFixerServiceSettings>();
            mockFixerServiceSettings.Setup(settings => settings.AccessKey).Returns(@"rtyrty");
           
            var httpClient = new HttpClient(mockMessageHandler.Object);
            httpClient.BaseAddress = new Uri(@"http://data.fixer.io/api/");

            var underTest = new FixerServiceClient(httpClient, mockFixerServiceSettings.Object);

            // Act
            var taskResult = await underTest.ConvertAmount(@"NOK", @"INR", (float)45.78);

            // Assert
            var result = Assert.IsType<float>(taskResult);
            var model = Assert.IsAssignableFrom<float>(result);
            Assert.True(Math.Abs(model - 410.41) < 0.1,
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
