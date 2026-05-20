using Microsoft.EntityFrameworkCore;
using RetroManager_Backend.Data;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Models;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.Services;

public class RetroTemplateService : IRetroTemplateService
{
    private readonly AppDbContext _context;

    public RetroTemplateService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RetroTemplateResponseDto>> GetAccessible(int userId)
    {
        var templates = await _context.RetroTemplates
            .Include(t => t.Creator)
            .Include(t => t.Columns.OrderBy(c => c.DisplayOrder))
            .Where(t => t.IsGlobal || t.CreatedBy == userId)
            .OrderBy(t => t.Name)
            .ToListAsync();

        return templates.Select(t => MapToDto(t, userId));
    }

    public async Task<RetroTemplateResponseDto?> GetById(int id, int userId)
    {
        var template = await _context.RetroTemplates
            .Include(t => t.Creator)
            .Include(t => t.Columns.OrderBy(c => c.DisplayOrder))
            .FirstOrDefaultAsync(t => t.Id == id);
        if (template == null) return null;
        if (!template.IsGlobal && template.CreatedBy != userId) return null;
        return MapToDto(template, userId);
    }

    public async Task<RetroTemplateResponseDto> Create(RetroTemplateCreateDto dto, int userId, UserRole role)
    {
        var template = new RetroTemplate
        {
            Name        = dto.Name,
            Description = dto.Description,
            CreatedBy   = userId,
            IsGlobal    = role == UserRole.Admin && dto.IsGlobal,
            CreatedAt   = DateTime.UtcNow,
            Columns     = dto.Columns.Select((c, i) => new RetroTemplateColumn
            {
                Name         = c.Name,
                Color        = c.Color,
                DisplayOrder = i
            }).ToList()
        };

        _context.RetroTemplates.Add(template);
        await _context.SaveChangesAsync();

        await _context.Entry(template).Reference(t => t.Creator).LoadAsync();
        return MapToDto(template, userId);
    }

    public async Task<RetroTemplateResponseDto?> Update(int id, RetroTemplateUpdateDto dto, int userId, UserRole role)
    {
        var template = await _context.RetroTemplates
            .Include(t => t.Creator)
            .Include(t => t.Columns)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (template == null) return null;
        if (template.CreatedBy != userId && role != UserRole.Admin) return null;

        template.Name        = dto.Name;
        template.Description = dto.Description;
        if (role == UserRole.Admin) template.IsGlobal = dto.IsGlobal;

        _context.RetroTemplateColumns.RemoveRange(template.Columns);
        template.Columns = dto.Columns.Select((c, i) => new RetroTemplateColumn
        {
            Name         = c.Name,
            Color        = c.Color,
            DisplayOrder = i,
            TemplateId   = template.Id
        }).ToList();

        await _context.SaveChangesAsync();
        return MapToDto(template, userId);
    }

    public async Task<bool> Delete(int id, int userId, UserRole role)
    {
        var template = await _context.RetroTemplates.FindAsync(id);
        if (template == null) return false;
        if (template.CreatedBy != userId && role != UserRole.Admin) return false;

        _context.RetroTemplates.Remove(template);
        await _context.SaveChangesAsync();
        return true;
    }

    private static RetroTemplateResponseDto MapToDto(RetroTemplate t, int currentUserId) => new()
    {
        Id            = t.Id,
        Name          = t.Name,
        Description   = t.Description,
        CreatedBy     = t.CreatedBy,
        CreatedByName = t.Creator?.Name,
        IsGlobal      = t.IsGlobal,
        IsOwner       = t.CreatedBy == currentUserId,
        CreatedAt     = t.CreatedAt,
        Columns       = t.Columns.OrderBy(c => c.DisplayOrder).Select(c => new RetroTemplateColumnDto
        {
            Id           = c.Id,
            Name         = c.Name,
            Color        = c.Color,
            DisplayOrder = c.DisplayOrder
        }).ToList()
    };
}
