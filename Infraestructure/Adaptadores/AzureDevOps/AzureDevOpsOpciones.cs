using System.ComponentModel.DataAnnotations;

namespace Infraestructure.Adaptadores.AzureDevOps;

/// <summary>Configuración del adaptador. El PAT NUNCA se versiona: user-secrets o variable de entorno.</summary>
public sealed class AzureDevOpsOpciones
{
    public const string Seccion = "AzureDevOps";

    [Required, Url] public string UrlBase { get; set; } = "https://dev.azure.com";
    [Required] public string VersionApi { get; set; } = "7.1";
    [Required] public string Pat { get; set; } = string.Empty;
    [Range(1, 120)] public int TiempoEsperaSegundos { get; set; } = 30;
}
