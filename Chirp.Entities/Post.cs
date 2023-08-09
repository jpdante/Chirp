using System.ComponentModel.DataAnnotations;

namespace Chirp.Entities; 

public class Post
{
  [Required, Key]
  public long PostId { get; set; }

  [Required]
  public long ProfileId { get; set; }
  public Profile Profile { get; set; }

  public string? Message { get; set; }

  [Required]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Required]
  public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

  public ICollection<Attachment>? Attachments { get; set; }
}