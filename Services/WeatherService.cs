using WeatherForecastHub.Models;
using WeatherForecastHub.Models.DTOs;
using WeatherForecastHub.Repositories;

namespace WeatherForecastHub.Services;

public class WeatherService : IWeatherService
{
    private readonly ICityRepository _cityRepository;
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(
        ICityRepository cityRepository,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<WeatherService> logger)
    {
        _cityRepository = cityRepository;
        _configuration = configuration;
        _logger = logger;
        _client = httpClientFactory.CreateClient("CWA");
    }

    public async Task<IEnumerable<WeatherData>> GetWeatherForecastAsync(int cityId)
    {
        _logger.LogInformation(message: "正在取得城市 ID {CityId} 的天氣預報", cityId);

        // 從資料庫取得城市資訊
        City? city = await _cityRepository.GetCityByIdAsync(cityId);
        if (city == null)
        {
            _logger.LogWarning(message: "找不到 ID 為 {CityId} 的城市", cityId);
            throw new KeyNotFoundException($"找不到 ID 為 {cityId} 的城市");
        }

        try
        {
            // 從中央氣象署 API 取得天氣預報
            IEnumerable<WeatherData> forecast = await GetWeatherForecastFromApiAsync(city.Name);

            // 將城市名稱附加到每個預報資料項目
            IEnumerable<WeatherData> result = forecast.Select(f =>
            {
                f.CityName = city.Name;
                return f;
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(exception: ex,
                             message: "取得城市 {CityName} (ID: {CityId}) 的天氣預報時發生錯誤",
                             city.Name,
                             cityId);

            throw;
        }
    }

    private async Task<IEnumerable<WeatherData>> GetWeatherForecastFromApiAsync(string cityName)
    {
        if (string.IsNullOrEmpty(cityName))
        {
            _logger.LogWarning("請求中缺少城市名稱");
            return Enumerable.Empty<WeatherData>();
        }

        var weatherDataList = new List<WeatherData>();

        try
        {
            string apiKey = _configuration["CWAApi:ApiKey"] ?? "";
            var requestUrl = $"rest/datastore/F-D0047-089?Authorization={apiKey}&LocationName={cityName}";

            // 發送請求至中央氣象署 API
            HttpResponseMessage response = await _client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            // 解析 API 回應
            string responseContent = await response.Content.ReadAsStringAsync();
            CwbApiResponse? apiResponse = JsonSerializer.Deserialize<CwbApiResponse>(json: responseContent,
                                                                                     options: new JsonSerializerOptions
                                                                                     {
                                                                                         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                                                                     });

            if (apiResponse?.Records?.Locations != null)
            {
                // 從回應中獲取資料
                List<Location>? locationData = apiResponse.Records.Locations.FirstOrDefault()?.Location;
                if (locationData != null && locationData.Count > 0)
                    foreach (Location location in locationData)
                    {
                        if (location.WeatherElement == null) continue;

                        // 找出各種氣象要素
                        WeatherElement? temperatureElement = location.WeatherElement.FirstOrDefault(w => w.ElementName == "溫度");
                        WeatherElement? humidityElement = location.WeatherElement.FirstOrDefault(w => w.ElementName == "相對濕度");
                        WeatherElement? windSpeedElement = location.WeatherElement.FirstOrDefault(w => w.ElementName == "風速");
                        WeatherElement? rainProbabilityElement = location.WeatherElement.FirstOrDefault(w => w.ElementName == "3小時降雨機率");
                        WeatherElement? weatherConditionElement = location.WeatherElement.FirstOrDefault(w => w.ElementName == "天氣現象");

                        // 確保至少有時間和溫度資料
                        if (temperatureElement?.Time != null && temperatureElement.Time.Count > 0)
                            // 處理每個時間點的資料
                            foreach (Time timePoint in temperatureElement.Time)
                            {
                                // 確定時間點
                                DateTime forecastDateTime;
                                if (!string.IsNullOrEmpty(timePoint.StartTime))
                                    forecastDateTime = DateTime.Parse(timePoint.StartTime!);
                                else if (!string.IsNullOrEmpty(timePoint.DataTime))
                                    forecastDateTime = DateTime.Parse(timePoint.DataTime!);
                                else
                                    continue; // 跳過無效時間點

                                // 處理溫度資料
                                double temperature = 0;
                                var hasData = false;
                                if (timePoint.ElementValue != null && timePoint.ElementValue.Any())
                                {
                                    string? tempValue = timePoint.ElementValue.FirstOrDefault(ev => !string.IsNullOrEmpty(ev.Temperature))
                                                                 ?.Temperature;

                                    if (!string.IsNullOrEmpty(tempValue) && double.TryParse(s: tempValue, result: out double temp) && temp > 0)
                                    {
                                        temperature = temp;
                                        hasData = true;
                                    }
                                }

                                if (!hasData) continue; // 如果沒有溫度資料，跳過此時間點

                                // 處理濕度資料
                                double humidity = 0;
                                if (humidityElement?.Time != null)
                                {
                                    Time? humidTimePoint = FindMatchingTimePoint(timePoints: humidityElement.Time, targetDateTime: forecastDateTime);
                                    if (humidTimePoint?.ElementValue != null && humidTimePoint.ElementValue.Any())
                                    {
                                        string? humidValue = humidTimePoint.ElementValue
                                                                           .FirstOrDefault(ev => !string.IsNullOrEmpty(ev.RelativeHumidity))
                                                                           ?.RelativeHumidity;

                                        if (!string.IsNullOrEmpty(humidValue) && double.TryParse(s: humidValue, result: out double humid)
                                                                              && humid > 0) humidity = humid;
                                    }
                                }

                                // 處理風速資料
                                double windSpeed = 0;
                                if (windSpeedElement?.Time != null)
                                {
                                    Time? windTimePoint = FindMatchingTimePoint(timePoints: windSpeedElement.Time, targetDateTime: forecastDateTime);
                                    if (windTimePoint?.ElementValue != null && windTimePoint.ElementValue.Any())
                                    {
                                        string? windValue = windTimePoint.ElementValue.FirstOrDefault(ev => !string.IsNullOrEmpty(ev.WindSpeed))
                                                                         ?.WindSpeed;

                                        if (!string.IsNullOrEmpty(windValue) && double.TryParse(s: windValue, result: out double wind) && wind > 0)
                                            windSpeed = wind;
                                    }
                                }

                                // 處理降雨機率資料
                                double rainProbability = 0;
                                if (rainProbabilityElement?.Time != null)
                                {
                                    Time? rainTimePoint =
                                        FindMatchingTimePoint(timePoints: rainProbabilityElement.Time, targetDateTime: forecastDateTime);

                                    if (rainTimePoint?.ElementValue != null && rainTimePoint.ElementValue.Any())
                                    {
                                        string? rainValue = rainTimePoint.ElementValue
                                                                         .FirstOrDefault(ev => !string.IsNullOrEmpty(ev.ProbabilityOfPrecipitation))
                                                                         ?.ProbabilityOfPrecipitation;

                                        if (!string.IsNullOrEmpty(rainValue) && double.TryParse(s: rainValue, result: out double rain) && rain >= 0)
                                            rainProbability = rain;
                                    }
                                }

                                // 處理天氣狀況
                                var weatherCondition = "未知";
                                if (weatherConditionElement?.Time != null)
                                {
                                    Time? weatherTimePoint =
                                        FindMatchingTimePoint(timePoints: weatherConditionElement.Time, targetDateTime: forecastDateTime);

                                    if (weatherTimePoint?.ElementValue != null && weatherTimePoint.ElementValue.Any())
                                    {
                                        string? weatherValue = weatherTimePoint.ElementValue.FirstOrDefault(ev => !string.IsNullOrEmpty(ev.Weather))
                                                                               ?.Weather;

                                        if (!string.IsNullOrEmpty(weatherValue)) weatherCondition = weatherValue;
                                    }
                                }

                                // 建立天氣資料物件
                                weatherDataList.Add(new WeatherData
                                {
                                    LocationId = location.LocationName ?? cityName,
                                    Datetime = forecastDateTime,
                                    Temperature = Math.Round(value: temperature, digits: 1),
                                    Humidity = Math.Round(value: humidity, digits: 1),
                                    WindSpeed = Math.Round(value: windSpeed, digits: 1),
                                    RainProbability = Math.Round(value: rainProbability, digits: 1),
                                    WeatherCondition = weatherCondition
                                });
                            }
                    }
            }

            // 如果無法解析資料，記錄警告
            if (!weatherDataList.Any())
            {
                _logger.LogWarning("無法從中央氣象署 API 回應中解析出有效的天氣資料");

                // 回傳空資料
                return Enumerable.Empty<WeatherData>();
            }

            return weatherDataList;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(exception: ex, message: "向中央氣象署 API 發送請求時發生錯誤");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(exception: ex, message: "解析中央氣象署 API 回應時發生錯誤");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(exception: ex, message: "處理天氣預報資料時發生未預期錯誤");
            throw;
        }
    }

    // 根據時間點找到匹配的資料
    private Time? FindMatchingTimePoint(List<Time> timePoints, DateTime targetDateTime)
    {
        return timePoints.FirstOrDefault(t =>
        {
            if (!string.IsNullOrEmpty(t.StartTime)) return DateTime.Parse(t.StartTime!) == targetDateTime;

            if (!string.IsNullOrEmpty(t.DataTime)) return DateTime.Parse(t.DataTime!) == targetDateTime;
            return false;
        });
    }
}
