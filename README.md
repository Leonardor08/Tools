# Tools · Trazabilidad de work items de Azure DevOps

API REST que reconstruye **quién tuvo a cargo un work item, durante cuánto tiempo, si venció su fecha límite y qué comentó**, a partir del historial de revisiones de Azure DevOps.

> 🚧 En modernización hacia **arquitectura hexagonal + CQRS**. Documentación completa en el commit final.

## Configuración rápida

```bash
cd ApiTools
dotnet user-secrets set "AzureDevOps:Pat" "<tu-PAT-con-permiso-Work-Items-Read>"
dotnet run
```

El valor de `appsettings.Development.json` es solo un **ejemplo**: el PAT real nunca se versiona.
