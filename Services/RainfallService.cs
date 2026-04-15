using Microsoft.ML;
using EvaluationMatricesPOC.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace EvaluationMatricesPOC.Services
{
    public class RainfallService : IRainfallService
    {
        private readonly PredictionEngine<RainfallInput, RainfallOutput> _engine;
        private readonly PredictionResult _metrics;

        public RainfallService()
        {
            var mlContext = new MLContext();
            
            // Load model and metrics - in a real app, these paths should be in config
            string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModels", "rainfallModel.zip");
            string metricsPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModels", "rainfall_metrics.json");

            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException("Rainfall ML model file not found", modelPath);
            }

            var model = mlContext.Model.Load(modelPath, out _);
            _engine = mlContext.Model.CreatePredictionEngine<RainfallInput, RainfallOutput>(model);

            if (File.Exists(metricsPath))
            {
                var json = File.ReadAllText(metricsPath);
                _metrics = JsonSerializer.Deserialize<PredictionResult>(json);
            }
        }

        /// <summary>
        /// Predicts rainfall based on the provided input asynchronously.
        /// </summary>
        /// <param name="input">The rainfall input data.</param>
        /// <returns>A prediction result containing the score and metrics.</returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when prediction engine is not initialized.</exception>
        public async Task<PredictionResult> PredictAsync(RainfallInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "Rainfall input cannot be null");
            }

            if (_engine == null)
            {
                throw new InvalidOperationException("Rainfall prediction engine is not initialized");
            }

            return await Task.Run(() =>
            {
                try
                {
                    var result = _engine.Predict(input);

                    return new PredictionResult
                    {
                        Prediction = result.Score,
                        RMSE = _metrics?.RMSE ?? 0,
                        MAE = _metrics?.MAE ?? 0,
                        R2 = _metrics?.R2 ?? 0,
                        Message = "Using CSV data"
                    };
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("An error occurred during rainfall prediction execution", ex);
                }
            });
        }
    }
}