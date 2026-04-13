using Microsoft.ML.Data;
    namespace EvaluationMatricesPOC.Models
{
    public class CaloriesOutput
    {
        [ColumnName("Score")]
        public float PredictedCalories { get; set; }
    }
}
