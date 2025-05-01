namespace WeatherForecastHub.Models.DTOs;

public class CityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateCityDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateCityDto
{
    public string Name { get; set; } = string.Empty;
}

public class WeatherForecastDto
{
    public int Id { get; set; }
    public string CityName { get; set; } = string.Empty;
    public DateTime Date { get; set; } // 保留為兼容性，但我們實際上將使用 Datetime 的值
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public double RainProbability { get; set; }
    public string WeatherCondition { get; set; } = string.Empty;
}
