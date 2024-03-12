using System.Text.Json;

namespace CurrencyConverterAPI.Repository
{
    public interface IExchangeRateRepository
    {
        Task<decimal?> GetExchangeRateAsync(string sourceCurrency, string targetCurrency);
    }

    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly Dictionary<string, decimal> _exchangeRates;

        public ExchangeRateRepository()
        {
            _exchangeRates = LoadExchangeRates();
        }

        private Dictionary<string, decimal> LoadExchangeRates()
        {
         
            var exchangeRatesJson = File.ReadAllText("exchangeRates.json");
            var exchangeRates = JsonSerializer.Deserialize<Dictionary<string, decimal>>(exchangeRatesJson);

           
            foreach (var envVar in Environment.GetEnvironmentVariables().Keys)
            {
                if (envVar.ToString().StartsWith("USD_TO_") || envVar.ToString().StartsWith("INR_TO_") || envVar.ToString().StartsWith("EUR_TO_"))
                {
                    var currencyCode = envVar.ToString().Substring(7);
                    if (decimal.TryParse(Environment.GetEnvironmentVariable(envVar.ToString()), out decimal rate))
                    {
                        exchangeRates[$"{currencyCode}_USD"] = 1 / rate;
                        exchangeRates[$"USD_{currencyCode}"] = rate;
                    }
                }
            }

            return exchangeRates;
        }

        public Task<decimal?> GetExchangeRateAsync(string sourceCurrency, string targetCurrency)
        {
            var key = $"{sourceCurrency.ToUpper()}_TO_{targetCurrency.ToUpper()}";
            if (_exchangeRates.TryGetValue(key, out var exchangeRate))
                return Task.FromResult<decimal?>(exchangeRate);

            return Task.FromResult<decimal?>(null);
        }
    }
}