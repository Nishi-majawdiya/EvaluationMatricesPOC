using Microsoft.ML;
using EvaluationMatricesPOC.Models;
using System.Text.Json;

namespace EvaluationMatricesPOC.Services
{
    public class RainfallService
    {
        private readonly PredictionEngine<RainfallInput, RainfallOutput> _engine;
        private readonly PredictionResult _metrics;

        public RainfallService()
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load("MLModels/rainfallModel.zip", out _);

            _engine = mlContext.Model.CreatePredictionEngine<RainfallInput, RainfallOutput>(model);

            var json = File.ReadAllText("MLModels/rainfall_metrics.json");
            _metrics = JsonSerializer.Deserialize<PredictionResult>(json);
        }

        public PredictionResult Predict(RainfallInput input)
        {
            var result = _engine.Predict(input);

            return new PredictionResult
            {
                Prediction = result.Score,
                RMSE = _metrics.RMSE,
                MAE = _metrics.MAE,
                R2 = _metrics.R2,
                Message = "Using CSV data"
            };
        }
    }
}