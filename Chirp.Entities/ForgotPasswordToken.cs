using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Entities;

[Index(nameof(AccountId), IsUnique = true)]
public class ForgotPasswordToken
{
  [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public Guid TokenId { get; set; }
  
  [Required]
  public long AccountId { get; set; }
  public Account Account { get; set; }

  [Required]
  public DateTime CreatedAt { get; set; }
}