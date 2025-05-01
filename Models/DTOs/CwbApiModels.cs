namespace WeatherForecastHub.Models.DTOs;

// 用於解析中央氣象署 API 回應的類別
public class CwbApiResponse
{
    [JsonPropertyName("success")] public string? Success { get; set; }

    [JsonPropertyName("result")] public Result? Result { get; set; }

    [JsonPropertyName("records")] public Records? Records { get; set; }
}

public class Result
{
    [JsonPropertyName("resource_id")] public string? ResourceId { get; set; }

    [JsonPropertyName("fields")] public List<Field>? Fields { get; set; }
}

public class Field
{
    [JsonPropertyName("id")] public string? Id { get; set; }

    [JsonPropertyName("type")] public string? Type { get; set; }
}

public class Records
{
    [JsonPropertyName("Locations")] public List<Locations>? Locations { get; set; }
}

public class Locations
{
    [JsonPropertyName("DatasetDescription")]
    public string? DatasetDescription { get; set; }

    [JsonPropertyName("LocationsName")] public string? LocationsName { get; set; }

    [JsonPropertyName("Dataid")] public string? DataId { get; set; }

    [JsonPropertyName("Location")] public List<Location>? Location { get; set; }
}

public class Location
{
    [JsonPropertyName("LocationName")] public string? LocationName { get; set; }

    [JsonPropertyName("Geocode")] public string? Geocode { get; set; }

    [JsonPropertyName("Latitude")] public string? Latitude { get; set; }

    [JsonPropertyName("Longitude")] public string? Longitude { get; set; }

    [JsonPropertyName("WeatherElement")] public List<WeatherElement>? WeatherElement { get; set; }
}

public class WeatherElement
{
    [JsonPropertyName("ElementName")] public string? ElementName { get; set; }

    [JsonPropertyName("Time")] public List<Time>? Time { get; set; }
}

public class Time
{
    [JsonPropertyName("StartTime")] public string? StartTime { get; set; }

    [JsonPropertyName("EndTime")] public string? EndTime { get; set; }

    [JsonPropertyName("DataTime")] public string? DataTime { get; set; }

    [JsonPropertyName("ElementValue")] public List<ElementValue>? ElementValue { get; set; }
}

public class ElementValue
{
    [JsonPropertyName("Temperature")] public string? Temperature { get; set; }

    [JsonPropertyName("DewPoint")] public string? DewPoint { get; set; }

    [JsonPropertyName("RelativeHumidity")] public string? RelativeHumidity { get; set; }

    [JsonPropertyName("ApparentTemperature")]
    public string? ApparentTemperature { get; set; }

    [JsonPropertyName("ComfortIndex")] public string? ComfortIndex { get; set; }

    [JsonPropertyName("ComfortIndexDescription")]
    public string? ComfortIndexDescription { get; set; }

    [JsonPropertyName("WindDirection")] public string? WindDirection { get; set; }

    [JsonPropertyName("WindSpeed")] public string? WindSpeed { get; set; }

    [JsonPropertyName("BeaufortScale")] public string? BeaufortScale { get; set; }

    [JsonPropertyName("ProbabilityOfPrecipitation")]
    public string? ProbabilityOfPrecipitation { get; set; }

    [JsonPropertyName("Weather")] public string? Weather { get; set; }

    [JsonPropertyName("WeatherCode")] public string? WeatherCode { get; set; }
}
