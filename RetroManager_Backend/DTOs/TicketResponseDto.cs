namespace RetroManager_Backend.DTOs;

public class TicketResponseDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int RetroColumnId { get; set; }
    public int RetrospectiveId { get; set; }

    public bool IsOwner { get; set; }
    public int VoteCount { get; set; }
    public bool HasVoted { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
