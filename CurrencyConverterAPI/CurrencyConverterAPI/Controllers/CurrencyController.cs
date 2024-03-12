using CurrencyConverterAPI.Repository;
using CurrencyConverterAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public CurrencyController(ICurrencyService currencyService, IExchangeRateRepository exchangeRateRepository)
        {
            _currencyService = currencyService;
            _exchangeRateRepository= exchangeRateRepository;
        }

        [HttpGet("convert")]
        public async Task<ActionResult<ConversionResult>> ConvertCurrency([FromQuery] string sourceCurrency, [FromQuery] string targetCurrency, [FromQuery] decimal amount)
        {
            // Check if sourceCurrency is a valid ISO 4217 currency code
            if (string.IsNullOrEmpty(sourceCurrency) || sourceCurrency.Length != 3)
                throw new ArgumentException("Invalid source currency code");

            // Check if amount is a positive value
            if (amount <= 0)
                throw new ArgumentException("Amount must be a positive value");

            var exchangeRate = await _exchangeRateRepository.GetExchangeRateAsync(sourceCurrency, targetCurrency);
            if (exchangeRate == null)
                throw new NotSupportedException("Unsupported currency pair");

            var convertedAmount = await _currencyService.ConvertCurrencyAsync(sourceCurrency, targetCurrency, amount);
            return Ok(new ConversionResult { ExchangeRate = exchangeRate.Value, ConvertedAmount = convertedAmount });
        }
    }

    public class ConversionResult
    {
        public decimal ExchangeRate { get; set; }
        public decimal ConvertedAmount { get; set; }
    }
}