using System.Text.Json;
using System.Text.RegularExpressions;
using Application.Puertos.Salida;
using Domain.Auditoria;
using Domain.Trazabilidad;
using Microsoft.Extensions.Options;

namespace Infraestructure.Adaptadores.Persistencia;

public sealed class AlmacenCortesOpciones
{
    public const string Seccion = "AlmacenCortes";
    public string Carpeta { get; set; } = "datos/cortes";
}

/// <summary>
/// Adaptador de persistencia del lado de escritura: un documento JSON por corte, agrupado por
/// work item. Suficiente para el MVP; al ser un puerto, puede cambiarse por SQL Server o
/// Cosmos DB sin tocar Application ni Domain.
/// </summary>
internal sealed partial class AlmacenCortesArchivoJson(IOptions<AlmacenCortesOpciones> opciones) : ICortesTrazabilidadRepositorio
{
    private static readonly JsonSerializerOptions Json = new() { WriteIndented = true };
    private static readonly SemaphoreSlim Escritura = new(1, 1);
    private readonly string _raiz = Path.GetFullPath(opciones.Value.Carpeta);

    public async Task GuardarAsync(CorteTrazabilidad corte, CancellationToken ct)
    {
        string carpeta = CarpetaDe(corte.Organizacion, corte.Proyecto, corte.WorkItemId);
        Directory.CreateDirectory(carpeta);
        string destino = Path.Combine(carpeta, $"{corte.Id}.json");

        await Escritura.WaitAsync(ct);
        try
        {
            // Escritura atómica: archivo temporal + renombrado, para no dejar documentos a medias.
            string temporal = destino + ".tmp";
            await File.WriteAllTextAsync(temporal, JsonSerializer.Serialize(Documento.Desde(corte), Json), ct);
            File.Move(temporal, destino, overwrite: false);
        }
        finally { Escritura.Release(); }
    }

    public async Task<CorteTrazabilidad?> ObtenerAsync(Guid id, CancellationToken ct)
    {
        if (!Directory.Exists(_raiz)) return null;
        string? ruta = Directory.EnumerateFiles(_raiz, $"{id}.json", SearchOption.AllDirectories).FirstOrDefault();
        return ruta is null ? null : await LeerAsync(ruta, ct);
    }

    public async Task<IReadOnlyList<CorteTrazabilidad>> ListarPorWorkItemAsync(string organizacion, string proyecto, int workItemId, CancellationToken ct)
    {
        string carpeta = CarpetaDe(organizacion, proyecto, workItemId);
        if (!Directory.Exists(carpeta)) return [];
        var cortes = new List<CorteTrazabilidad>();
        foreach (var ruta in Directory.EnumerateFiles(carpeta, "*.json"))
            cortes.Add(await LeerAsync(ruta, ct));
        return cortes;
    }

    private static async Task<CorteTrazabilidad> LeerAsync(string ruta, CancellationToken ct)
    {
        await using var flujo = File.OpenRead(ruta);
        var doc = await JsonSerializer.DeserializeAsync<Documento>(flujo, Json, ct)
                  ?? throw new InvalidDataException($"Documento de corte vacío: {ruta}");
        return doc.AEntidad();
    }

    private string CarpetaDe(string organizacion, string proyecto, int workItemId) =>
        Path.Combine(_raiz, Seguro(organizacion), Seguro(proyecto), workItemId.ToString());

    [GeneratedRegex(@"[^\p{L}\p{Nd}._-]")]
    private static partial Regex CaracteresNoSeguros();
    private static string Seguro(string nombre) => CaracteresNoSeguros().Replace(nombre.Trim(), "_");

    // Modelo de persistencia (separado del agregado de dominio).
    private sealed record Documento(Guid Id, string Organizacion, string Proyecto, int WorkItemId, DateTime FechaCorteUtc,
        string Motivo, string Huella, List<DocumentoPeriodo> Periodos)
    {
        public static Documento Desde(CorteTrazabilidad c) => new(c.Id, c.Organizacion, c.Proyecto, c.WorkItemId,
            c.FechaCorteUtc, c.Motivo, c.Huella,
            c.Periodos.Select(p => new DocumentoPeriodo(p.Responsable, p.Desde, p.Hasta, p.FechaLimite, p.Comentarios.ToList())).ToList());

        public CorteTrazabilidad AEntidad() => CorteTrazabilidad.Reconstituir(Id, Organizacion, Proyecto, WorkItemId,
            FechaCorteUtc, Motivo,
            Periodos.Select(p => new PeriodoAsignacion(p.Responsable, p.Desde, p.Hasta, p.FechaLimite, p.Comentarios)).ToList(),
            Huella);
    }

    private sealed record DocumentoPeriodo(string Responsable, DateTime Desde, DateTime? Hasta, DateTime? FechaLimite, List<string> Comentarios);
}
