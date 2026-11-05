using System.Net;
using Application.Puertos.Salida;
using HtmlAgilityPack;

namespace Infraestructure.Adaptadores.Html;

/// <summary>Adaptador de salida: HtmlAgilityPack queda confinado a infraestructura (antes estaba en Application).</summary>
internal sealed class LimpiadorHtmlAgilityPack : ILimpiadorHtml
{
    public string ATextoPlano(string contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido) || !contenido.Contains('<'))
            return contenido?.Trim() ?? string.Empty;

        var documento = new HtmlDocument();
        documento.LoadHtml(contenido);
        return WebUtility.HtmlDecode(documento.DocumentNode.InnerText).Trim();
    }
}
