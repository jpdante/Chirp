using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Entities;

[Index(nameof(AccountId), IsUnique = true)]
public class Token
{
  public enum TokenType
  {
    ResetPassword = 0,
    EmailVerification = 1
  }
  
  [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public Guid TokenId { get; set; }
  
  public TokenType Type { get; set; }
  
  [Required]
  public long AccountId { get; set; }
  public Account Account { get; set; }

  [Required]
  public DateTime CreatedAt { get; set; }
}