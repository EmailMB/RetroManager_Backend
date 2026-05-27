using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroManager_Backend.DTOs;
using RetroManager_Backend.Services;

namespace RetroManager_Backend.Controllers;

[ApiController]
[Route("api/retro-templates")]
[Authorize]
public class RetroTemplatesController : BaseController
{
    private readonly IRetroTemplateService _templateService;

    public RetroTemplatesController(IRetroTemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RetroTemplateResponseDto>>> GetAccessible()
    {
        var (userId, _) = GetCaller();
        var templates = await _templateService.GetAccessible(userId);
        return Ok(templates);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroTemplateResponseDto>> GetById(int id)
    {
        var (userId, _) = GetCaller();
        var template = await _templateService.GetById(id, userId);
        if (template == null) return NotFound();
        return Ok(template);
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetroTemplateResponseDto>> Create(RetroTemplateCreateDto dto)
    {
        var (userId, role) = GetCaller();
        var created = await _templateService.Create(dto, userId, role);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<RetroTemplateResponseDto>> Update(int id, RetroTemplateUpdateDto dto)
    {
        var (userId, role) = GetCaller();
        var updated = await _templateService.Update(id, dto, userId, role);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var (userId, role) = GetCaller();
        var success = await _templateService.Delete(id, userId, role);
        if (!success) return NotFound();
        return NoContent();
    }
}
