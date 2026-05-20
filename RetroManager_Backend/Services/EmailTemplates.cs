namespace RetroManager_Backend.Services;

public static class EmailTemplates
{
    private const string Brand  = "#4f46e5";
    private const string Bg     = "#f8fafc";
    private const string Card   = "#ffffff";
    private const string Text   = "#1f2937";
    private const string Muted  = "#6b7280";

    private static string Wrapper(string body) => $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'><title>RetroManager</title></head>
<body style='margin:0;padding:0;background:{Bg};font-family:-apple-system,BlinkMacSystemFont,Segoe UI,Helvetica,Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:{Bg};padding:32px 16px;'>
    <tr><td align='center'>
      <table width='560' cellpadding='0' cellspacing='0' style='background:{Card};border-radius:12px;box-shadow:0 1px 3px rgba(0,0,0,.06);overflow:hidden;max-width:560px;width:100%;'>
        <tr><td style='background:{Brand};padding:20px 32px;color:#fff;'>
          <table cellpadding='0' cellspacing='0'>
            <tr>
              <td style='background:rgba(255,255,255,.2);width:32px;height:32px;border-radius:8px;text-align:center;font-weight:800;font-size:18px;color:#fff;vertical-align:middle;'>R</td>
              <td style='padding-left:10px;font-weight:700;font-size:16px;vertical-align:middle;'>RetroManager</td>
            </tr>
          </table>
        </td></tr>
        <tr><td style='padding:32px;color:{Text};font-size:14px;line-height:1.6;'>
          {body}
        </td></tr>
        <tr><td style='padding:18px 32px;background:{Bg};color:{Muted};font-size:12px;border-top:1px solid #e5e7eb;line-height:1.5;'>
          <strong>Mensagem automática</strong> — por favor não respondas a este email. Esta caixa não é monitorizada.
          <br>RetroManager © {DateTime.UtcNow.Year}
        </td></tr>
      </table>
    </td></tr>
  </table>
</body>
</html>";

    private static string Button(string href, string label) =>
        $@"<a href='{href}' style='display:inline-block;padding:12px 24px;background:{Brand};color:#fff;border-radius:8px;text-decoration:none;font-weight:600;font-size:14px;margin:18px 0;'>{label}</a>";

    public static string Verification(string name, string link) => Wrapper($@"
        <h2 style='margin:0 0 12px;font-size:22px;'>Olá {name}!</h2>
        <p>Bem-vindo ao RetroManager. Para ativar a tua conta, confirma o teu endereço de email clicando no botão:</p>
        {Button(link, "Confirmar Email")}
        <p style='color:{Muted};font-size:12px;'>Se o botão não funcionar, copia este link:<br>{link}</p>
        <p style='color:{Muted};font-size:12px;'>Este link expira em 48 horas.</p>");

    public static string ActionAssigned(string name, string description, string projectName, string retroTitle, DateTime? expected, string link) => Wrapper($@"
        <h2 style='margin:0 0 12px;font-size:20px;'>Olá {name}!</h2>
        <p>Foi-te atribuída uma nova ação no projeto <strong>{projectName}</strong>:</p>
        <div style='background:{Bg};border-left:4px solid {Brand};padding:14px 16px;margin:14px 0;border-radius:4px;'>
          <p style='margin:0;font-weight:600;color:{Text};'>{description}</p>
          <p style='margin:6px 0 0;color:{Muted};font-size:12px;'>Retrospectiva: {retroTitle}</p>
          {(expected.HasValue ? $"<p style='margin:6px 0 0;color:{Muted};font-size:12px;'>Data prevista: <strong>{expected.Value:dd/MM/yyyy}</strong></p>" : "")}
        </div>
        {Button(link, "Ver Retrospectiva")}");

    public static string RetrospectiveClosed(string retroTitle, string projectName) => Wrapper($@"
        <h2 style='margin:0 0 12px;font-size:20px;'>Retrospectiva fechada</h2>
        <p>A retrospectiva <strong>{retroTitle}</strong> do projeto <strong>{projectName}</strong> foi fechada pelo manager.</p>
        <p>A partir de agora não é possível editar tickets, ações ou notas — mas continua acessível para consulta.</p>");

    public static string RoleChanged(string name, string newRole) => Wrapper($@"
        <h2 style='margin:0 0 12px;font-size:20px;'>Olá {name}!</h2>
        <p>O teu role no RetroManager foi atualizado para <strong>{newRole}</strong>.</p>
        <p>Se fizeres login novamente, terás acesso às novas permissões.</p>");

    public static string AddedToProject(string name, string projectName, string link) => Wrapper($@"
        <h2 style='margin:0 0 12px;font-size:20px;'>Olá {name}!</h2>
        <p>Foste adicionado ao projeto <strong>{projectName}</strong>. Já tens acesso às suas retrospectivas e ações.</p>
        {Button(link, "Ver Projetos")}");
}
