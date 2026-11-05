using Application.Puertos.Salida;

namespace Infraestructure;

internal sealed class RelojSistema : IReloj
{
    public DateTime AhoraUtc => DateTime.UtcNow;
}
