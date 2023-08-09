using System.ComponentModel.DataAnnotations;

namespace Chirp.Entities;

public class Attachment
{

  [Required]
  [Key]
  public long AttachmentId { get; set; }

  [Required]
  public long PostId { get; set; }
  public Post? Post { get; set; }

  [Required]
  public AttachmentType AttachmentType { get; set; }

  [Required]
  public string AttachmentData { get; set; } = "{}";

  [Required]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}