using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistOMeter.Boundaries;
using DistOMeter.Entities;

namespace DistOMeter.Tests;

public class DistOMeterApiClientTests
{
    [Fact]
    public async Task CalculateAsync_ReturnsMeasurementResult()
    {
        using HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://127.0.0.1:8000")
        };

        IDistOMeterApiClient apiClient =
            new DistOMeterApiClient(httpClient);

        MeasurementRequest request = new MeasurementRequest(
            300,
            new List<ObjectMeasurement>
            {
                new ObjectMeasurement("A", 70, 40),
                new ObjectMeasurement("B", 55, 50),
                new ObjectMeasurement("C", 30, 60)
            }
        );

        MeasurementResult result =
            await apiClient.CalculateAsync(request);

        Assert.NotNull(result);
        Assert.Equal(300, result.Baseline);
        Assert.Equal(3, result.Objects.Count);
        Assert.Contains("A", result.Objects.Keys);
        Assert.Contains("B", result.Objects.Keys);
        Assert.Contains("C", result.Objects.Keys);

        Assert.Contains("A", result.DistancesFromP.Keys);

        Assert.False(string.IsNullOrWhiteSpace(result.PlotBase64));
    }
}
