using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using DistOMeter.Entities;
using System.Text.Json;

namespace DistOMeter.Boundaries;

public class DistOMeterApiClient : IDistOMeterApiClient
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public DistOMeterApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<MeasurementResult> CalculateAsync(MeasurementRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "/calculate",
            request,
            _jsonOptions
        );

        if(!response.IsSuccessStatusCode)
{
            string error = await response.Content.ReadAsStringAsync();

            throw new Exception(
                $"API error {(int)response.StatusCode}: {error}"
            );
        }

        MeasurementResult? result =
            await response.Content.ReadFromJsonAsync<MeasurementResult>(
                _jsonOptions
            );

        if (result is null)
        {
            throw new InvalidOperationException(
                "The Dist-O-Meter API returned an empty response."
            );
        }

        return result;
    }
}