using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistOMeter.Boundaries;
using DistOMeter.Controllers;
using DistOMeter.Entities;

namespace DistOMeter.Tests;

public class MeasurementFlowIntegrationTests
{
    [Fact]
    public async Task CalculateAsync_UsesControllerAndRealApiClient()
    {
        IMeasurementController controller =
            new MeasurementController();

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
            await controller.CalculateAsync(request);

        Assert.NotNull(result);
        Assert.Equal(300, result.Baseline);

        Assert.Equal(3, result.Objects.Count);
        Assert.Contains("A", result.Objects.Keys);
        Assert.Contains("B", result.Objects.Keys);
        Assert.Contains("C", result.Objects.Keys);

        Assert.Equal(3, result.DistancesFromP.Count);
        Assert.Equal(3, result.DistancesBetweenObjects.Count);

        Assert.False(string.IsNullOrWhiteSpace(result.PlotBase64));
    }
}