using System.Text.RegularExpressions;

namespace Application.Consultas;

internal static partial class Reglas
{
    // Nombres de organización/proyecto de Azure DevOps: letras, dígitos, espacio, punto, guion y guion bajo.
    [GeneratedRegex(@"^[\p{L}\p{Nd} ._-]{1,64}$")]
    private static partial Regex NombreValido();

    public static IEnumerable<(string, string)> OrganizacionYProyecto(string organizacion, string proyecto)
    {
        if (string.IsNullOrWhiteSpace(organizacion) || !NombreValido().IsMatch(organizacion))
            yield return ("organizacion", "La organización es obligatoria y solo admite letras, números, espacio, '.', '-' o '_'.");
        if (string.IsNullOrWhiteSpace(proyecto) || !NombreValido().IsMatch(proyecto))
            yield return ("proyecto", "El proyecto es obligatorio y solo admite letras, números, espacio, '.', '-' o '_'.");
    }

    public static IEnumerable<(string, string)> WorkItem(int workItemId)
    {
        if (workItemId <= 0) yield return ("workItemId", "El identificador del work item debe ser positivo.");
    }
}
