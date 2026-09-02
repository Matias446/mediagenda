using System.Reflection;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc.Filters;

namespace mediAgenda.WebAPI.Filters;

/// <summary>
/// Sanitiza (quita tags/atributos HTML) todos los strings de los DTOs que llegan
/// a los controllers, salvo los campos de contraseña -- pasarlos por el
/// sanitizador podría alterar la contraseña que el usuario realmente escribió
/// antes de hashearla.
/// </summary>
public class SanitizarInputFilter : IActionFilter
{
    private static readonly HtmlSanitizer Sanitizer = CrearSanitizer();

    private static HtmlSanitizer CrearSanitizer()
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedCssProperties.Clear();
        sanitizer.AllowedSchemes.Clear();
        return sanitizer;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argumento in context.ActionArguments.Values)
        {
            if (argumento == null) continue;
            SanitizarPropiedades(argumento);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    private static void SanitizarPropiedades(object modelo)
    {
        var tipo = modelo.GetType();
        if (tipo.Namespace == null || !tipo.Namespace.StartsWith("mediAgenda")) return;

        foreach (var propiedad in tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (propiedad.PropertyType != typeof(string) || !propiedad.CanRead || !propiedad.CanWrite)
                continue;
            if (propiedad.Name.Contains("password", StringComparison.OrdinalIgnoreCase))
                continue;

            if (propiedad.GetValue(modelo) is string valor && valor.Length > 0)
                propiedad.SetValue(modelo, Sanitizer.Sanitize(valor));
        }
    }
}
