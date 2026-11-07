# Tools · Trazabilidad de work items de Azure DevOps

API REST que reconstruye **quién tuvo a cargo un work item, durante cuánto tiempo, si venció su fecha límite y qué comentó**, a partir del historial de revisiones de Azure DevOps.

> 🚧 En modernización: **arquitectura hexagonal + CQRS**. Documentación completa en el commit final.

## Endpoints

| Tipo | Método y ruta | Descripción |
|---|---|---|
| Consulta | `GET /api/{org}/{proyecto}/workitems/{id}/trazabilidad` | Periodos de asignación, horas y vencimiento |
| Consulta | `GET /api/{org}/{proyecto}/consultas/{consultaId}` | Ejecuta una consulta guardada |
| Consulta | `GET /api/{org}/{proyecto}/consultas/historias?responsable=` | Historias activas de un responsable |
| **Comando** | `POST /api/{org}/{proyecto}/workitems/{id}/cortes` | Congela la trazabilidad como evidencia (huella SHA-256) |
| Consulta | `GET /api/{org}/{proyecto}/workitems/{id}/cortes` | Cortes registrados de un work item |
| Consulta | `GET /api/cortes/{corteId}` | Detalle del corte con verificación de integridad |

## Configuración rápida

```bash
cd ApiTools
dotnet user-secrets set "AzureDevOps:Pat" "<tu-PAT-con-permiso-Work-Items-Read>"
dotnet run
```

El valor de `appsettings.Development.json` es solo un **ejemplo**: el PAT real nunca se versiona.
