using EvaluationMatricesPOC.Models;
using Microsoft.ML;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace EvaluationMatricesPOC.Services
{
    public class LaptopService : ILaptopService
    {
        private readonly MLContext _mlContext;

        public LaptopService()
        {
            _mlContext = new MLContext();
        }

        /// <summary>
        /// Predicts laptop price based on specifications asynchronously.
        /// </summary>
        /// <param name="input">The laptop specifications.</param>
        /// <returns>A prediction result containing the predicted price and metrics.</returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the ML model or metrics file is missing.</exception>
        /// <exception cref="InvalidOperationException">Thrown when an error occurs during prediction.</exception>
        public async Task<PredictionResult> PredictAsync(LaptopInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "Laptop input cannot be null");
            }

            var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModels", "laptopModel.zip");
            var metricsPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModels", "laptop_metrics.json");

            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException("Laptop ML model file not found", modelPath);
            }

            if (!File.Exists(metricsPath))
            {
                throw new FileNotFoundException("Laptop metrics file not found", metricsPath);
            }

            return await Task.Run(() =>
            {
                try
                {
                    var model = _mlContext.Model.Load(modelPath, out _);
                    var engine = _mlContext.Model.CreatePredictionEngine<LaptopInput, LaptopOutput>(model);

                    var prediction = engine.Predict(input);

                    var json = File.ReadAllText(metricsPath);
                    using (var doc = JsonDocument.Parse(json))
                    {
                        var rmse = doc.RootElement.GetProperty("RMSE").GetDouble();
                        var mae = doc.RootElement.GetProperty("MAE").GetDouble();
                        var r2 = doc.RootElement.GetProperty("R2").GetDouble();

                        return new PredictionResult
                        {
                            Prediction = prediction.PredictedPrice,
                            RMSE = (float)rmse,
                            MAE = (float)mae,
                            R2 = (float)r2,
                            Message = "Prediction successful"
                        };
                    }
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException("Failed to parse laptop metrics file", ex);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("An error occurred during laptop price prediction", ex);
                }
            });
        }
    }
}