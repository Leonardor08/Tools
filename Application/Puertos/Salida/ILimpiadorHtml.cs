namespace Application.Puertos.Salida;

/// <summary>Puerto de salida: convertir comentarios HTML de Azure DevOps a texto plano.</summary>
public interface ILimpiadorHtml
{
    string ATextoPlano(string contenido);
}
