using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistOMeter.Entities;

public class ObjectMeasurement
{
    public string Name { get; set; }
    public double AngleR { get; set; }
    public double AngleQ { get; set; }

    public ObjectMeasurement(string name, double angleR, double angleQ)
    {
        Name = name;
        AngleR = angleR;
        AngleQ = angleQ;
    }
}
