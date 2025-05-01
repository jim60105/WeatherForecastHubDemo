using WeatherForecastHub.Models;
using WeatherForecastHub.Repositories;

namespace WeatherForecastHub.Services;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly ILogger<CityService> _logger;

    public CityService(ICityRepository cityRepository, ILogger<CityService> logger)
    {
        _cityRepository = cityRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<City>> GetAllCitiesAsync()
    {
        _logger.LogInformation("正在取得所有城市");
        return await _cityRepository.GetAllCitiesAsync();
    }

    public async Task<City?> GetCityByIdAsync(int id)
    {
        _logger.LogInformation("正在取得 ID 為 {Id} 的城市", id);
        return await _cityRepository.GetCityByIdAsync(id);
    }

    public async Task<City> AddCityAsync(City city)
    {
        _logger.LogInformation("正在新增城市: {Name}", city.Name);
        
        // 基本驗證
        if (string.IsNullOrWhiteSpace(city.Name))
        {
            _logger.LogWarning("城市名稱不能為空");
            throw new ArgumentException("城市名稱不能為空", nameof(city.Name));
        }

        if (string.IsNullOrWhiteSpace(city.LocationId))
        {
            _logger.LogWarning("LocationId 不能為空");
            throw new ArgumentException("LocationId 不能為空", nameof(city.LocationId));
        }

        return await _cityRepository.AddCityAsync(city);
    }

    public async Task<City?> UpdateCityAsync(City city)
    {
        _logger.LogInformation("正在更新 ID 為 {Id} 的城市", city.Id);
        
        // 基本驗證
        if (city.Id <= 0)
        {
            _logger.LogWarning("城市 ID 無效");
            throw new ArgumentException("城市 ID 必須大於 0", nameof(city.Id));
        }

        if (string.IsNullOrWhiteSpace(city.Name))
        {
            _logger.LogWarning("城市名稱不能為空");
            throw new ArgumentException("城市名稱不能為空", nameof(city.Name));
        }

        if (string.IsNullOrWhiteSpace(city.LocationId))
        {
            _logger.LogWarning("LocationId 不能為空");
            throw new ArgumentException("LocationId 不能為空", nameof(city.LocationId));
        }

        return await _cityRepository.UpdateCityAsync(city);
    }

    public async Task<bool> DeleteCityAsync(int id)
    {
        _logger.LogInformation("正在刪除 ID 為 {Id} 的城市", id);
        
        if (id <= 0)
        {
            _logger.LogWarning("城市 ID 無效");
            throw new ArgumentException("城市 ID 必須大於 0", nameof(id));
        }

        return await _cityRepository.DeleteCityAsync(id);
    }
}
