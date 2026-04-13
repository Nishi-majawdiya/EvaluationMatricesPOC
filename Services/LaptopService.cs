using EvaluationMatricesPOC.Models;
using Microsoft.ML;
using System.IO;
using System.Text.Json;

public class LaptopService
{
    private readonly MLContext _mlContext;

    public LaptopService()
    {
        _mlContext = new MLContext();
    }

    public object Predict(LaptopInput input)
    {
        var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModels", "laptopModel.zip");

        if (!File.Exists(modelPath))
        {
            throw new Exception("Laptop model not found! Train first.");
        }

        var model = _mlContext.Model.Load(modelPath, out var schema);
        var engine = _mlContext.Model.CreatePredictionEngine<LaptopInput, LaptopOutput>(model);

        var prediction = engine.Predict(input);

        var metricsPath = Path.Combine(Directory.GetCurrentDirectory(), "MLModels", "laptop_metrics.json");
        var json = File.ReadAllText(metricsPath);
        var doc = JsonDocument.Parse(json);

        return new
        {
            prediction = prediction.PredictedPrice,
            rmse = doc.RootElement.GetProperty("RMSE").GetDouble(),
            mae = doc.RootElement.GetProperty("MAE").GetDouble(),
            r2 = doc.RootElement.GetProperty("R2").GetDouble()
        };
    }
}