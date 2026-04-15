using EvaluationMatricesPOC.Models;
using System.Threading.Tasks;

namespace EvaluationMatricesPOC.Services
{
    /// <summary>
    /// Interface for laptop price prediction service.
    /// </summary>
    public interface ILaptopService
    {
        /// <summary>
        /// Predicts laptop price asynchronously based on specifications.
        /// </summary>
        Task<PredictionResult> PredictAsync(LaptopInput input);
    }
}
