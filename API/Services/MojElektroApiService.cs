using System.Net.Http;
using System.Threading.Tasks;
using API.DTOs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace API.Services;

public class MojElektroApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public MojElektroApiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _baseUrl = "https://mojelektro.informatika.si/api"; // or from config
    }

    public async Task<IntervalBlock> GetMeterReadingsAsync(string usagePoint, string startTime, string endTime, string option, string apiToken)
    {
        var url = $"https://api-test.informatika.si/mojelektro/v1/meter-readings?usagePoint={Uri.EscapeDataString(usagePoint)}&startTime={Uri.EscapeDataString(startTime)}&endTime={Uri.EscapeDataString(endTime)}&option={Uri.EscapeDataString(option)}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("accept", "application/json");
        request.Headers.Add("X-API-TOKEN", apiToken);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error fetching data: {response.ReasonPhrase}");
        }
        IntervalBlock? myDeserializedClass =
            JsonConvert.DeserializeObject<IntervalBlock>(await response.Content.ReadAsStringAsync());

        if (myDeserializedClass == null)
        {
            throw new JsonException("Failed to deserialize meter readings response.");
        }

        return myDeserializedClass;
    }
}
