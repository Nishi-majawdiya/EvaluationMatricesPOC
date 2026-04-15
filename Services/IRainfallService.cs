using EvaluationMatricesPOC.Models;
using System.Threading.Tasks;

namespace EvaluationMatricesPOC.Services
{
    /// <summary>
    /// Interface for rainfall prediction service.
    /// </summary>
    public interface IRainfallService
    {
        /// <summary>
        /// Predicts rainfall based on the provided input asynchronously.
        /// </summary>
        Task<PredictionResult> PredictAsync(RainfallInput input);
    }
}
