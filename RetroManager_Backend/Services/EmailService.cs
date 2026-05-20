using System.Net;
using System.Net.Mail;
using RetroManager_Backend.Models;

namespace RetroManager_Backend.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    private bool Enabled => _config.GetValue<bool>("EmailSettings:Enabled");
    private string BaseUrl => _config["EmailSettings:BaseUrl"] ?? "http://localhost:5173";
    private string SenderEmail => _config["EmailSettings:SenderEmail"] ?? "";
    private string SenderName  => _config["EmailSettings:SenderName"] ?? "RetroManager";

    public async Task SendVerificationEmail(string toEmail, string name, string token)
    {
        var link = $"{BaseUrl}/verify-email?token={Uri.EscapeDataString(token)}";

        if (!Enabled)
        {
            _logger.LogInformation("Verification link for {Email}: {Link}", toEmail, link);
            return;
        }

        var html = EmailTemplates.Verification(name, link);
        await SendAsync(toEmail, "Confirma a tua conta RetroManager", html);
    }

    public async Task SendActionAssignedEmail(string toEmail, string name, ActionItem action, string projectName, string retroTitle)
    {
        var link = $"{BaseUrl}/retrospectives/{action.RetrospectiveId}";
        var html = EmailTemplates.ActionAssigned(name, action.Description, projectName, retroTitle, action.ExpectedCompletionDate, link);
        await SendAsync(toEmail, $"Nova ação atribuída — {projectName}", html);
    }

    public async Task SendRetrospectiveClosedEmail(IEnumerable<string> emails, string retroTitle, string projectName)
    {
        var html = EmailTemplates.RetrospectiveClosed(retroTitle, projectName);
        foreach (var email in emails.Where(e => !string.IsNullOrWhiteSpace(e)))
            await SendAsync(email, $"Retrospectiva fechada — {retroTitle}", html);
    }

    public async Task SendRoleChangedEmail(string toEmail, string name, string newRole)
    {
        var html = EmailTemplates.RoleChanged(name, newRole);
        await SendAsync(toEmail, "O teu role foi atualizado", html);
    }

    public async Task SendAddedToProjectEmail(string toEmail, string name, string projectName)
    {
        var link = $"{BaseUrl}/projects";
        var html = EmailTemplates.AddedToProject(name, projectName, link);
        await SendAsync(toEmail, $"Foste adicionado ao projeto {projectName}", html);
    }

    private async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        if (!Enabled)
        {
            _logger.LogInformation("[EMAIL OFF] Para: {Email} | Assunto: {Subject}", toEmail, subject);
            return;
        }

        try
        {
            var host = _config["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var port = _config.GetValue<int>("EmailSettings:SmtpPort", 587);
            var user = _config["EmailSettings:SmtpUsername"] ?? SenderEmail;
            var pass = _config["EmailSettings:SenderPassword"] ?? "";

            using var smtp = new SmtpClient(host, port)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(user, pass)
            };

            using var msg = new MailMessage
            {
                From = new MailAddress(SenderEmail, SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            msg.To.Add(toEmail);

            await smtp.SendMailAsync(msg);
            _logger.LogInformation("Email enviado para {Email}: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar email para {Email}", toEmail);
        }
    }
}
