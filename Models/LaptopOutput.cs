using Microsoft.ML.Data;

namespace EvaluationMatricesPOC.Models
{
    public class LaptopOutput
    {
        [ColumnName("Score")]
        public float PredictedPrice { get; set; }
    }
}