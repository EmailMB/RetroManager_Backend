namespace RetroManager_Backend.Models.Enums;

/// <summary>
/// Represents the current execution state of a defined action item.
/// </summary>
public enum ActionStatus
{
    /// <summary>
    /// The action has been created but work hasn't started.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// The action is currently being addressed.
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// The action has been successfully resolved.
    /// </summary>
    Complete = 3
}