using WeatherForecastHub.Data;
using WeatherForecastHub.Models;

namespace WeatherForecastHub.Repositories;

public class CityRepository : ICityRepository
{
    private readonly WeatherDbContext _context;
    private readonly ILogger<CityRepository> _logger;

    public CityRepository(WeatherDbContext context, ILogger<CityRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<City>> GetAllCitiesAsync()
    {
        _logger.LogInformation("正在取得所有城市");
        return await _context.Cities.ToListAsync();
    }

    public async Task<City?> GetCityByIdAsync(int id)
    {
        _logger.LogInformation("正在取得 ID 為 {Id} 的城市", id);
        return await _context.Cities.FindAsync(id);
    }

    public async Task<City> AddCityAsync(City city)
    {
        _logger.LogInformation("正在新增城市: {Name}", city.Name);
        _context.Cities.Add(city);
        await _context.SaveChangesAsync();
        return city;
    }

    public async Task<City?> UpdateCityAsync(City city)
    {
        _logger.LogInformation("正在更新 ID 為 {Id} 的城市", city.Id);
        
        var existingCity = await _context.Cities.FindAsync(city.Id);
        if (existingCity == null)
        {
            _logger.LogWarning("找不到 ID 為 {Id} 的城市", city.Id);
            return null;
        }

        existingCity.Name = city.Name;
        existingCity.LocationId = city.LocationId;
        
        await _context.SaveChangesAsync();
        return existingCity;
    }

    public async Task<bool> DeleteCityAsync(int id)
    {
        _logger.LogInformation("正在刪除 ID 為 {Id} 的城市", id);
        
        var city = await _context.Cities.FindAsync(id);
        if (city == null)
        {
            _logger.LogWarning("找不到 ID 為 {Id} 的城市", id);
            return false;
        }

        _context.Cities.Remove(city);
        await _context.SaveChangesAsync();
        return true;
    }
}
