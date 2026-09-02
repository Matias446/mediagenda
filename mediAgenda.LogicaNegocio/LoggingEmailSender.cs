using mediAgenda.ILogicaNegocio;
using Microsoft.Extensions.Logging;

namespace mediAgenda.LogicaNegocio;

/// <summary>
/// Implementación temporal mientras no haya un proveedor de email configurado
/// (SMTP, SendGrid, etc.). Loguea el contenido en vez de enviarlo. Reemplazar
/// por una implementación real de IEmailSender y registrarla en Program.cs
/// cuando haya credenciales de un proveedor.
/// </summary>
public class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        _logger = logger;
    }

    public Task EnviarAsync(string destinatario, string asunto, string cuerpo)
    {
        _logger.LogWarning(
            "EMAIL NO ENVIADO (sin proveedor configurado). Para: {Destinatario} | Asunto: {Asunto}\n{Cuerpo}",
            destinatario, asunto, cuerpo);
        return Task.CompletedTask;
    }
}
