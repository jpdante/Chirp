using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Entities;

[Index(nameof(Handle), IsUnique = true)]
public class Profile
{

  [Required, Key]
  public long ProfileId { get; set; }

  public long AccountId { get; set; }
  public Account? Account { get; set; }

  [Required]
  [MinLength(3)]
  [MaxLength(15)]
  public string Handle { get; set; } = "";

  [Required]
  [MinLength(3)]
  [MaxLength(50)]
  public string Name { get; set; } = "";

  public string? Biography { get; set; }

  public string? ProfilePicture { get; set; }

  public string? BackgroundPicture { get; set; }

  public ICollection<Post>? Posts { get; set; }

}