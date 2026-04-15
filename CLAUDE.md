# CLAUDE.md

## Project Structure

**Backend (ASP.NET Core 10, Clean Architecture, CQRS):**

- `KProject.Domain` — entities, value objects, domain rules; no dependencies on other layers
- `KProject.Application` — CQRS handlers, commands, queries, validators (FluentValidation), repository interfaces (`Interfaces/`), CQRS marker interfaces (`Abstractions/Messaging/`), shared pagination types (`Shared/`)
- `KProject.Infrastructure` — EF Core `AppDbContext`, repository implementations, `Migrations/`
- `KProject.Api` — minimal API endpoints (`Endpoints/`), `Program.cs`, DI wiring; no business logic here
- `KProject.Common` — cross-cutting primitives: `Result<T>`, `Error`, `ErrorType`
- `KProject.Tests` — `Unit/` (entities + handlers), `Integration/` (endpoints via `ApiFactory`)

Each feature folder (e.g. `Clientes/CriaCliente/`) contains the command/query, its handler, and its validator together.

**Frontend (`KProject.Web/src/app/`):**

- `core/` — `auth.ts` (auth service), `auth-guard.ts`, `public-guard.ts`, `mock.interceptor.ts` (mocka endpoints de lotes com delay aleatório)
- `types/` — interfaces TypeScript globais (`cliente.ts`, `lote.ts`, `produto.ts`, `venda.ts`, `paginated-response.ts`, `result.ts`)
- `layouts/` — `secure-layout` (autenticado) e `public-layout` (login/register), sem lógica de negócio
- `components/` — componentes reutilizáveis globais: `data-table`, `pagination`, `search-bar`, `page-layout`
- `pages/` — uma pasta por rota (`clientes/`, `produtos/`, `vendas/`, `login/`, `register/`, `relatorios/`); cada página pode ter um subdiretório `components/` para componentes exclusivos dela
- `styles/` — partials SCSS globais (ex.: `_auth-form.scss`, `_drawer.scss`)

**Infrastructure:**

- `compose.dev.yaml` is for local dev (includes database); `compose.prod.yaml` is for production (no database — uses external `$POSTGRES_URI`)
- CI/CD via GitHub Actions; hosted on Dokploy

## Domain

Consignment management system:

- **Consigned items** (`ItemConsignado`) belong to a **sale** (`Venda`) and reference a **patient** (`Paciente`) — the end recipient
- A **client** (`Cliente`) receives the consigned items to distribute to patients
- Each item has a history: snapshots of what was returned or sold
- Sales lifecycle: **open → closed** (partial return/sale) or **cancelled** (full return)

**Inventory:**

- Products are in **lots** (`Lote`) with quantity and expiry
- Creating a sale draws stock from a specific lot
- Stock history is delta-based (event sourcing style)

## Commands

**Backend:**
- Run API: `dotnet run --project KProject.Api`
- Run tests: `dotnet test`
- Run unit tests only: `dotnet test --filter "FullyQualifiedName~Unit"`
- Run integration tests only: `dotnet test --filter "FullyQualifiedName~Integration"`
- Apply migrations: `dotnet ef database update --project KProject.Infrastructure --startup-project KProject.Api`
- Add migration: `dotnet ef migrations add <Name> --project KProject.Infrastructure --startup-project KProject.Api`

**Frontend (`KProject.Web/`):**
- Dev server: `npm start`
- Run tests: `npm test`
- Build: `npm run build`

## Backend Rules

- No business logic in controllers — only communication-layer concerns (HTTP status, file streaming, etc.)
- Logic belongs in handlers (CQRS)

## Frontend Conventions

- BEM naming for CSS classes (`.block__element--modifier`)
- Use `:host` as the root selector instead of a wrapper `<div>` in component templates
- No UI library — everything custom

## Testing

- **Backend:** unit tests for entities and handlers; integration tests for endpoints
- **Frontend:** `npm test` (Vitest)
- Run related tests before considering a task done

## Infrastructure

- Do **not** modify `compose.dev.yaml`, `compose.prod.yaml`, CI/CD pipelines, or `appsettings.json` without asking
