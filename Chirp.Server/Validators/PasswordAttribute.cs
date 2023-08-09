using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Chirp.Server.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed partial class PasswordAttribute : ValidationAttribute
{
  private const string Mask = "/[\\d|[a-z]|[A-Z]/g";
  
  public PasswordAttribute(string? errorMessage = null)
  {
    ErrorMessage = errorMessage ?? "Password must contain at least one digit, one lowercase, one uppercase and be at least 6 characters long";
  }

  public override bool IsValid(object? value)
  {
    if (value is not string password) return false;

    if (!HasDigitRegex().Match(password).Success) return false;

    if (!HasLowercaseRegex().Match(password).Success) return false;

    if (!HasUppercaseRegex().Match(password).Success) return false;

    return true;
  }
  
  public override string FormatErrorMessage(string name)
  {
    return string.Format(CultureInfo.CurrentCulture,
      ErrorMessageString, name, Mask);
  }

  [GeneratedRegex("\\d+")]
  private static partial Regex HasDigitRegex();

  [GeneratedRegex("[a-z]")]
  private static partial Regex HasLowercaseRegex();

  [GeneratedRegex("[A-Z]")]
  private static partial Regex HasUppercaseRegex();
}