using EvaluationMatricesPOC.Models;
using System.Threading.Tasks;

namespace EvaluationMatricesPOC.Services
{
    /// <summary>
    /// Interface for authentication service.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user asynchronously.
        /// </summary>
        Task<object> RegisterAsync(RegisterModel model);

        /// <summary>
        /// Authenticates a user and returns a token asynchronously.
        /// </summary>
        Task<object> LoginAsync(LoginModel model);
    }
}
