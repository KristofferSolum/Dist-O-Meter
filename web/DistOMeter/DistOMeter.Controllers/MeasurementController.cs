using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistOMeter.Boundaries;
using DistOMeter.Entities;

namespace DistOMeter.Controllers;

public class MeasurementController : IMeasurementController
{
    private readonly IDistOMeterApiClient _apiClient;

    public MeasurementController()
    {
        HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://127.0.0.1:8000")
        };

        _apiClient = new DistOMeterApiClient(httpClient);
    }

    public async Task<MeasurementResult> CalculateAsync(
        MeasurementRequest request)
    {
        return await _apiClient.CalculateAsync(request);
    }
}
