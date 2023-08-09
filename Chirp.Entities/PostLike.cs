using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Entities;

[PrimaryKey(nameof(ProfileId), nameof(PostId))]
public class PostLike {

  [Required]
  public long ProfileId { get; set; }
  public Profile Profile { get; set; }

  [Required]
  public long PostId { get; set; }
  public Post Post { get; set; }

  [Required]
  public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}