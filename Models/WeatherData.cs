namespace WeatherForecastHub.Models;

public class WeatherData
{
    public int Id { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public DateTime Datetime { get; set; } // 合併 Date 和 Time 欄位
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public double RainProbability { get; set; }
    public string WeatherCondition { get; set; } = string.Empty;
}
