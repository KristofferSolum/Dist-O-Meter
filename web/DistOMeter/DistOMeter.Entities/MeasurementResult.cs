using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistOMeter.Entities;

public class MeasurementResult
{
    public double Baseline { get; set; }

    public Dictionary<string, PointCoordinate> ReferencePoints { get; set; }

    public Dictionary<string, PointCoordinate> Objects { get; set; }

    public Dictionary<string, double> DistancesFromP { get; set; }

    public List<ObjectDistance> DistancesBetweenObjects { get; set; }

    public string PlotBase64 { get; set; }

    public MeasurementResult(
        double baseline,
        Dictionary<string, PointCoordinate> referencePoints,
        Dictionary<string, PointCoordinate> objects,
        Dictionary<string, double> distancesFromP,
        List<ObjectDistance> distancesBetweenObjects,
        string plotBase64)
    {
        Baseline = baseline;
        ReferencePoints = referencePoints;
        Objects = objects;
        DistancesFromP = distancesFromP;
        DistancesBetweenObjects = distancesBetweenObjects;
        PlotBase64 = plotBase64;
    }
}