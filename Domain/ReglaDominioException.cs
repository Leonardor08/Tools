namespace Domain;

/// <summary>Se viola una regla del dominio (p. ej. un corte sin periodos).</summary>
public sealed class ReglaDominioException(string mensaje) : Exception(mensaje);
