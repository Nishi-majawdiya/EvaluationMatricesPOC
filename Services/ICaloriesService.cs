using EvaluationMatricesPOC.Models;
using System.Threading.Tasks;

namespace EvaluationMatricesPOC.Services
{
    /// <summary>
    /// Interface for calories prediction service.
    /// </summary>
    public interface ICaloriesService
    {
        /// <summary>
        /// Predicts calories burned asynchronously.
        /// </summary>
        Task<PredictionResult> PredictAsync(CaloriesInput input);
    }
}
