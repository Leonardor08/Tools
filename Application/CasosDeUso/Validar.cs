using System.Text.RegularExpressions;
using Application.Excepciones;

namespace Application.CasosDeUso;

internal static partial class Validar
{
    // Nombres de organización/proyecto de Azure DevOps: letras, dígitos, espacio, punto, guion y guion bajo.
    [GeneratedRegex(@"^[\p{L}\p{Nd} ._-]{1,64}$")]
    private static partial Regex NombreValido();

    public static void OrganizacionYProyecto(string organizacion, string proyecto)
    {
        if (string.IsNullOrWhiteSpace(organizacion) || !NombreValido().IsMatch(organizacion))
            throw ValidacionException.De("organizacion", "La organización es obligatoria y solo admite letras, números, espacio, '.', '-' o '_'.");
        if (string.IsNullOrWhiteSpace(proyecto) || !NombreValido().IsMatch(proyecto))
            throw ValidacionException.De("proyecto", "El proyecto es obligatorio y solo admite letras, números, espacio, '.', '-' o '_'.");
    }
}
