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
  
  /// <summary>
  /// Registers a user, pending email verification
  /// </summary>
  /// <response code="200">Registration successful</response>
  /// <response code="400">Email or handle already exists</response>
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

      var verifyAccountToken = new Token
      {
        AccountId = account.AccountId,
        CreatedAt = DateTime.UtcNow,
        Type = Token.TokenType.EmailVerification
      };

      await _context.Tokens.AddAsync(verifyAccountToken, cToken);
      await _context.SaveChangesAsync(cToken);

      var message = new MailMessage();
      message.To.Add(new MailAddress(account.Email));
      message.Subject = "Welcome to Chirp - Confirm your email";
      message.Body = $"Hello {profile.Name}!\n\n" +
                     $"Please confirm your email by clicking this link: " +
                     $"{_config["Frontend:Url"]}/confirm-email/{verifyAccountToken.TokenId}";
      await _mailService.SendEmailAsync(message, cToken);

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
  
  /// <summary>
  /// Confirms a user's email address
  /// </summary>
  /// <response code="200">Email confirmed successfully</response>
  /// <response code="400">Token is invalid</response>
  [HttpPost("confirmEmail", Name = "ConfirmEmail")]
  [AllowAnonymous]
  public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto model, CancellationToken cToken)
  {
    await using var transaction = await _context.Database.BeginTransactionAsync(cToken);
    try
    {
      var token = await _context.Tokens.FirstOrDefaultAsync(x => x.TokenId == model.Token, cToken);
      if (token == null)
      {
        return BadRequest(new { message = "Token is invalid" });
      }

      if (token.Type != Token.TokenType.EmailVerification)
      {
        return BadRequest(new { message = "Token is invalid" });
      }

      var account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == token.AccountId, cToken);
      if (account == null)
      {
        return BadRequest(new { message = "Token is invalid" });
      }

      var now = DateTime.UtcNow;
      account.Active = true;
      account.LastUpdatedAt = now;
      account.LastConfirmedAt = now;
      
      _context.Accounts.Update(account);
      await _context.SaveChangesAsync(cToken);
      
      _context.Tokens.Remove(token);

      await transaction.CommitAsync(cToken);

      return Ok(new { message = "Email confirmed successfully" });
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync(cToken);
      _logger.LogError(ex, "Error while confirming email");
      return BadRequest(new { message = "Error while confirming email" });
    }
  }

  /// <summary>
  /// Logs user in for 1 hour, returning a JWT Bearer token
  /// </summary>
  /// <response code="200">Login successful</response>
  /// <response code="400">Email or password is incorrect</response>
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

  /// <summary>
  /// Sends a password reset email to the user
  /// </summary>
  /// <response code="200">Email sent successfully</response>
  /// <response code="400">Email is incorrect</response>
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

      var forgotPasswordToken = new Token
      {
        AccountId = account.AccountId,
        CreatedAt = DateTime.UtcNow,
        Type = Token.TokenType.ResetPassword
      };

      await _context.Tokens.AddAsync(forgotPasswordToken, cToken);
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

  /// <summary>
  /// Resets the user's password with the provided token
  /// </summary>
  /// <response code="200">Password reset successfully</response>
  /// <response code="400">Token is invalid or has expired</response>
  [HttpPost("resetPassword", Name = "ResetPassword")]
  [AllowAnonymous]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model, CancellationToken cToken)
  {
    await using var transaction = await _context.Database.BeginTransactionAsync(cToken);
    try
    {
      var token = await _context.Tokens.FirstOrDefaultAsync(x => x.TokenId == model.Token, cToken);
      if (token == null)
      {
        return BadRequest(new { message = "Token is invalid" });
      }

      if (token.CreatedAt.AddHours(1) < DateTime.UtcNow)
      {
        return BadRequest(new { message = "Token has expired" });
      }

      var account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == token.AccountId, cToken);
      if (account == null)
      {
        return BadRequest(new { message = "Account not found" });
      }

      account.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
      account.LastUpdatedAt = DateTime.UtcNow;

      _context.Accounts.Update(account);
      _context.Tokens.Remove(token);
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