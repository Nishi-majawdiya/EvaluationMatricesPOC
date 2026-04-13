using EvaluationMatricesPOC.Models;
using EvaluationMatricesPOC.Data; // ✅ IMPORTANT
using Microsoft.ML;
using System;
using System.IO;

public class MLTrainerCalories
{
    public static void Train()
    {
        var mlContext = new MLContext();

        Console.WriteLine("🔥 Calories Training Started...");

        var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "CaloriesBurnPrediction.csv");

        IDataView data;
        string message;

        // ✅ CSV OR DUMMY
        if (File.Exists(dataPath))
        {
            data = mlContext.Data.LoadFromTextFile<CaloriesInput>(
                path: dataPath,
                hasHeader: true,
                separatorChar: ',');

            message = "✅ Using real CSV dataset";
        }
        else
        {
            Console.WriteLine("⚠ CSV not found → Using dummy data");

            var dummyData = CaloriesDummyData.GetData(); // ✅ CLEAN CALL
            data = mlContext.Data.LoadFromEnumerable(dummyData);

            message = "⚠ Using dummy in-memory dataset";
        }

        Console.WriteLine(message);

        var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

        var pipeline = mlContext.Transforms.Categorical.OneHotEncoding(
                            "ExerciseTypeEncoded", "ExerciseType")

            .Append(mlContext.Transforms.Concatenate("Features",
                "Weight", "Duration", "HeartRate", "ExerciseTypeEncoded"))

            .Append(mlContext.Transforms.NormalizeMinMax("Features"))

            .Append(mlContext.Regression.Trainers.LightGbm(
                labelColumnName: "CaloriesBurned",
                featureColumnName: "Features"));

        var model = pipeline.Fit(split.TrainSet);

        var predictions = model.Transform(split.TestSet);
        var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "CaloriesBurned");

        Console.WriteLine($"✅ RMSE: {metrics.RootMeanSquaredError}");
        Console.WriteLine($"✅ MAE: {metrics.MeanAbsoluteError}");
        Console.WriteLine($"✅ R2: {metrics.RSquared}");

        // SAVE MODEL
        var modelDir = Path.Combine(Directory.GetCurrentDirectory(), "MLModels");
        Directory.CreateDirectory(modelDir);

        var modelPath = Path.Combine(modelDir, "caloriesModel.zip");
        mlContext.Model.Save(model, data.Schema, modelPath);

        // SAVE METRICS
        var metricsPath = Path.Combine(modelDir, "calories_metrics.json");

        var json = $@"{{
    ""RMSE"": {metrics.RootMeanSquaredError},
    ""MAE"": {metrics.MeanAbsoluteError},
    ""R2"": {metrics.RSquared},
    ""DataSource"": ""{message}""
}}";

        File.WriteAllText(metricsPath, json);

        Console.WriteLine("🎉 Training Completed!");
    }
}