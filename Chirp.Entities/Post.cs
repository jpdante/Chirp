using System.ComponentModel.DataAnnotations;

namespace Chirp.Entities; 

public class Post {

  [Required, Key]
  public long PostId { get; set; }

  public long ProfileId { get; set; }
  public Profile Profile { get; set; }

  public string Message { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

  public ICollection<Attachment>? Attachments { get; set; }
}