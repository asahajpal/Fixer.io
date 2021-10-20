using Fixer_MVC.Controllers;
using Fixer_MVC.DataModel;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fixer_MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Proff_Mvc_UnitTests
{
    public class UnitTests
    {
        [Fact]
        public async Task Index_ReturnsAViewResult()
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
        public async Task ConvertAmount_ReturnsCorrectAmount()
        {
            // Arrange

            var mockFixerServiceClient = new Mock<IFixerServiceClient>();
            var mockLogger = new Mock<ILogger<HomeController>>();

            mockFixerServiceClient.Setup(
                client => client.GetLatestRates(It.IsAny<string>())).ReturnsAsync(GetTestRates()
            );
            var controller = new HomeController(mockLogger.Object, mockFixerServiceClient.Object);

            // Act

            var result = await controller.ConvertAmount(@"dfg", @"hjk", (float)45.78);
            // Assert

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CurrencyRateViewModel>(viewResult.ViewData.Model);
            Assert.Equal(2,model.CurrencyRates.First().value);
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
    }
}
