using Microsoft.AspNetCore.Mvc;
using EvaluationMatricesPOC.Models;
using EvaluationMatricesPOC.Services;
using Microsoft.Extensions.Logging;

namespace EvaluationMatricesPOC.Controllers
{
    [ApiController]
    [Route("api/prediction")]
    public class PredictionController : ControllerBase
    {
        private readonly RainfallService _rainfallService;
        private readonly CaloriesService _caloriesService;
        private readonly LaptopService _laptopService;
        private readonly ILogger<PredictionController> _logger;

        // ✅ Dependency Injection with Logger
        public PredictionController(
            RainfallService rainfallService,
            CaloriesService caloriesService,
            LaptopService laptopService,
            ILogger<PredictionController> logger)
        {
            _rainfallService = rainfallService;
            _caloriesService = caloriesService;
            _laptopService = laptopService;
            _logger = logger;
        }

        // 🌧 Rainfall Prediction
        [HttpPost("rainfall")]
        public IActionResult PredictRainfall([FromBody] RainfallInput input)
        {
            _logger.LogInformation("🌧 Rainfall prediction requested: {@input}", input);

            try
            {
                var result = _rainfallService.Predict(input);
                _logger.LogInformation("✅ Rainfall prediction success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Rainfall prediction failed");
                return StatusCode(500, "Error in rainfall prediction");
            }
        }

        // 🔥 Calories Prediction
        [HttpPost("calories")]
        public IActionResult PredictCalories([FromBody] CaloriesInput input)
        {
            _logger.LogInformation("🔥 Calories prediction requested: {@input}", input);

            try
            {
                var result = _caloriesService.Predict(input);
                _logger.LogInformation("✅ Calories prediction success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Calories prediction failed");
                return StatusCode(500, "Error in calories prediction");
            }
        }

        // 🔍 Check if CSV exists
        [HttpGet("calories-status")]
        public IActionResult CaloriesStatus()
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Data",
                "CaloriesBurnPrediction.csv"
            );

            bool isDummy = !System.IO.File.Exists(path);

            _logger.LogInformation("📊 Calories data source check: {status}",
                isDummy ? "Using Dummy Data" : "Using Real CSV");

            return Ok(new
            {
                isDummy = isDummy
            });
        }

        // 💻 Laptop Prediction
        [HttpPost("laptop")]
        public IActionResult PredictLaptop([FromBody] LaptopInput input)
        {
            _logger.LogInformation("💻 Laptop prediction requested: {@input}", input);

            try
            {
                var result = _laptopService.Predict(input);
                _logger.LogInformation("✅ Laptop prediction success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Laptop prediction failed");
                return StatusCode(500, "Error in laptop prediction");
            }
        }
    }
}