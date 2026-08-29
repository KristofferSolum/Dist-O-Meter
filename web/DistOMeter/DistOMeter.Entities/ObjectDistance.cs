using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistOMeter.Entities;

public class ObjectDistance
{
    public string From { get; set; }
    public string To { get; set; }
    public double Distance { get; set; }

    public ObjectDistance(string from, string to, double distance)
    {
        From = from;
        To = to;
        Distance = distance;
    }
}