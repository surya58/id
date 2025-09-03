namespace Data;

/// <summary>
/// Database context for user input parsing system
/// Manages storage of raw user input and parsed structured data from Groq API
/// </summary>
public class UserInputDbContext(DbContextOptions<UserInputDbContext> options) : DbContext(options)
{
    public DbSet<UserInput> UserInputs => Set<UserInput>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserInput>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.RawInput).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.AddressLine).HasMaxLength(300);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.Zip).HasMaxLength(20);
            entity.Property(e => e.Confidence).HasColumnType("decimal(3,2)");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            
            // Add index for common queries
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.FullName);
        });
    }
}
