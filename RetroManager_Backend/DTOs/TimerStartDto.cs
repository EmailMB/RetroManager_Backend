using System.ComponentModel.DataAnnotations;

namespace RetroManager_Backend.DTOs;

public class TimerStartDto
{
    [Required]
    [Range(10, 7200)]
    public int DurationSeconds { get; set; }
}
