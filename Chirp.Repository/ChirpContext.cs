using Chirp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Repository;

public class ChirpContext : DbContext
{

  public ChirpContext(DbContextOptions<ChirpContext> options) : base(options) { }

  public DbSet<Account> Accounts { get; set; }
  public DbSet<Profile> Profiles { get; set; }
  public DbSet<Post> Posts { get; set; }
  public DbSet<Attachment> Attachments { get; set; }

}