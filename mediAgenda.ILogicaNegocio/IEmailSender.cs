namespace mediAgenda.ILogicaNegocio;

public interface IEmailSender
{
    Task EnviarAsync(string destinatario, string asunto, string cuerpo);
}
