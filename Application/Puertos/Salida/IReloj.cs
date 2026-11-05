namespace Application.Puertos.Salida;

/// <summary>Puerto de salida: hora actual (permite pruebas deterministas).</summary>
public interface IReloj
{
    DateTime AhoraUtc { get; }
}
