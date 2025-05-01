using WeatherForecastHub.Models;

namespace WeatherForecastHub.Repositories;

public interface ICityRepository
{
    Task<IEnumerable<City>> GetAllCitiesAsync();
    Task<City?> GetCityByIdAsync(int id);
    Task<City> AddCityAsync(City city);
    Task<City?> UpdateCityAsync(City city);
    Task<bool> DeleteCityAsync(int id);
}
