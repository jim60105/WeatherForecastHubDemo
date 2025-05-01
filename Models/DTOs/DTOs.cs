namespace WeatherForecastHub.Models.DTOs;

public class CityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
}

public class CreateCityDto
{
    public string Name { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
}

public class UpdateCityDto
{
    public string Name { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
}

public class WeatherForecastDto
{
    public string CityName { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public double RainProbability { get; set; }
    public string WeatherCondition { get; set; } = string.Empty;
}
