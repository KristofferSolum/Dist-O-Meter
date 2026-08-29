using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistOMeter.Entities;

public class PointCoordinate
{
    public double X { get; set; }
    public double Y { get; set; }

    public PointCoordinate(double x, double y)
    {
        X = x;
        Y = y;
    }
}
