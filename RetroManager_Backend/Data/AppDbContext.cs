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
    /// <summary>
    /// Table for system users and their authentication data.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Table for project details and metadata.
    /// </summary>
    public DbSet<Project> Projects { get; set; }

    /// <summary>
    /// Table for retrospective sessions linked to projects.
    /// </summary>
    public DbSet<Retrospective> Retrospectives { get; set; }

    /// <summary>
    /// Table for feedback tickets (Positive/To Improve).
    /// </summary>
    public DbSet<Ticket> Tickets { get; set; }

    /// <summary>
    /// Table for action items/tasks defined in retrospectives.
    /// </summary>
    public DbSet<ActionItem> Actions { get; set; }

    /// <summary>
    /// Join table for user attendance in retrospective sessions.
    /// </summary>
    public DbSet<RetrospectiveAttendance> Attendances { get; set; }
    #endregion

    /// <summary>
    /// Configures the database schema, keys, and relationships using Fluent API.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Composite Keys
        // Defines the composite primary key for the attendance table (RetrospectiveId + UserId).
        modelBuilder.Entity<RetrospectiveAttendance>()
            .HasKey(ra => new { ra.RetrospectiveId, ra.UserId });
        #endregion

        #region Relationships & Join Tables
        // Configure Many-to-Many relationship between Projects and Users (Members).
        // This creates the 'project_user' join table as defined in the SQL schema.
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Members)
            .WithMany()
            .UsingEntity(j => j.ToTable("project_user"));

        // Configure the One-to-Many relationship for Project Creator.
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Creator)
            .WithMany(u => u.CreatedProjects)
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion

        #region Constraints & Indexes
        // Ensures the User Email is unique in the database.
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        #endregion
    }
}