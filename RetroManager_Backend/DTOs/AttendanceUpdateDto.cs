using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

public class AttendanceUpdateDto
{
    [Required]
    public bool IsPresent { get; set; }
}
