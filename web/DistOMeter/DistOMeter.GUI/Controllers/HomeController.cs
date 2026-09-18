using DistOMeter.GUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DistOMeter.Controllers;
using DistOMeter.Entities;
using System.Runtime.CompilerServices;

namespace DistOMeter.GUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IMeasurementController _measurementController;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _measurementController = new MeasurementController();
        }

        public IActionResult Index()
        {
            MeasurementInputViewModel model = new MeasurementInputViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(MeasurementInputViewModel model)
        {
            if (model.Baseline <= 0)
                ModelState.AddModelError("", "The baseline must be greater than zero.");

            if (model.Objects.Count == 0)
                ModelState.AddModelError("", "Add at least one object.");

            foreach (var obj in model.Objects)
            {
                if (string.IsNullOrWhiteSpace(obj.Name) || obj.AngleR <= 0 || obj.AngleQ <= 0 || obj.AngleR >= 180 || obj.AngleQ >= 180 || obj.AngleR <= obj.AngleQ)
                    ModelState.AddModelError("", "Each object needs a name and valid angles. The angle from R must be greater than the angle from Q.");
            }

            if (model.Objects.Select(obj => obj.Name?.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() != model.Objects.Count)
                ModelState.AddModelError("", "Each object must have a unique name.");

            if (!ModelState.IsValid)
                return View(model);

            List<ObjectMeasurement> objects = model.Objects
                .Select(obj => new ObjectMeasurement(
                    obj.Name,
                    obj.AngleR,
                    obj.AngleQ
                ))
                .ToList();

            MeasurementRequest request = new MeasurementRequest(
                model.Baseline,
                objects
            );

            MeasurementResult result;
            try
            {
                result = await _measurementController.CalculateAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Measurement calculation failed");
                ModelState.AddModelError("", "The measurement could not be calculated. Check the values and try again.");
                return View(model);
            }

            MeasurementResultsViewModel resultsModel =
                new MeasurementResultsViewModel(result);

            return View("Results", resultsModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
