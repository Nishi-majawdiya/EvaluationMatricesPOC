using EvaluationMatricesPOC.Models;
using Microsoft.ML;
using System.Data.Common;
using System.IO;
using System.Text.Json;

public class CaloriesService
{
    private readonly MLContext _mlContext;

    public CaloriesService()
    {
        _mlContext = new MLContext();
    }

    public object Predict(CaloriesInput input)
    {
        // ✅ Load model only when needed
        var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModels", "caloriesModel.zip");

        if (!File.Exists(modelPath))
        {
            throw new Exception("Calories model not found! Train the model first.");
        }

        var model = _mlContext.Model.Load(modelPath, out var schema);
        var engine = _mlContext.Model.CreatePredictionEngine<CaloriesInput, CaloriesOutput>(model);

        var prediction = engine.Predict(input);

        // Metrics
        var metricsPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModels", "calories_metrics.json");
        var json = File.ReadAllText(metricsPath);
        var doc = JsonDocument.Parse(json);

        var rmse = doc.RootElement.GetProperty("RMSE").GetDouble();
        var mae = doc.RootElement.GetProperty("MAE").GetDouble();
        var r2 = doc.RootElement.GetProperty("R2").GetDouble();
        var dataSource = doc.RootElement.GetProperty("DataSource").GetString();

        return new
        {
            prediction = prediction.PredictedCalories,
            rmse = rmse,
            mae = mae,
            r2 = r2,
             message = dataSource
        };
    }
}