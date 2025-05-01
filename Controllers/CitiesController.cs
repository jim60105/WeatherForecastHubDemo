using WeatherForecastHub.Models;
using WeatherForecastHub.Models.DTOs;
using WeatherForecastHub.Services;

namespace WeatherForecastHub.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;
    private readonly ILogger<CitiesController> _logger;

    public CitiesController(ICityService cityService, ILogger<CitiesController> logger)
    {
        _cityService = cityService;
        _logger = logger;
    }

    /// <summary>
    /// 取得所有關注的城市
    /// </summary>
    /// <returns>城市列表</returns>
    [HttpGet(Name = "GetCities")]
    [ProducesResponseType(typeof(IEnumerable<CityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCities()
    {
        _logger.LogInformation("取得所有城市");
        var cities = await _cityService.GetAllCitiesAsync();
        
        var cityDtos = cities.Select(c => new CityDto
        {
            Id = c.Id,
            Name = c.Name,
            LocationId = c.LocationId
        });
        
        return Ok(cityDtos);
    }

    /// <summary>
    /// 取得指定 ID 的關注城市
    /// </summary>
    /// <param name="id">城市 ID</param>
    /// <returns>城市資訊</returns>
    [HttpGet("{id}", Name = "GetCity")]
    [ProducesResponseType(typeof(CityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CityDto>> GetCity(int id)
    {
        _logger.LogInformation("取得 ID 為 {Id} 的城市", id);
        
        var city = await _cityService.GetCityByIdAsync(id);
        if (city == null)
        {
            _logger.LogWarning("找不到 ID 為 {Id} 的城市", id);
            return NotFound();
        }

        var cityDto = new CityDto
        {
            Id = city.Id,
            Name = city.Name,
            LocationId = city.LocationId
        };
        
        return Ok(cityDto);
    }

    /// <summary>
    /// 新增關注的城市
    /// </summary>
    /// <param name="createCityDto">城市資訊</param>
    /// <returns>新建的城市資訊</returns>
    [HttpPost(Name = "CreateCity")]
    [ProducesResponseType(typeof(CityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CityDto>> CreateCity(CreateCityDto createCityDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("輸入模型無效");
            return BadRequest(ModelState);
        }

        try
        {
            var city = new City
            {
                Name = createCityDto.Name,
                LocationId = createCityDto.LocationId
            };

            var createdCity = await _cityService.AddCityAsync(city);
            
            var cityDto = new CityDto
            {
                Id = createdCity.Id,
                Name = createdCity.Name,
                LocationId = createdCity.LocationId
            };

            return CreatedAtAction(nameof(GetCity), new { id = cityDto.Id }, cityDto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "新增城市時發生驗證錯誤");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增城市時發生錯誤");
            return StatusCode(StatusCodes.Status500InternalServerError, "無法新增城市，請稍後再試");
        }
    }

    /// <summary>
    /// 更新關注的城市
    /// </summary>
    /// <param name="id">城市 ID</param>
    /// <param name="updateCityDto">城市資訊</param>
    /// <returns>更新後的城市資訊</returns>
    [HttpPut("{id}", Name = "UpdateCity")]
    [ProducesResponseType(typeof(CityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CityDto>> UpdateCity(int id, UpdateCityDto updateCityDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("輸入模型無效");
            return BadRequest(ModelState);
        }

        try
        {
            var city = new City
            {
                Id = id,
                Name = updateCityDto.Name,
                LocationId = updateCityDto.LocationId
            };

            var updatedCity = await _cityService.UpdateCityAsync(city);
            if (updatedCity == null)
            {
                _logger.LogWarning("找不到 ID 為 {Id} 的城市", id);
                return NotFound();
            }
            
            var cityDto = new CityDto
            {
                Id = updatedCity.Id,
                Name = updatedCity.Name,
                LocationId = updatedCity.LocationId
            };

            return Ok(cityDto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "更新城市時發生驗證錯誤");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新城市時發生錯誤");
            return StatusCode(StatusCodes.Status500InternalServerError, "無法更新城市，請稍後再試");
        }
    }

    /// <summary>
    /// 刪除關注的城市
    /// </summary>
    /// <param name="id">城市 ID</param>
    /// <returns>無內容</returns>
    [HttpDelete("{id}", Name = "DeleteCity")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCity(int id)
    {
        try
        {
            var result = await _cityService.DeleteCityAsync(id);
            if (!result)
            {
                _logger.LogWarning("找不到 ID 為 {Id} 的城市", id);
                return NotFound();
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "刪除城市時發生驗證錯誤");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除城市時發生錯誤");
            return StatusCode(StatusCodes.Status500InternalServerError, "無法刪除城市，請稍後再試");
        }
    }
}
