using CurrencyConverterAPI.Controllers;
using CurrencyConverterAPI.Repository;
using CurrencyConverterAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CurrencyConverterAPITests
{
    public class CurrencyControllerTests
    { 
        [Fact]
        public async Task ConvertCurrency_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var currencyServiceMock = new Mock<ICurrencyService>();
            currencyServiceMock.Setup(service => service.ConvertCurrencyAsync("USD", "EUR", 100))
                               .ReturnsAsync(85); 

            var exchangeRateRepositoryMock = new Mock<IExchangeRateRepository>();
            exchangeRateRepositoryMock.Setup(repo => repo.GetExchangeRateAsync("USD", "EUR"))
                                      .ReturnsAsync(0.85m); 

            var controller = new CurrencyController(currencyServiceMock.Object, exchangeRateRepositoryMock.Object);

            // Act
            var result = await controller.ConvertCurrency("USD", "EUR", 100);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var conversionResult = Assert.IsType<ConversionResult>(okResult.Value);
            Assert.Equal(0.85m, conversionResult.ExchangeRate);
            Assert.Equal(85, conversionResult.ConvertedAmount);
        }

        [Fact]
        public async Task ConvertCurrency_UnsupportedCurrencyPair_ThrowsNotSupportedException()
        {
            // Arrange
            var currencyServiceMock = new Mock<ICurrencyService>();
            var exchangeRateRepositoryMock = new Mock<IExchangeRateRepository>();
            exchangeRateRepositoryMock.Setup(repo => repo.GetExchangeRateAsync("AAA", "BBB"))
                                      .ReturnsAsync((decimal?)null);

            var controller = new CurrencyController(currencyServiceMock.Object, exchangeRateRepositoryMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<NotSupportedException>(async () =>
            {
                await controller.ConvertCurrency("AAA", "BBB", 100);
            });
        }

        [Fact]
        public async Task ConvertCurrency_InvalidInput_ThrowsArgumentException()
        {
            // Arrange
            var currencyServiceMock = new Mock<ICurrencyService>();
            var exchangeRateRepositoryMock = new Mock<IExchangeRateRepository>();

            var controller = new CurrencyController(currencyServiceMock.Object, exchangeRateRepositoryMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await controller.ConvertCurrency("", "EUR", 100);
            });
        }

        [Fact]
        public async Task ConvertCurrency_NegativeAmount_ThrowsArgumentException()
        {
            // Arrange
            var currencyServiceMock = new Mock<ICurrencyService>();
            var exchangeRateRepositoryMock = new Mock<IExchangeRateRepository>();

            var controller = new CurrencyController(currencyServiceMock.Object, exchangeRateRepositoryMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await controller.ConvertCurrency("USD", "EUR", -100);
            });
        }

    }
}