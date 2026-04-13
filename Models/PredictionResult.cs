namespace EvaluationMatricesPOC.Models
{
    public class PredictionResult
    {
        public float Prediction { get; set; }
        public float RMSE { get; set; }
        public float MAE { get; set; }
        public float R2 { get; set; }
    }
}
