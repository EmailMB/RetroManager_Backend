using System.ComponentModel.DataAnnotations;
using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

public class ActionUpdateStatusDto
{
    [Required]
    public ActionStatus Status { get; set; }

    public string? Notes { get; set; }
}
