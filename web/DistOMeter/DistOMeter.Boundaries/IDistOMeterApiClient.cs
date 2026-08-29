using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistOMeter.Entities;

namespace DistOMeter.Boundaries;

public interface IDistOMeterApiClient
{
    Task<MeasurementResult> CalculateAsync(MeasurementRequest request);
}