using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Models;

namespace RetroManager_Backend.Data;

/// <summary>
/// The main database context that acts as a bridge between C# models and the SQLite database.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    #region DbSets (Tables)
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Retrospective> Retrospectives { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<ActionItem> Actions { get; set; }
    public DbSet<RetrospectiveAttendance> Attendances { get; set; }
    #endregion

    /// <summary>
    /// Configures the database schema and relationships using Fluent API.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Composite Primary Key for RetrospectiveAttendance
        modelBuilder.Entity<RetrospectiveAttendance>()
            .HasKey(ra => new { ra.RetrospectiveId, ra.UserId });

        // Configure N:N relationship between Project and User (Members)
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Members)
            .WithMany(u => u.Attendances.Select(a => a.User).Distinct().ToList() == null ? null : null) // Placeholder for simplicity
            .UsingEntity(j => j.ToTable("project_user"));

        // Enforce unique email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}