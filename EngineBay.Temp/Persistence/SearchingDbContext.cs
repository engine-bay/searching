using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using SearchingTemp.Entities;

namespace SearchingTemp.Persistence
{
  public class SearchingDbContext : DbContext
  {
    public SearchingDbContext(DbContextOptions<SearchingDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<PiiVector> PiiVectors { get; set; }
    public DbSet<Vocabulary> Vocabularies { get; set; }
    public DbSet<MinHash> MinHashFunction { get; set; }
    public DbSet<SearchResult> SearchResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Person>().HasKey(p => p.Id);
      modelBuilder.Entity<Person>().Property(p => p.Id)
        .ValueGeneratedOnAdd();
      modelBuilder.Entity<Person>().Ignore(p => p.ShingleSet);
      modelBuilder.Entity<Person>().Ignore(p => p.OneHotVector);


      modelBuilder.Entity<PiiVector>().HasKey(p => p.Id);
      modelBuilder.Entity<PiiVector>().Property(p => p.Id)
        .ValueGeneratedOnAdd();
      modelBuilder.Entity<PiiVector>()
        .HasOne(p => p.Person)
        .WithMany(s => s.PiiVectors)
        .HasForeignKey(p => p.PersonId);

      modelBuilder.Entity<Vocabulary>().HasKey(p => p.Id);
      modelBuilder.Entity<Vocabulary>().Property(p => p.Id)
        .ValueGeneratedOnAdd();

      modelBuilder.Entity<MinHash>().HasKey(p => p.Id);
      modelBuilder.Entity<MinHash>().Property(p => p.Id)
        .ValueGeneratedOnAdd();
      
      modelBuilder.Entity<SearchResult>().HasNoKey();
    }
  }
}