using RetroManager_Backend.Models;

namespace RetroManager_Backend.Services;

public interface IEmailService
{
    Task SendVerificationEmail(string toEmail, string name, string token);
    Task SendActionAssignedEmail(string toEmail, string name, ActionItem action, string projectName, string retroTitle);
    Task SendRetrospectiveClosedEmail(IEnumerable<string> emails, string retroTitle, string projectName);
    Task SendRoleChangedEmail(string toEmail, string name, string newRole);
    Task SendAddedToProjectEmail(string toEmail, string name, string projectName);
}
