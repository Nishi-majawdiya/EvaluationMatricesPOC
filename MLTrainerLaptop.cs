using EvaluationMatricesPOC.Models;
using Microsoft.ML;
using System;
using System.IO;
using System.Linq;

public class MLTrainerLaptop
{
    public static void Train()
    {
        var mlContext = new MLContext();

        Console.WriteLine("💻 Laptop training started...");

        var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "LaptopPricePrediction.csv");
        Console.WriteLine("CSV Path: " + dataPath);

        if (!File.Exists(dataPath))
        {
            Console.WriteLine("❌ Laptop CSV file not found.");
            return;
        }

        var allLines = File.ReadAllLines(dataPath);
        Console.WriteLine("Total lines in CSV: " + allLines.Length);

        if (allLines.Length <= 1)
        {
            Console.WriteLine("❌ CSV has no training rows.");
            return;
        }

        var data = mlContext.Data.LoadFromTextFile<LaptopInput>(
            path: dataPath,
            hasHeader: true,
            separatorChar: ',');

        var rowCount = mlContext.Data
            .CreateEnumerable<LaptopInput>(data, reuseRowObject: false)
            .Count();

        Console.WriteLine("Rows loaded into ML.NET: " + rowCount);

        if (rowCount == 0)
        {
            Console.WriteLine("❌ No rows loaded. Check mapping.");
            return;
        }

        var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

        var trainCount = mlContext.Data
            .CreateEnumerable<LaptopInput>(split.TrainSet, false).Count();

        var testCount = mlContext.Data
            .CreateEnumerable<LaptopInput>(split.TestSet, false).Count();

        Console.WriteLine($"Train rows: {trainCount}");
        Console.WriteLine($"Test rows: {testCount}");

        if (trainCount == 0)
        {
            Console.WriteLine("❌ Training set is empty.");
            return;
        }

        // 🔥 UPDATED PIPELINE (LightGBM)
        var pipeline = mlContext.Transforms.Categorical.OneHotEncoding(
                            "BrandEncoded", nameof(LaptopInput.Brand))
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(
                            "ProcessorEncoded", nameof(LaptopInput.Processor)))

            .Append(mlContext.Transforms.Concatenate(
                            "Features",
                            "BrandEncoded",
                            "ProcessorEncoded",
                            nameof(LaptopInput.RAM),
                            nameof(LaptopInput.Storage),
                            nameof(LaptopInput.ScreenSize)))

            .Append(mlContext.Regression.Trainers.LightGbm(
                            labelColumnName: nameof(LaptopInput.Price),
                            featureColumnName: "Features"));

        var model = pipeline.Fit(split.TrainSet);

        var predictions = model.Transform(split.TestSet);

        var metrics = mlContext.Regression.Evaluate(
            predictions,
            labelColumnName: nameof(LaptopInput.Price));

        Console.WriteLine($"✅ RMSE: {metrics.RootMeanSquaredError}");
        Console.WriteLine($"✅ MAE: {metrics.MeanAbsoluteError}");
        Console.WriteLine($"✅ R2: {metrics.RSquared}");

        var modelDir = Path.Combine(Directory.GetCurrentDirectory(), "MLModels");
        Directory.CreateDirectory(modelDir);

        var modelPath = Path.Combine(modelDir, "laptopModel.zip");
        mlContext.Model.Save(model, data.Schema, modelPath);

        var metricsPath = Path.Combine(modelDir, "laptop_metrics.json");

        var json = $@"{{
  ""RMSE"": {metrics.RootMeanSquaredError},
  ""MAE"": {metrics.MeanAbsoluteError},
  ""R2"": {metrics.RSquared}
}}";

        File.WriteAllText(metricsPath, json);

        Console.WriteLine("🎉 Laptop model trained successfully with LightGBM!");
    }
}