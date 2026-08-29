using DistOMeter.Entities;

namespace DistOMeter.GUI.Models;

public class MeasurementResultsViewModel
{
    public MeasurementResult Result { get; set; }

    public MeasurementResultsViewModel(MeasurementResult result)
    {
        Result = result;
    }
}
