using EvaluationMatricesPOC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EvaluationMatricesPOC.Services;

namespace EvaluationMatricesPOC.Controllers   
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthApiController(AuthService authService)
        {
            _authService = authService;
        }

        // ✅ NO AUTHORIZE HERE
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(RegisterModel model)
        {
            var result = _authService.Register(model);
            return Ok(result);
        }

        // ✅ NO AUTHORIZE HERE
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            var token = _authService.Login(model);
            return Ok(token);
        }
    }
}