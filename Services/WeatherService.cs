using System.Net.Http.Headers;
using System.Text.Json;
using WeatherForecastHub.Models;
using WeatherForecastHub.Repositories;

namespace WeatherForecastHub.Services;

public class WeatherService : IWeatherService
{
    private readonly ICityRepository _cityRepository;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(
        ICityRepository cityRepository,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<WeatherService> logger)
    {
        _cityRepository = cityRepository;
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        // 設定 HttpClient
        _httpClient.BaseAddress = new Uri("https://opendata.cwb.gov.tw/api/v1/");
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<IEnumerable<WeatherData>> GetWeatherForecastAsync(int cityId)
    {
        _logger.LogInformation("正在取得城市 ID {CityId} 的天氣預報", cityId);

        // 從資料庫取得城市資訊
        var city = await _cityRepository.GetCityByIdAsync(cityId);
        if (city == null)
        {
            _logger.LogWarning("找不到 ID 為 {CityId} 的城市", cityId);
            throw new KeyNotFoundException($"找不到 ID 為 {cityId} 的城市");
        }

        try
        {
            // 從中央氣象署 API 取得天氣預報
            var forecast = await GetWeatherForecastFromApiAsync(city.Name);
            
            // 將城市名稱附加到每個預報資料項目
            var result = forecast.Select(f =>
            {
                f.CityName = city.Name;
                return f;
            });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得城市 {CityName} (ID: {CityId}) 的天氣預報時發生錯誤", 
                city.Name, cityId);
            throw;
        }
    }

    private async Task<IEnumerable<WeatherData>> GetWeatherForecastFromApiAsync(string cityName)
    {
        _logger.LogInformation("正在從中央氣象署 API 取得城市名稱為 {CityName} 的天氣預報", cityName);
        
        try
        {
            var apiKey = _configuration["WeatherApi:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("找不到中央氣象署 API 金鑰");
                throw new InvalidOperationException("找不到中央氣象署 API 金鑰");
            }

            // 建構 API 請求 URL
            // 這裡假設我們想取得未來 3 天的天氣預報
            // 實際 URL 與參數需要根據中央氣象署 API 文件調整
            var requestUrl = $"rest/datastore/F-D0047-089?Authorization={apiKey}&locationName={cityName}";
            
            // 發送請求
            var response = await _httpClient.GetAsync(requestUrl);
            
            // 確保請求成功
            response.EnsureSuccessStatusCode();
            
            // 解析回應資料
            var content = await response.Content.ReadAsStringAsync();
            var forecastData = JsonSerializer.Deserialize<CwbApiResponse>(content);
            
            if (forecastData == null || forecastData.Records == null || forecastData.Records.Locations == null)
            {
                _logger.LogWarning("從中央氣象署 API 獲取的資料為空或格式不正確");
                return Enumerable.Empty<WeatherData>();
            }

            // 轉換 API 回應為我們的模型資料
            // 這裡的轉換邏輯需要根據實際 API 回應調整
            var weatherDataList = new List<WeatherData>();
            
            // 模擬從 API 回應中解析資料
            // 實際解析邏輯需根據中央氣象署 API 回應格式調整
            for (int i = 0; i < 3; i++) // 假設我們只要 3 天的預報
            {
                var date = DateTime.Today.AddDays(i);
                weatherDataList.Add(new WeatherData
                {
                    LocationId = cityName,
                    Date = date,
                    Temperature = 25 + i, // 模擬溫度值
                    Humidity = 60 - i * 2, // 模擬濕度值
                    WindSpeed = 3 + i * 0.5, // 模擬風速值
                    RainProbability = 20 + i * 5, // 模擬降雨機率
                    WeatherCondition = "晴時多雲" // 模擬天氣狀況
                });
            }
            
            return weatherDataList;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "向中央氣象署 API 發送請求時發生錯誤");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "解析中央氣象署 API 回應時發生錯誤");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得天氣預報時發生未知錯誤");
            throw;
        }
    }
}
