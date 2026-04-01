namespace RetroManager_Backend.DTOs;

/// <summary>
/// Represents a single attendance record returned to the client.
/// </summary>
public class AttendanceResponseDto
{
    /// <summary>ID of the retrospective this record belongs to.</summary>
    public int RetrospectiveId { get; set; }

    /// <summary>ID of the user whose attendance is recorded.</summary>
    public int UserId { get; set; }

    /// <summary>Display name of the user.</summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>Whether the user was present during the session.</summary>
    public bool IsPresent { get; set; }

    /// <summary>ID of the Manager who last updated this record, if any.</summary>
    public int? UpdatedBy { get; set; }

    /// <summary>Timestamp of the last update, if any.</summary>
    public DateTime? UpdatedAt { get; set; }
}
