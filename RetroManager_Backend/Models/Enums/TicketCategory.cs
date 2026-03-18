namespace RetroManager_Backend.Models.Enums;

/// <summary>
/// Defines the feedback categories for retrospective tickets.
/// </summary>
public enum TicketCategory
{
    /// <summary>
    /// Positive feedback or highlights of what went well (SQL: Positivo).
    /// </summary>
    Positive = 1,

    /// <summary>
    /// Areas identified for improvement (SQL: Melhorar).
    /// </summary>
    ToImprove = 2
}