﻿using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Entities;

[Index(nameof(Email), IsUnique = true)]
public class Account
{
  [Required, Key]
  public long AccountId { get; set; }

  [Required]
  public bool Active { get; set; }

  [Required]
  public string Password { get; set; }

  [Required]
  [EmailAddress]
  public string Email { get; set; }

  [Required]
  public DateTime CreatedAt { get; set; }

  [Required]
  public DateTime LastUpdatedAt { get; set; }

  public DateTime? LastConfirmedAt { get; set; }

  public DateTime? DeletedAt { get; set; }

  public ICollection<Profile> Profiles { get; }
}