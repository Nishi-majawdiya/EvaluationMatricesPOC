using Microsoft.ML;
using EvaluationMatricesPOC.Models;
using System.Text.Json;
using System.IO;

public class MLTrainer
{
    public static void TrainRainfallModel()
    {
        var mlContext = new MLContext();

        Console.WriteLine("🌧 Rainfall Training Started...");

        var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "rainfall.csv");

        if (!File.Exists(dataPath))
        {
            Console.WriteLine("❌ Rainfall CSV not found!");
            return;
        }

        var data = mlContext.Data.LoadFromTextFile<RainfallInput>(
            dataPath,
            hasHeader: true,
            separatorChar: ',');

        // 🔥 UPDATED PIPELINE (FASTTREE)
        var pipeline = mlContext.Transforms.Concatenate("Features",
                nameof(RainfallInput.Temperature),
                nameof(RainfallInput.Humidity),
                nameof(RainfallInput.WindSpeed),
                nameof(RainfallInput.Pressure))

            .Append(mlContext.Transforms.NormalizeMinMax("Features"))

            .Append(mlContext.Regression.Trainers.FastTree(
                labelColumnName: nameof(RainfallInput.Rainfall),
                featureColumnName: "Features"));

        var model = pipeline.Fit(data);

        // ✅ Save model
        var modelDir = Path.Combine(Directory.GetCurrentDirectory(), "MLModels");
        Directory.CreateDirectory(modelDir);

        var modelPath = Path.Combine(modelDir, "rainfallModel.zip");
        mlContext.Model.Save(model, data.Schema, modelPath);

        // ✅ Evaluate
        var predictions = model.Transform(data);

        var metrics = mlContext.Regression.Evaluate(
            predictions,
            labelColumnName: nameof(RainfallInput.Rainfall));

        // Save metrics
        var metricsData = new
        {
            RMSE = metrics.RootMeanSquaredError,
            MAE = metrics.MeanAbsoluteError,
            R2 = metrics.RSquared
        };

        var metricsPath = Path.Combine(modelDir, "rainfall_metrics.json");

        File.WriteAllText(metricsPath,
            JsonSerializer.Serialize(metricsData));

        Console.WriteLine("🎉 Rainfall Model trained successfully with FastTree!");
    }
}