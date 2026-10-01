# FocusPocuss

### Turn “I need to do this” into a clear next action.

[![CI](https://github.com/laderhad/FocusPocuss/actions/workflows/ci.yml/badge.svg)](https://github.com/laderhad/FocusPocuss/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-19-149ECA?logo=react&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?logo=typescript&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?logo=postgresql&logoColor=white)

FocusPocuss is a focus and task-initiation web application for people who know what they need to do but struggle to begin, stay with it, or return after a distraction.

Capture a task in your own words, get one concrete starting action, and work through a short focus session. If your attention drifts, a recovery flow helps you reconnect with the task.

**The goal: less time managing work, more time actually starting it.**

[Getting started](#getting-started) · [Features](#features) · [Architecture](#architecture) · [Development](#development) · [Product direction](PRODUCT.md)

## Why FocusPocuss?

A task such as “work on my graduation project” leaves a lot to decide before you can start. FocusPocuss narrows that intention into a manageable unit of work.

For example, a starting recommendation might look like this:

> **Task:** I need to work on my backend assignment, but I keep putting it off.  
> **Next action:** Implement the request model for the first missing endpoint.  
> **Suggested session:** 10 minutes.

This is an illustrative example, not a guaranteed model response.

The experience follows a simple loop:

1. **Capture** what you need to do without organizing categories or priorities.
2. **Prepare** one meaningful next action and a suggested duration.
3. **Focus** on that action with a minimal timer interface.
4. **Recover** when distracted, using support matched to the reported reason.
5. **Reflect** briefly and revisit your session history.

The product is designed around low cognitive load: one dominant action, concise language, and fewer decisions before the user can begin.

## Features

| Capability | Current implementation |
| --- | --- |
| Authentication | Registration and sign-in through ASP.NET Core Identity |
| Task capture | Natural-language input, original text preservation, and user-owned task listing |
| Task start planning | AI-generated next action and duration, stored as a reusable start plan |
| Focus sessions | Start or resume an incomplete session, follow the current action, and explicitly complete the session |
| Distraction recovery | Report a reason and receive a corresponding recovery intervention |
| Reflection and history | Record a short reflection after completion and view previous sessions |
| Localization | Turkish and English interface text and task-planning language |
| API documentation | Generated OpenAPI document and Scalar API reference |

### Recovery is part of the workflow

The recovery flow responds to what interrupted the user:

| Reported reason | Selected intervention |
| --- | --- |
| Task feels too difficult | Shrink the current action |
| Next action is unclear | Clarify the current action |
| Phone or social media | Reset the environment |
| Another thought | Capture and park the thought |
| Tiredness | Choose a short reset, a smaller action, or ending the session |
| Other | Reconnect to the current action |

Application rules select the intervention. AI can help transform the action within that selected strategy; session state and authorization remain controlled by application code.

## Technology

| Area | Stack |
| --- | --- |
| Backend | .NET 10, ASP.NET Core, MediatR, FluentValidation |
| Persistence | Entity Framework Core, PostgreSQL |
| Frontend | React 19, TypeScript, Vite, React Router |
| Server state | TanStack Query |
| Localization | i18next, react-i18next |
| Styling | Sass, Pico CSS, product-specific styles and SVG assets |
| AI | Microsoft.Extensions.AI, OpenAI, structured JSON output |
| Local orchestration | Aspire |
| API tooling | OpenAPI, Scalar, NSwag client generation |
| Testing | NUnit, Shouldly, Moq, Reqnroll, Playwright |
| CI | GitHub Actions |

The configured AI model is `gpt-5-mini`. The implementation validates structured responses and keeps the provider integration in Infrastructure.

## Getting started

### Prerequisites

- **.NET SDK 10.0.400**, or a compatible SDK allowed by [global.json](global.json).
- **Node.js 24** and npm, matching the CI environment.
- **Docker** running locally for the PostgreSQL container.
- An **OpenAI API key** for AI-assisted task planning and action transformation.

### 1. Clone the repository

```bash
git clone https://github.com/laderhad/FocusPocuss.git
cd FocusPocuss
```

### 2. Restore dependencies and build

```bash
dotnet restore FocusPocuss.slnx
dotnet build FocusPocuss.slnx
npm ci --prefix src/Web/ClientApp
```

Building the backend generates the OpenAPI document used by the frontend's NSwag client generator.

### 3. Configure the AI provider

Store your development key in the Web project's user secrets:

```bash
dotnet user-secrets set "AI:OpenAI:ApiKey" "<your-openai-api-key>" --project src/Web
```

The model defaults to `gpt-5-mini` in [appsettings.json](src/Web/appsettings.json). To override it locally:

```bash
dotnet user-secrets set "AI:OpenAI:Model" "<model-name>" --project src/Web
```

Equivalent environment variables are `AI__OpenAI__ApiKey` and `AI__OpenAI__Model`. Keep keys out of source control.

### 4. Start the application

```bash
dotnet run --project src/AppHost
```

Aspire orchestrates the PostgreSQL container, backend, and Vite frontend. Open the frontend URL shown in the Aspire dashboard. The dashboard also provides application logs and a **Scalar API Reference** link.

In Development, the backend applies EF Core migrations and seeds the local database at startup. Register an account through the UI to try the task and focus flows.

### Configuration reference

| Setting | Purpose |
| --- | --- |
| `AI:OpenAI:ApiKey` | API key for the OpenAI integration |
| `AI:OpenAI:Model` | Model used by the planners; defaults to `gpt-5-mini` |
| `ConnectionStrings:FocusPocussDb` | PostgreSQL connection; supplied through Aspire during orchestrated local runs |

Task content used for AI-assisted planning is sent to the configured OpenAI service. Account for this when entering private or sensitive information.

## Architecture

The backend follows the existing Clean Architecture boundaries, with use cases grouped by feature.

| Location | Responsibility |
| --- | --- |
| [src/Domain](src/Domain) | Entities, value objects, domain events, and business invariants |
| [src/Application](src/Application) | Task and focus use cases, validation, contracts, and intervention selection |
| [src/Infrastructure](src/Infrastructure) | EF Core persistence, Identity, AI provider implementations, and prompts |
| [src/Web](src/Web) | HTTP endpoints, middleware, composition, and the React application |
| [src/AppHost](src/AppHost) | Aspire resource orchestration |
| [src/ServiceDefaults](src/ServiceDefaults) | Shared service configuration and observability |
| [src/Web/ClientApp/src](src/Web/ClientApp/src) | Frontend app setup, pages, features, and shared UI code |
| [tests](tests) | Domain and application unit tests, functional tests, and browser acceptance tests |

Domain and Application stay independent of OpenAI SDK details. The task planner is consumed through `ITaskStartPlanner`; recovery action planning uses `IRecoveryActionPlanner`.

AI-generated plans use a structured schema and server-side validation. Stored plans retain the model and prompt version, making prompt changes traceable.

The solution builds on the [Jason Taylor Clean Architecture template](https://github.com/jasontaylordev/CleanArchitecture), version 10.8.0.

## Development

### Backend checks

```bash
dotnet build FocusPocuss.slnx
dotnet test tests/Domain.UnitTests/Domain.UnitTests.csproj
dotnet test tests/Application.UnitTests/Application.UnitTests.csproj
dotnet test tests/Application.FunctionalTests/Application.FunctionalTests.csproj
```

Functional tests use containerized dependencies; Docker must be available.

### Frontend checks

Run these from `src/Web/ClientApp` after building the backend:

```bash
npm run generate-api
npm exec -- tsc --noEmit
npm run lint
npm run build
```

Both `npm start` and `npm run build` regenerate the API client through their pre-scripts.

### Browser acceptance tests

Build the solution in Release mode, then install Chromium and run the acceptance project:

```bash
dotnet build FocusPocuss.slnx --configuration Release
pwsh artifacts/bin/Web.AcceptanceTests/release/playwright.ps1 install --with-deps chromium
dotnet test tests/Web.AcceptanceTests/Web.AcceptanceTests.csproj --configuration Release --no-build
```

This installation command requires PowerShell (`pwsh`). Docker and the frontend dependencies must also be available.

The [CI workflow](.github/workflows/ci.yml) builds the solution, runs unit and functional tests, checks frontend types and lint, builds the frontend, and runs browser acceptance tests.

## Project status and direction

FocusPocuss is under active development. The current work centers on making the capture → start → focus → recover → reflect loop reliable and easy to use.

Longer-term goals include recommendations informed by actual session outcomes, better duration selection, and clearer insight into effective recovery strategies. These are product goals, not claims of completed personalization features.

FocusPocuss is a productivity support application and makes no medical treatment claims.

## Contributing

Read [AGENTS.md](AGENTS.md) and [PRODUCT.md](PRODUCT.md) before changing behavior. Keep contributions focused on helping users start, stay with their work, or return after distraction.

Useful project documents:

- [Product specification](PRODUCT.md)
- [Engineering guidelines](AGENTS.md)
- [Task start planner evaluation](docs/task-start-planner-evaluation.md)
- [Recovery intervention design](docs/recovery-interventions.md)

## Author

Built by [Utku Kerem Kalaycı](https://github.com/laderhad).
