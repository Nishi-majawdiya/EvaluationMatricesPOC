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
        public IActionResult PredictRainfall([FromBody] List<RainfallInput> inputs)
        {
            int maxLimit = 250;

            // Check null or empty
            if (inputs == null || inputs.Count == 0)
            {
                return BadRequest("No data provided");
            }

            // LIMIT CHECK
            if (inputs.Count > maxLimit)
            {
                _logger.LogWarning("❌ Rainfall limit exceeded: {count}", inputs.Count);

                return BadRequest(new
                {
                    message = $"Max {maxLimit} records allowed. You sent {inputs.Count}"
                });
            }

            _logger.LogInformation("🌧 Rainfall batch request: {count}", inputs.Count);

            try
            {
                var results = inputs.Select(x => _rainfallService.Predict(x)).ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Rainfall prediction failed");
                return StatusCode(500, "Error in rainfall prediction");
            }
        }

        // Calories Prediction
        [HttpPost("calories")]
        public IActionResult PredictCalories([FromBody] List<CaloriesInput> inputs)
        {
            int maxLimit = 250;

            // Null or Empty Check
            if (inputs == null || inputs.Count == 0)
            {
                _logger.LogWarning("⚠️ Calories request received with no data");
                return BadRequest("No data provided");
            }

            // LIMIT CHECK
            if (inputs.Count > maxLimit)
            {
                _logger.LogWarning("❌ Calories limit exceeded: {count}", inputs.Count);

                return BadRequest(new
                {
                    message = $"Max {maxLimit} records allowed. You sent {inputs.Count}"
                });
            }

            _logger.LogInformation("🔥 Calories batch request for {count} records", inputs.Count);

            try
            {
                var results = inputs
                    .Select(x => _caloriesService.Predict(x))
                    .ToList();

                _logger.LogInformation("✅ Calories prediction success");

                return Ok(new
                {
                    count = results.Count,
                    data = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Calories prediction failed");
                return StatusCode(500, "Error in calories prediction");
            }
        }

        // Check if CSV exists
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
        public IActionResult PredictLaptop([FromBody] List<LaptopInput> inputs)
        {
            int maxLimit = 250;

            // ✅ Null or Empty Check
            if (inputs == null || inputs.Count == 0)
            {
                _logger.LogWarning("⚠️ Laptop request received with no data");
                return BadRequest("No data provided");
            }

            // ✅ LIMIT CHECK
            if (inputs.Count > maxLimit)
            {
                _logger.LogWarning("❌ Laptop limit exceeded: {count}", inputs.Count);

                return BadRequest(new
                {
                    message = $"Max {maxLimit} records allowed. You sent {inputs.Count}"
                });
            }

            _logger.LogInformation("💻 Laptop batch request for {count} records", inputs.Count);

            try
            {
                var results = inputs
                    .Select(x => _laptopService.Predict(x))
                    .ToList();

                _logger.LogInformation("✅ Laptop prediction success");

                return Ok(new
                {
                    count = results.Count,
                    data = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Laptop prediction failed");
                return StatusCode(500, "Error in laptop prediction");
            }
        }
    }
}