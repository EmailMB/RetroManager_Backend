using RetroManager_Backend.Models.Enums;

namespace RetroManager_Backend.DTOs;

/// <summary>
/// Represents a ticket returned to the client.
/// The author identity is intentionally omitted to preserve anonymity (RF18).
/// IsOwner is a safe computed flag so the UI can render edit/delete controls.
/// </summary>
public class TicketResponseDto
{
    /// <summary>Unique ticket identifier.</summary>
    public int Id { get; set; }

    /// <summary>Feedback text content.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Ticket category value (Positive | ToImprove).</summary>
    public TicketCategory Category { get; set; }

    /// <summary>Human-readable category label.</summary>
    public string CategoryLabel => Category == TicketCategory.Positive ? "Positivo" : "A Melhorar";

    /// <summary>ID of the retrospective this ticket belongs to.</summary>
    public int RetrospectiveId { get; set; }

    /// <summary>
    /// True when the authenticated user is the author of this ticket.
    /// Computed server-side so the UI can show edit/delete controls
    /// without exposing the actual author identity.
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>Timestamp of ticket creation.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Timestamp of the last update, if any.</summary>
    public DateTime? UpdatedAt { get; set; }
}
