using Microsoft.AspNetCore.Mvc;
using EvaluationMatricesPOC.Models;
using EvaluationMatricesPOC.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace EvaluationMatricesPOC.Controllers
{
    [ApiController]
    [Route("api/prediction")]
    public class PredictionController : ControllerBase
    {
        private readonly IRainfallService _rainfallService;
        private readonly ICaloriesService _caloriesService;
        private readonly ILaptopService _laptopService;
        private readonly ILogger<PredictionController> _logger;

        // Dependency Injection with Interfaces
        public PredictionController(
            IRainfallService rainfallService,
            ICaloriesService caloriesService,
            ILaptopService laptopService,
            ILogger<PredictionController> logger)
        {
            _rainfallService = rainfallService;
            _caloriesService = caloriesService;
            _laptopService = laptopService;
            _logger = logger;
        }

        /// <summary>
        /// Predicts rainfall for a batch of inputs asynchronously.
        /// </summary>
        [HttpPost("rainfall")]
        public async Task<IActionResult> PredictRainfall([FromBody] List<RainfallInput> inputs)
        {
            const int MAX_LIMIT = 250;

            if (inputs == null || inputs.Count == 0)
            {
                return BadRequest("No data provided");
            }

            if (inputs.Count > MAX_LIMIT)
            {
                _logger.LogWarning("❌ Rainfall limit exceeded: {count}", inputs.Count);
                return BadRequest(new { message = $"Max {MAX_LIMIT} records allowed. You sent {inputs.Count}" });
            }

            _logger.LogInformation("🌧 Rainfall batch request: {count}", inputs.Count);

            try
            {
                var tasks = inputs.Select(x => _rainfallService.PredictAsync(x));
                var results = await Task.WhenAll(tasks);

                return Ok(results);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "⚠️ Invalid input provided for rainfall prediction");
                return BadRequest(new { message = "Invalid input data", details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "❌ Rainfall prediction engine or model is not properly initialized");
                return StatusCode(503, "Prediction service is temporarily unavailable");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ An unexpected error occurred during rainfall prediction");
                return StatusCode(500, "An internal error occurred");
            }
        }

        /// <summary>
        /// Predicts calories burned for a batch of inputs asynchronously.
        /// </summary>
        [HttpPost("calories")]
        public async Task<IActionResult> PredictCalories([FromBody] List<CaloriesInput> inputs)
        {
            const int MAX_LIMIT = 250;

            if (inputs == null || inputs.Count == 0)
            {
                _logger.LogWarning("⚠️ Calories request received with no data");
                return BadRequest("No data provided");
            }

            if (inputs.Count > MAX_LIMIT)
            {
                _logger.LogWarning("❌ Calories limit exceeded: {count}", inputs.Count);
                return BadRequest(new { message = $"Max {MAX_LIMIT} records allowed. You sent {inputs.Count}" });
            }

            _logger.LogInformation("🔥 Calories batch request for {count} records", inputs.Count);

            try
            {
                var tasks = inputs.Select(x => _caloriesService.PredictAsync(x));
                var results = await Task.WhenAll(tasks);

                _logger.LogInformation("✅ Calories prediction success");

                return Ok(new
                {
                    count = results.Length,
                    data = results
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "⚠️ Invalid input provided for calories prediction");
                return BadRequest(new { message = "Invalid input data", details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "❌ Calories prediction service error");
                return StatusCode(503, "Prediction service is temporarily unavailable");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error in calories prediction");
                return StatusCode(500, "An internal error occurred");
            }
        }

        /// <summary>
        /// Check if the calories CSV data exists.
        /// </summary>
        [HttpGet("calories-status")]
        public IActionResult CaloriesStatus()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "CaloriesBurnPrediction.csv");
            bool isDummy = !System.IO.File.Exists(path);

            _logger.LogInformation("📊 Calories data source check: {status}", isDummy ? "Using Dummy Data" : "Using Real CSV");

            return Ok(new { isDummy = isDummy });
        }

        /// <summary>
        /// Predicts laptop price for a batch of inputs asynchronously.
        /// </summary>
        [HttpPost("laptop")]
        public async Task<IActionResult> PredictLaptop([FromBody] List<LaptopInput> inputs)
        {
            const int MAX_LIMIT = 250;

            if (inputs == null || inputs.Count == 0)
            {
                _logger.LogWarning("⚠️ Laptop request received with no data");
                return BadRequest("No data provided");
            }

            if (inputs.Count > MAX_LIMIT)
            {
                _logger.LogWarning("❌ Laptop limit exceeded: {count}", inputs.Count);
                return BadRequest(new { message = $"Max {MAX_LIMIT} records allowed. You sent {inputs.Count}" });
            }

            _logger.LogInformation("💻 Laptop batch request for {count} records", inputs.Count);

            try
            {
                var tasks = inputs.Select(x => _laptopService.PredictAsync(x));
                var results = await Task.WhenAll(tasks);

                _logger.LogInformation("✅ Laptop prediction success");

                return Ok(new
                {
                    count = results.Length,
                    data = results
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "⚠️ Invalid input provided for laptop prediction");
                return BadRequest(new { message = "Invalid input data", details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "❌ Laptop prediction service error");
                return StatusCode(503, "Prediction service is temporarily unavailable");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error in laptop prediction");
                return StatusCode(500, "An internal error occurred");
            }
        }
    }
}