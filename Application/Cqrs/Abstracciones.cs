namespace Application.Cqrs;

// ---------------------------------------------------------------------------
// CQRS mínimo propio (sin MediatR): separa explícitamente lo que LEE (consultas)
// de lo que MODIFICA estado (comandos), y permite comportamientos transversales.
// ---------------------------------------------------------------------------

/// <summary>Cualquier mensaje que la aplicación sabe atender y que produce <typeparamref name="TRespuesta"/>.</summary>
public interface ISolicitud<TRespuesta> { }

/// <summary>Consulta: solo lee; no cambia estado. Puede cachearse.</summary>
public interface IConsulta<TRespuesta> : ISolicitud<TRespuesta> { }

/// <summary>Comando: cambia estado del sistema. Nunca se cachea.</summary>
public interface IComando<TRespuesta> : ISolicitud<TRespuesta> { }

/// <summary>Consulta cuyo resultado puede reutilizarse durante un tiempo (p. ej. respuestas de Azure DevOps).</summary>
public interface IConsultaCacheable
{
    string ClaveCache { get; }
    TimeSpan DuracionCache { get; }
}

public interface IManejador<in TSolicitud, TRespuesta> where TSolicitud : ISolicitud<TRespuesta>
{
    Task<TRespuesta> ManejarAsync(TSolicitud solicitud, CancellationToken ct);
}

public delegate Task<TRespuesta> SiguientePaso<TRespuesta>();

/// <summary>Comportamiento transversal (pipeline): registro, validación, caché...</summary>
public interface IComportamiento<in TSolicitud, TRespuesta> where TSolicitud : ISolicitud<TRespuesta>
{
    Task<TRespuesta> ManejarAsync(TSolicitud solicitud, SiguientePaso<TRespuesta> siguiente, CancellationToken ct);
}

/// <summary>Valida una solicitud antes de llegar al manejador.</summary>
public interface IValidador<in TSolicitud>
{
    IEnumerable<(string Campo, string Mensaje)> Validar(TSolicitud solicitud);
}

/// <summary>Puerto de ENTRADA único: los adaptadores (API, CLI, pruebas) envían mensajes aquí.</summary>
public interface IMediador
{
    Task<TRespuesta> EnviarAsync<TRespuesta>(ISolicitud<TRespuesta> solicitud, CancellationToken ct = default);
}
