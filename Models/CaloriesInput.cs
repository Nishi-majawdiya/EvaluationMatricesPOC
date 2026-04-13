using Microsoft.ML.Data;

namespace EvaluationMatricesPOC.Models
{
    public class CaloriesInput
    {
        [LoadColumn(0)]
        public float Weight { get; set; }

        [LoadColumn(1)]
        public float Duration { get; set; }
        [LoadColumn(2)]
        public float HeartRate { get; set; }
        [LoadColumn(3)]

        public string ExerciseType { get; set; }

        [LoadColumn(4)]
        public float CaloriesBurned { get; set; }
    }
}
