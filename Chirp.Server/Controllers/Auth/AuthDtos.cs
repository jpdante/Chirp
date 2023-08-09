using System.ComponentModel.DataAnnotations;
using Chirp.Server.Validators;

namespace Chirp.Server.Controllers.Auth;

public record LoginDto
{
  [EmailAddress(ErrorMessage = "Invalid email address")]
  [Required]
  public string Email { get; set; } = null!;

  [MinLength(6, ErrorMessage = "Password should be at least 6 characters long")]
  [Password]
  [Required]
  public string Password { get; set; } = null!;
}

public record RegisterDto
{
  [MinLength(3, ErrorMessage = "Handle should be at least 3 characters long")]
  [MaxLength(15, ErrorMessage = "Handle should be at maximum 15 characters long")]
  [Required]
  public string Handle { get; set; } = null!;
  
  [MinLength(3, ErrorMessage = "Name should be at least 3 characters long")]
  [MaxLength(50, ErrorMessage = "Name should be at maximum 50 characters long")]
  [Required]
  public string Name { get; set; } = null!;
  
  [EmailAddress(ErrorMessage = "Invalid email address")]
  [Required]
  public string Email { get; set; } = null!;

  [MinLength(6, ErrorMessage = "Password should be at least 6 characters long")]
  [Password]
  [Required]
  public string Password { get; set; } = null!;
}

public record ForgotPasswordDto
{
  [EmailAddress(ErrorMessage = "Invalid email address")]
  [Required]
  public string Email { get; set; } = null!;
}

public record ResetPasswordDto
{
  [Required]
  public Guid Token { get; set; }

  [MinLength(6, ErrorMessage = "Password should be at least 6 characters long")]
  [Password]
  [Required]
  public string Password { get; set; } = null!;
}