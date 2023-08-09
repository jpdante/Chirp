using Chirp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Repository; 

public class ChipContext : DbContext {

  public ChipContext() { }

  public DbSet<Account> Accounts { get; set; }
  public DbSet<Profile> Profiles { get; set; }
  
}