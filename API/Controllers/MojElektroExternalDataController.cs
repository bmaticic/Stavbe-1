using System;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MojElektroExternalDataController : BaseApiController
{
private readonly MojElektroApiService _apiService;

    public MojElektroExternalDataController(MojElektroApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet("meter-readings")]
    public async Task<IActionResult> GetMeterReadings(
        [FromQuery] string usagePoint,
        [FromQuery] string startTime,
        [FromQuery] string endTime,
        [FromQuery] string option,
        [FromHeader(Name = "X-API-TOKEN")] string apiToken)
    {
        var data = await _apiService.GetMeterReadingsAsync(usagePoint, startTime, endTime, option, apiToken);
        return Ok(data);
    }
}
