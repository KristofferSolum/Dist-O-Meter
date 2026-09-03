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
            List<ObjectMeasurement> objects = model.Objects
                .Where(obj => obj.AngleR > 0 && obj.AngleQ > 0)
                .Select(obj => new ObjectMeasurement(
                    obj.Name,
                    obj.AngleR,
                    obj.AngleQ
                ))
                .ToList();

                Console.WriteLine($"Baseline: {model.Baseline}");
                Console.WriteLine($"ModelState valid: {ModelState.IsValid}");

                for (int i = 0; i < model.Objects.Count; i++)
                {
                    Console.WriteLine(
                        $"Object {i}: Name={model.Objects[i].Name}, " +
                        $"AngleR={model.Objects[i].AngleR}, " +
                        $"AngleQ={model.Objects[i].AngleQ}"
                    );
                }

            MeasurementRequest request = new MeasurementRequest(
                model.Baseline,
                objects
            );

            MeasurementResult result =
                await _measurementController.CalculateAsync(request);

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
