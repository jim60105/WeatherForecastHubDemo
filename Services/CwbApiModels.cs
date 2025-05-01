using System.Text.Json.Serialization;

namespace WeatherForecastHub.Services;

// 用於解析中央氣象署 API 回應的類別
// 這些類別需要根據實際 API 回應格式調整
public class CwbApiResponse
{
    public Records? Records { get; set; }
}

public class Records
{
    public List<Location>? Locations { get; set; }
}

public class Location
{
    public List<WeatherElement>? WeatherElements { get; set; }
}

public class WeatherElement
{
    public string? ElementName { get; set; }
    public List<Time>? Times { get; set; }
}

public class Time
{
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public Parameter? Parameter { get; set; }
}

public class Parameter
{
    public string? ParameterName { get; set; }
    public string? ParameterValue { get; set; }
    public string? ParameterUnit { get; set; }
}
