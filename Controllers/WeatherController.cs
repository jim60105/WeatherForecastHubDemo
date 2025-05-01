using WeatherForecastHub.Models.DTOs;
using WeatherForecastHub.Services;

namespace WeatherForecastHub.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    /// <summary>
    /// 取得指定城市的氣象預報資料
    /// </summary>
    /// <param name="cityId">城市 ID</param>
    /// <returns>氣象預報資料</returns>
    [HttpGet("{cityId}", Name = "GetWeatherForecast")]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecastDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<WeatherForecastDto>>> GetWeatherForecast(int cityId)
    {
        _logger.LogInformation("取得城市 ID {CityId} 的天氣預報", cityId);

        try
        {
            var forecast = await _weatherService.GetWeatherForecastAsync(cityId);
            
            var forecastDtos = forecast.Select(f => new WeatherForecastDto
            {
                CityName = f.CityName,
                LocationId = f.LocationId,
                Date = f.Date,
                Temperature = f.Temperature,
                Humidity = f.Humidity,
                WindSpeed = f.WindSpeed,
                RainProbability = f.RainProbability,
                WeatherCondition = f.WeatherCondition
            });
            
            return Ok(forecastDtos);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得天氣預報時發生錯誤");
            return StatusCode(StatusCodes.Status500InternalServerError, "無法取得天氣預報，請稍後再試");
        }
    }
}
