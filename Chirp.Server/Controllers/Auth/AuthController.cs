using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Chirp.Entities;
using Chirp.Repository;
using Chirp.Server.Services;
using IdGen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Chirp.Server.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
  private readonly ILogger<AuthController> _logger;
  private readonly ChirpContext _context;
  private readonly IConfiguration _config;
  private readonly IdGenerator _idGenerator;
  private readonly IMailService _mailService;

  public AuthController(ILogger<AuthController> logger, ChirpContext context, IConfiguration config,
    IdGenerator idGenerator, IMailService mailService)
  {
    _logger = logger;
    _context = context;
    _config = config;
    _idGenerator = idGenerator;
    _mailService = mailService;
  }

  [HttpPost("register", Name = "RegisterUser")]
  [AllowAnonymous]
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
        AccountId = _idGenerator.CreateId(),
        Email = model.Email,
        Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
        CreatedAt = DateTime.UtcNow,
        LastUpdatedAt = DateTime.UtcNow
      };

      await _context.Accounts.AddAsync(account, cToken);
      await _context.SaveChangesAsync(cToken);

      var profile = new Profile
      {
        ProfileId = _idGenerator.CreateId(),
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
  [AllowAnonymous]
  public async Task<IActionResult> Login([FromBody] LoginDto model, CancellationToken cToken)
  {
    await using var transaction = await _context.Database.BeginTransactionAsync(cToken);
    try
    {
      var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == model.Email, cToken);
      if (account == null)
      {
        return BadRequest(new { message = "Email is incorrect" });
      }

      if (!BCrypt.Net.BCrypt.Verify(model.Password, account.Password))
      {
        return BadRequest(new { message = "Password is incorrect" });
      }

      var token = await GenerateTokenAsync(account);

      return Ok(new { token });
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync(cToken);
      _logger.LogError(ex, "Error while logging in user");
      return BadRequest(new { message = "Error while logging in user" });
    }
  }

  private Task<string> GenerateTokenAsync(Account account)
  {
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? ""));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    var claims = new[]
    {
      new Claim("sub", account.AccountId.ToString())
    };

    var token = new JwtSecurityToken(_config["Jwt:Issuer"] ?? "http://localhost:5000",
      _config["Jwt:Audience"] ?? "http://localhost:5000",
      claims,
      expires: DateTime.Now.AddHours(1),
      signingCredentials: credentials);

    return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
  }

  [HttpPost("forgotPassword", Name = "ForgotPassword")]
  [AllowAnonymous]
  public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model, CancellationToken cToken)
  {
    await using var transaction = await _context.Database.BeginTransactionAsync(cToken);
    try
    {
      var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == model.Email, cToken);
      if (account == null)
      {
        return BadRequest(new { message = "Email is incorrect" });
      }

      var forgotPasswordToken = new ForgotPasswordToken
      {
        AccountId = account.AccountId,
        CreatedAt = DateTime.UtcNow
      };

      await _context.ForgotPasswordTokens.AddAsync(forgotPasswordToken, cToken);
      await _context.SaveChangesAsync(cToken);

      var message = new MailMessage();

      message.To.Add(new MailAddress(account.Email));
      message.Subject = "Chirp - Reset Password";
      message.Body = "Please click the following link to reset your password: " +
                     $"{_config["Frontend:Url"]}/resetPassword/{forgotPasswordToken.TokenId.ToString()}";

      await _mailService.SendEmailAsync(message, cToken);
      
      await transaction.CommitAsync(cToken);

      return Ok(new { message = "Password reset email sent" });
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync(cToken);
      _logger.LogError(ex, "Error while sending password reset email");
      return BadRequest(new { message = "Error while sending password reset email" });
    }
  }

  [HttpPost("resetPassword", Name = "ResetPassword")]
  [AllowAnonymous]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model, CancellationToken cToken)
  {
    await using var transaction = await _context.Database.BeginTransactionAsync(cToken);
    try
    {
      var token = await _context.ForgotPasswordTokens.FirstOrDefaultAsync(x => x.TokenId == model.Token, cToken);
      if (token == null)
      {
        return NotFound(new { message = "Token is invalid" });
      }
      
      if (token.CreatedAt.AddHours(1) < DateTime.UtcNow)
      {
        return BadRequest(new { message = "Token has expired" });
      }
      
      var account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == token.AccountId, cToken);
      if (account == null)
      {
        return NotFound(new { message = "Account not found" });
      }

      account.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
      account.LastUpdatedAt = DateTime.UtcNow;
      
      _context.Accounts.Update(account);
      _context.ForgotPasswordTokens.Remove(token);
      await _context.SaveChangesAsync(cToken);

      await transaction.CommitAsync(cToken);

      return Ok(new { message = "Password reset successfully" });
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync(cToken);
      _logger.LogError(ex, "Error while resetting password");
      return BadRequest(new { message = "Error while resetting password" });
    }
  }
}