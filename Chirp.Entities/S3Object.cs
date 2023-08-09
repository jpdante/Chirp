using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Entities;

[Index(nameof(ObjectKey), IsUnique = true)]
public class S3Object {

  [Required, Key]
  public long ObjectId { get; set; }

  [Required]
  public string ObjectKey { get; set; } = "";

  public long? AttachmentId { get; set; }
  public Attachment? Attachment { get; set; }

  [Required]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}