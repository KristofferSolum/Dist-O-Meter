using DistOMeter.Entities;

namespace DistOMeter.GUI.Models;

public class MeasurementInputViewModel
{
    public double Baseline { get; set; }

    public List<ObjectInputViewModel> Objects { get; set; } =
        new List<ObjectInputViewModel>
        {
            new ObjectInputViewModel()
        };
}