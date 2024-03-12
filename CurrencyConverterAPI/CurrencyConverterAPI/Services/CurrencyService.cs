using CurrencyConverterAPI.Repository;

namespace CurrencyConverterAPI.Services
{
    public interface ICurrencyService
    {
        Task<decimal> ConvertCurrencyAsync(string sourceCurrency, string targetCurrency, decimal amount);
    }

    public class CurrencyService : ICurrencyService
    {
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public CurrencyService(IExchangeRateRepository exchangeRateRepository)
        {
            _exchangeRateRepository = exchangeRateRepository;
        }

        public async Task<decimal> ConvertCurrencyAsync(string sourceCurrency, string targetCurrency, decimal amount)
        {
            var exchangeRate = await _exchangeRateRepository.GetExchangeRateAsync(sourceCurrency, targetCurrency);
            if (exchangeRate == null)
                throw new ArgumentException("Unsupported currency pair");

            return amount * exchangeRate.Value;
        }
    }

}
