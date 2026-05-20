namespace RetroManager_Backend.DTOs;

public class AttendanceResponseDto
{
    public int RetrospectiveId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsPresent { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
