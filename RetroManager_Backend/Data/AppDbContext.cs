using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Models;

namespace RetroManager_Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Retrospective> Retrospectives { get; set; }
    public DbSet<RetroColumn> RetroColumns { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<ActionItem> Actions { get; set; }
    public DbSet<RetrospectiveAttendance> Attendances { get; set; }
    public DbSet<RetroTemplate> RetroTemplates { get; set; }
    public DbSet<RetroTemplateColumn> RetroTemplateColumns { get; set; }
    public DbSet<TicketVote> TicketVotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RetrospectiveAttendance>()
            .HasKey(ra => new { ra.RetrospectiveId, ra.UserId });

        modelBuilder.Entity<Project>()
            .HasMany(p => p.Members)
            .WithMany()
            .UsingEntity(j => j.ToTable("project_user"));

        modelBuilder.Entity<Project>()
            .HasOne(p => p.Creator)
            .WithMany(u => u.CreatedProjects)
            .HasForeignKey(p => p.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<RetroColumn>()
            .HasOne(c => c.Retrospective)
            .WithMany(r => r.Columns)
            .HasForeignKey(c => c.RetrospectiveId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.RetroColumn)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.RetroColumnId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RetroTemplateColumn>()
            .HasOne(c => c.Template)
            .WithMany(t => t.Columns)
            .HasForeignKey(c => c.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TicketVote>()
            .HasKey(v => new { v.TicketId, v.UserId });

        modelBuilder.Entity<TicketVote>()
            .HasOne(v => v.Ticket)
            .WithMany(t => t.Votes)
            .HasForeignKey(v => v.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
