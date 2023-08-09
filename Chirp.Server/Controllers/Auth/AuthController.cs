using Microsoft.AspNetCore.Mvc;

namespace Chirp.Server.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  [HttpPost("register", Name = "RegisterUser")]
  public async Task<IActionResult> Register([FromBody] RegisterDto model, CancellationToken cToken)
  {
    return Ok();
  }

  [HttpPost("login", Name = "LoginUser")]
  public async Task<IActionResult> Login([FromBody] LoginDto model, CancellationToken cToken)
  {
    return Ok();
  }

  [HttpPost("forgotPassword", Name = "ForgotPassword")]
  public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model, CancellationToken cToken)
  {
    return Ok();
  }
}