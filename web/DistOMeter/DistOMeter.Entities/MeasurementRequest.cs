using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistOMeter.Entities;

public class MeasurementRequest
{
    public double Baseline { get; set; }
    public List<ObjectMeasurement> Objects { get; set; }

    public MeasurementRequest(double baseline, List<ObjectMeasurement> objects)
    {
        Baseline = baseline;
        Objects = objects;
    }
}
