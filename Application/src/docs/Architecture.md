# Architecture

This project uses Vertical Slice Architecture for organizing ASP.NET Core API features.

## Services

The solution is split into independently deployable ASP.NET Core services under
`Application/src/services`:

| Service | Responsibility |
|---|---|
| `Order.Service` | Order lifecycle and order-related workflows |
| `Payment.Service` | Payment processing and payment status |
| `Product.Service` | Product catalog and product information |
| `Inventory.Service` | Stock availability and inventory updates |

Each service is a separate project and owns its own API boundary, business
features, infrastructure integrations, and composition root. Services must never
reference another service project; the only allowed shared reference is
`Platform.Contracts`. Cross-service workflows should use explicit contracts or
integration events through the messaging boundary.

`Platform.Contracts` is a plain class library under `Application/src` that
contains shared event classes and message contracts. It has no NuGet
dependencies and may contain only records and enums. Event names are versioned,
such as `OrderCreatedV1`; a published event version must never be edited, and
changes must be introduced as a new version. It must remain independent of
service implementations, persistence, transport concerns, and business
workflows. Services may reference these contracts when publishing or consuming
integration events, but the contracts project must not reference any service.

The current service layout is:

```text
Application/
└── src/
    ├── docs/
    │   └── Architecture.md
    ├── Platform.Contracts/
    │   └── Platform.Contracts.csproj
    └── services/
        ├── Inventory.Service/
        ├── Order.Service/
        ├── Payment.Service/
        └── Product.Service/
```

New services should be created as sibling projects in `Application/src/services`,
and shared event classes should be added to `Application/src/Platform.Contracts`.
All projects must be added to `Micro-Service.RabbitMQ.sln`.

Each service follows this structure:

```text
<Service>/
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── Dockerfile
├── Controllers/
├── Features/
│   └── <Area>/
│       ├── <Entity>.cs
│       └── <UseCase>/
│           ├── Command.cs
│           ├── Handler.cs
│           ├── Validator.cs
│           └── Response.cs
├── Common/
│   ├── Behaviors/
│   ├── Abstractions/
│   ├── Domain/
│   ├── Options/
│   ├── Extensions/
│   ├── Results/
│   ├── Exceptions/
│   └── Middleware/
├── Infrastructure/
│   ├── DependencyInjection.cs
│   ├── Persistence/
│   │   ├── Configurations/
│   │   └── Migrations/
│   ├── Messaging/
│   │   ├── Consumers/
│   │   ├── FaultConsumers/
│   │   ├── Publishers/
│   │   ├── Outbox/
│   │   ├── Filters/
│   │   └── Options/
│   ├── ExternalClients/
│   └── Security/
```

Each service has corresponding tests organized by test type:

```text
tests/
├── <Service>.UnitTests/
└── <Service>.IntegrationTests/
```

Shared event contracts are grouped by their publishing service:

```text
Platform.Contracts/
├── Orders/
├── Payments/
├── Products/
└── Inventory/
```

The repository layout is:

```text
Micro-Service.RabbitMQ/
├── Application/
├── deploy/
├── .github/
│   └── workflows/
├── docs/
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
├── .editorconfig
├── .gitignore
└── README.md
```

The solution layout supports both service-focused and repository-wide work.
Each service has its own `.sln` file for daily development, while
`Micro-Service.RabbitMQ.sln` remains at the repository root for full builds and
CI.

## API Gateway Guardrails

`Api.Gateway` is a sibling to the `services/` folder inside `Application/src/`.
It is the only public-facing application entry point and is implemented as a
minimal YARP reverse proxy.

The gateway has zero business knowledge. It is strictly a routing reverse proxy
and must never access a database, reference `Platform.Contracts`, or contain
domain business logic. It owns global routing, rate limiting, and SSL
termination. Data validation and feature-specific authorization remain entirely
within the downstream vertical slices.

## Deployment

Deployment assets live in the root `deploy/` directory:

```text
deploy/
├── docker-compose.yml
├── docker-compose.override.yml
├── k8s/
│   ├── namespace.yaml
│   └── services/
│       ├── inventory-service.yaml
│       ├── order-service.yaml
│       ├── payment-service.yaml
│       └── product-service.yaml
└── iac/
    └── terraform/
        ├── main.tf
        ├── outputs.tf
        ├── README.md
        └── variables.tf
```

The base Compose file runs all four services, one PostgreSQL database per
service, and RabbitMQ for local integration testing. The override file is
reserved for developer-machine settings and must not contain production
secrets. Service names, container names, Kubernetes resources, and deployment
files use lowercase kebab-case; project and assembly names retain their
PascalCase `.Service` convention.

The Kubernetes manifests are intentionally scoped to the service deployments
and internal ClusterIP endpoints. Database, broker, image registry, secrets,
storage, and environment-specific configuration belong in the deployment
platform or the Terraform boundary rather than in service code.

## Core Guardrails

### Thin Controllers

Controllers are HTTP transport adapters. They accept input, delegate to the appropriate feature, and translate the result into an HTTP response.

Controllers must not contain business rules, database queries, messaging logic, or external API orchestration. If a controller grows beyond transport concerns, move the behavior into the relevant feature slice.

### Self-Contained Feature Slices

Each requirement should be implemented inside its own feature directory. A slice owns the request, response, validation, handler, mapping, and coordination required for one use case.

Feature slices must not depend on another slice's internal implementation. Shared code is appropriate only for stable, genuinely cross-cutting technical behavior. Explicit abstractions, domain contracts, or integration events should be used when slices need to communicate.

### Entities

Entities live in `Features/<Area>/` and are shared only by slices within the
same area. `Common` holds only generic base types and must not contain
business-specific entities.

### Consumers

Broker consumers live in `Infrastructure/Messaging/Consumers`. Consumers must
remain thin and only call the relevant feature through the mediator. Features
must never reference `Infrastructure`.

## Boundary Matrix

| Area | Owns | May contain | Must not contain |
|---|---|---|---|
| `Features/` | Business use cases | Requests, responses, handlers, validators, feature-specific rules | Generic infrastructure plumbing or unrelated feature internals |
| `Common/` | Technical cross-cutting concerns | Behaviors, abstractions, options, results, exceptions, middleware, extensions | Business rules, feature workflows, EF Core implementations |
| `Infrastructure/` | External technical plumbing | EF Core, migrations, brokers, API clients, security providers, storage | HTTP controller behavior or business decisions |
| `Controllers/` | HTTP transport | Routing, model binding, authorization metadata, response mapping | Business logic, persistence, messaging, external calls |
| `Program.cs` | Composition root | Startup and extension-method orchestration | Service registrations, middleware details, feature logic |

### Common

`Common` contains reusable, business-agnostic technical mechanisms such as pipeline behaviors, result types, exception handling, middleware, options, shared technical interfaces, and registration extensions.

It must not know about business concepts such as orders, customers, or payments.

### Infrastructure

`Infrastructure` contains implementations that communicate with external systems or concrete providers, including EF Core, message brokers, HTTP clients, identity integrations, and file storage.

Infrastructure may implement abstractions defined elsewhere, but business rules remain in the relevant feature or domain model.

## Dependency Direction

```text
Controllers
    ↓
Features
    ↓
Common abstractions

Infrastructure → Common abstractions
Infrastructure → external providers
Infrastructure/Messaging → Features (commands only)
Infrastructure/Persistence → Features (entities only)
```

Features should depend on abstractions when external behavior is required.
Infrastructure supplies implementations during application composition.
Features must never reference Infrastructure.

Although each microservice project references the shared platform packages
globally, code under `Features/` must remain agnostic of concrete
Infrastructure tools. Features may use MediatR and FluentValidation, but must
never reference MassTransit or Entity Framework Core namespaces directly.
Messaging and persistence concerns remain behind the Infrastructure boundary.

## Documentation as Code

Update this document alongside architectural code changes. When introducing a new pattern or boundary:

1. Document the guardrail and intended dependency direction.
2. Keep examples representative and implementation-independent.
3. Remove guidance that no longer reflects the codebase.

## Changes

- Documented strict service isolation with `Platform.Contracts` as the only shared reference.
- Defined `Platform.Contracts` contents, dependency, and event-versioning rules.
- Added entity ownership, consumer placement, mediator usage, and dependency-direction rules.
- Added the standard folder structure for each service.
- Added service test, publisher-grouped contract, and repository layout trees.
- Documented the per-service and root solution layout.
- Added the persistence-to-features entity dependency direction.
- Corrected the solution name to `Micro-Service.RabbitMQ.sln`.
- Documented the package-boundary rule separating feature code from messaging and persistence implementations.
