using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public ProjectService(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<IEnumerable<ProjectResponseDto>> GetAll(int userId, UserRole role)
    {
        var query = _context.Projects
            .Include(p => p.Members)
            .Include(p => p.Retrospectives)
            .AsQueryable();

        if (role != UserRole.Admin)
            query = query.Where(p => p.Members.Any(m => m.Id == userId));

        var projects = await query.ToListAsync();
        return projects.Select(MapToDto);
    }

    public async Task<ProjectResponseDto?> GetById(int projectId, int userId, UserRole role)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .Include(p => p.Retrospectives)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return null;

        if (role != UserRole.Admin && !project.Members.Any(m => m.Id == userId))
            return null;

        return MapToDto(project);
    }

    public async Task<ProjectResponseDto> Create(ProjectCreateDto dto, int creatorId)
    {
        var creator = await _context.Users.FindAsync(creatorId);

        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedBy = creatorId,
            CreatedAt = DateTime.UtcNow
        };

        project.Members.Add(creator!);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        await _context.Entry(project).Collection(p => p.Members).LoadAsync();
        await _context.Entry(project).Collection(p => p.Retrospectives).LoadAsync();

        return MapToDto(project);
    }

    public async Task<bool> Update(int projectId, ProjectUpdateDto dto, int updatorId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null) return false;

        project.Name = dto.Name;
        project.Description = dto.Description;
        project.UpdatedBy = updatorId;
        project.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AddMemberResult> AddMember(int projectId, int userId)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return AddMemberResult.ProjectNotFound;

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return AddMemberResult.UserNotFound;

        if (project.Members.Any(m => m.Id == userId))
            return AddMemberResult.AlreadyMember;

        project.Members.Add(user);
        await _context.SaveChangesAsync();

        _ = _emailService.SendAddedToProjectEmail(user.Email, user.Name, project.Name);

        return AddMemberResult.Success;
    }

    public async Task<RemoveMemberResult> RemoveMember(int projectId, int userId)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return RemoveMemberResult.ProjectNotFound;

        var user = project.Members.FirstOrDefault(m => m.Id == userId);
        if (user == null) return RemoveMemberResult.NotAMember;

        project.Members.Remove(user);
        await _context.SaveChangesAsync();

        return RemoveMemberResult.Success;
    }

    private static ProjectResponseDto MapToDto(Project p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        CreatedBy = p.CreatedBy,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt,
        Members = p.Members.Select(m => new ProjectMemberDto
        {
            UserId = m.Id,
            Name = m.Name,
            Email = m.Email
        }).ToList(),
        Retrospectives = p.Retrospectives.Select(r => new ProjectRetrospectiveSummaryDto
        {
            Id = r.Id,
            Title = r.Title,
            Date = r.Date
        }).ToList()
    };
}
