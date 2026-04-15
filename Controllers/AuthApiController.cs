using EvaluationMatricesPOC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EvaluationMatricesPOC.Services;
using System.Threading.Tasks;

namespace EvaluationMatricesPOC.Controllers   
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthApiController(IAuthService authService)
        {
            _authService = authService;
        }

        // NO AUTHORIZE HERE
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var result = await _authService.RegisterAsync(model);
            return Ok(result);
        }

        // ✅ NO AUTHORIZE HERE
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var token = await _authService.LoginAsync(model);
            return Ok(token);
        }
    }
}