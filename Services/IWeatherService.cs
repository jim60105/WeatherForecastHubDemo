using WeatherForecastHub.Models;

namespace WeatherForecastHub.Services;

public interface IWeatherService
{
    Task<IEnumerable<WeatherData>> GetWeatherForecastAsync(int cityId);
}
