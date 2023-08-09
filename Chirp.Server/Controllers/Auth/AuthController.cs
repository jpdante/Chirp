using Chirp.Entities;
using Chirp.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Server.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  private readonly ILogger<AuthController> _logger;
  private readonly ChirpContext _context;
  
  public AuthController(ILogger<AuthController> logger, ChirpContext context)
  {
    _logger = logger;
    _context = context;
  }
  
  [HttpPost("register", Name = "RegisterUser")]
  public async Task<IActionResult> Register([FromBody] RegisterDto model, CancellationToken cToken)
  {
    await using var transaction = await _context.Database.BeginTransactionAsync(cToken);
    try
    {
      if (await _context.Accounts.AnyAsync(x => x.Email == model.Email, cToken))
      {
        return BadRequest("Email already exists");
      }
      
      if (await _context.Profiles.AnyAsync(x => x.Handle == model.Handle, cToken))
      {
        return BadRequest("Handle already exists");
      }
      
      var account = new Account
      {
        Email = model.Email,
        Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
        CreatedAt = DateTime.UtcNow,
        LastUpdatedAt = DateTime.UtcNow
      };
      
      await _context.Accounts.AddAsync(account, cToken);
      await _context.SaveChangesAsync(cToken);
      
      var profile = new Profile
      {
        AccountId = account.AccountId,
        Handle = model.Handle,
        Name = model.Name
      };
      
      await _context.Profiles.AddAsync(profile, cToken);
      await _context.SaveChangesAsync(cToken);
      
      await transaction.CommitAsync(cToken);

      return Ok(new { message = "User registered successfully" });
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync(cToken);
      _logger.LogError(ex, "Error while registering user");
      return BadRequest(new { message = "Error while registering user" });
    }
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