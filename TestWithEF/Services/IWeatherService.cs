using Polly.CircuitBreaker;

namespace TestWithEF.Services;

public interface IWeatherService
{
    Task<string> GetWeatherAsync();
}

public class WeatherService:IWeatherService
{
    private readonly HttpClient _httpClient;
    public WeatherService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<string> GetWeatherAsync()
    {
        try
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://official-joke-api.appspot.com/random_jok")
            };
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        catch (BrokenCircuitException ex)
        {

            return $"Request failed due to opened circuit: {ex.Message}";
        }
        catch (HttpRequestException httpEx)
        {
            return $"Request failed. StatusCode={httpEx.StatusCode} Message={httpEx.Message}";
        }
    }
}
