# FocusPocus Engineering & Agent Instructions

This file defines the engineering rules for FocusPocus.

All coding agents and contributors should follow these instructions unless a newer explicit task requirement overrides them.

Before implementing user-facing behavior, read `PRODUCT.md`.

---

## 1. Engineering Philosophy

FocusPocus should be:

- modular,
- maintainable,
- testable where it matters,
- easy to change,
- easy to understand,
- pragmatic,
- resistant to unnecessary complexity.

Prefer:

> clear code + explicit boundaries + small changes

over:

> clever abstractions + speculative architecture + framework-heavy solutions

Do not optimize for hypothetical future requirements that have not appeared.

Do not introduce architectural complexity merely to demonstrate a design pattern.

---

## 2. Current Technology Stack

### Backend

- .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Aspire
- MediatR where already useful
- FluentValidation where appropriate
- OpenAPI

### Frontend

- React
- TypeScript with strict mode
- Vite
- React Router
- TanStack Query for server state
- React Hook Form for forms where appropriate
- Zod for client-side schema validation where appropriate
- react-i18next for localization
- Tailwind CSS
- shadcn/ui as a component foundation when appropriate
- a FocusPocus-specific design system built on reusable tokens and primitives

### AI

- Provider-independent application abstractions
- `Microsoft.Extensions.AI` may be used in Infrastructure
- Structured AI output where possible
- No provider SDK dependencies in Domain
- No provider-specific business logic in Application

### Data

- PostgreSQL
- EF Core migrations
- No additional database technology without demonstrated need

### Local Development

- Aspire
- Docker / containerized dependencies where useful

---

## 3. Baseline Backend Architecture

The project starts from the Jason Taylor Clean Architecture solution template.

Respect the existing dependency direction.

Conceptually:

```text
Domain
↑
Application
↑
Infrastructure / Web
```

Inner layers must not depend on outer implementation details.

The initial project does not need to be physically split into many module projects.

Prefer logical feature boundaries first.

---

## 4. Backend Project Responsibilities

### Domain

Domain contains core business concepts and behavior.

Examples:

- entities,
- value objects,
- domain rules,
- domain events where justified,
- business invariants.

Domain must not depend on:

- EF Core implementation types,
- ASP.NET Core,
- HTTP,
- PostgreSQL,
- OpenAI,
- Gemini,
- external services,
- Infrastructure.

Do not create an anemic domain model purely for architectural appearance, but also do not force rich DDD where the problem is simple CRUD.

Use domain behavior when there is real business behavior.

---

### Application

Application contains use cases.

Examples:

- commands,
- queries,
- handlers,
- validation,
- interfaces required by use cases,
- orchestration,
- DTO projections where appropriate.

Application should answer:

> "What does the system do?"

Prefer organizing by feature/use case.

Example:

```text
Application/
  Tasks/
    Commands/
      CreateTask/
    Queries/
      GetTasks/

  Focus/
    Commands/
      StartFocusSession/
      CompleteFocusSession/

  Behavior/
    Commands/
      ReportDistraction/
```

Do not organize the entire project around generic technical buckets such as:

```text
Services/
Managers/
Helpers/
Processors/
```

unless there is a concrete reason.

---

### Infrastructure

Infrastructure contains implementation details.

Examples:

- EF Core DbContext implementation,
- persistence configuration,
- identity implementation,
- AI providers,
- email providers,
- external API integrations,
- file storage,
- system clock implementation if required.

Infrastructure may depend on Application and Domain.

Application and Domain must not depend on Infrastructure.

---

### Web

Web is the delivery mechanism.

Responsibilities:

- HTTP endpoints,
- authentication wiring,
- authorization wiring,
- request/response translation,
- middleware,
- dependency composition,
- API documentation.

Endpoints should be thin.

Do not put business rules directly inside endpoints.

---

## 5. Data Access Rules

Follow the template's pragmatic approach unless a feature requires otherwise.

Do not automatically create:

```text
IEntityRepository
EntityRepository
IEntityService
EntityService
```

for every entity.

Avoid generic repository abstractions over EF Core.

If the existing `IApplicationDbContext` approach is sufficient, use it.

Introduce repositories only when they model a meaningful domain boundary or aggregate behavior, not merely to wrap `DbSet<T>`.

---

## 6. Modular Boundaries

FocusPocus values modularity, but MVP development must remain pragmatic.

Initial logical areas include:

- Identity
- Tasks
- Focus
- Behavior
- Coaching / AI
- Analytics
- Localization

Do not allow unrelated features to become tightly coupled.

A feature should not reach into another feature's internal implementation without a clear contract.

Avoid shared mutable state across features.

If module boundaries later become important enough to justify physical project separation, refactor then.

Do not create separate class libraries for every module during the MVP solely for theoretical purity.

---

## 7. API Design

Prefer clear resource and use-case-oriented endpoints.

API contracts should be explicit.

Do not expose EF Core entities directly as public API contracts.

Use request / response contracts where appropriate.

Use correct HTTP semantics.

Return errors consistently.

Do not leak exception implementation details to clients.

Use cancellation tokens for asynchronous operations where relevant.

---

## 8. Async Rules

For I/O-bound backend operations:

- use async APIs,
- propagate `CancellationToken`,
- avoid `.Result`,
- avoid `.Wait()`,
- avoid blocking async code,
- do not create fake async methods.

Prefer:

```csharp
await operation(cancellationToken);
```

Do not use `Task.Run` to wrap normal database or network I/O.

---

## 9. Validation

Validation belongs at appropriate boundaries.

Use FluentValidation where it improves clarity.

Do not rely on frontend validation for correctness.

Backend validation remains authoritative.

Avoid duplicating complex business rules in validators if they belong in domain behavior.

---

## 10. Error Handling

Use consistent error handling.

Do not:

- swallow exceptions,
- return raw stack traces,
- catch `Exception` without a real recovery reason,
- use exceptions for expected control flow.

Expected business failures should use explicit application-level results or validation errors where appropriate.

Unexpected failures should be handled by centralized exception handling.

---

## 11. React Architecture

The frontend should use feature-based organization.

Recommended direction:

```text
src/
  app/
  pages/
  features/
  shared/
```

Dependency direction:

```text
app
↓
pages
↓
features
↓
shared
```

`shared` must not depend on product features.

Pages compose features.

Features own their behavior.

Shared code must genuinely be reusable.

---

## 12. React Feature Structure

A feature may use a structure similar to:

```text
features/
  focus/
    api/
    components/
    hooks/
    model/
    schemas/
    index.ts
```

Not every folder is mandatory.

Create only what the feature actually needs.

Do not create empty architectural folders.

---

## 13. React Component Rules

UI components should remain focused.

Avoid components that combine:

- API calls,
- validation,
- timer logic,
- analytics,
- state orchestration,
- rendering,

all in one file.

Extract logic when it has a meaningful reason to exist.

Do not extract every three-line function into a hook.

Prefer composition over giant components.

Keep component props understandable.

---

## 14. Server State vs Client State

Server state belongs primarily in TanStack Query.

Examples:

- tasks,
- focus sessions,
- user profile,
- analytics,
- session history.

Do not mirror server data into global state without a concrete need.

Client/UI state may include:

- open dialog state,
- temporary form state,
- transient focus UI state,
- theme,
- locale.

Use global state only when state is truly global.

Do not add Redux, Zustand, or another global state library without demonstrated need.

---

## 15. API Calls in React

Do not make raw HTTP calls directly inside presentational UI components.

Avoid:

```tsx
function FocusCard() {
  fetch("/api/focus");
}
```

Prefer a feature-level API function or hook.

Example:

```text
features/focus/api/startFocusSession.ts
```

Components should consume a clear interface.

---

## 16. TypeScript Rules

Use TypeScript strict mode.

Avoid `any`.

If `any` is unavoidable, explain why.

Prefer explicit types at public boundaries.

Avoid unnecessary type duplication.

Do not create complex generic abstractions for simple feature code.

---

## 17. Forms

Use React Hook Form and Zod when they simplify non-trivial forms.

Do not force form libraries onto tiny one-field interactions if native state is simpler.

Frontend validation improves UX.

Backend validation remains authoritative.

---

## 18. Localization

All user-facing text must support localization.

Initial locales:

- `tr`
- `en`

Do not hardcode product copy throughout components.

Prefer translation keys.

Localization should include:

- navigation,
- buttons,
- errors,
- empty states,
- focus messaging,
- intervention messaging.

AI-generated language should follow the user's preferred language.

---

## 19. Frontend Shared Folder Rules

`shared/` is not a dumping ground.

Good candidates:

- Button
- Input
- Dialog
- Typography
- generic API client
- reusable utility with no product knowledge

Bad candidates:

- FocusTimer
- TaskBreakdownPanel
- DistractionReasonSelector

Those belong to their features.

Rule:

> If something is used by only one feature and contains feature knowledge, it is not shared.

---

## 20. Design System Rules

FocusPocus must have one consistent visual system.

Prefer shared tokens for:

- color,
- typography,
- spacing,
- radius,
- shadows,
- motion.

Do not introduce arbitrary one-off visual values without reason.

Avoid:

```tsx
rounded-[17px]
bg-[#121317]
px-[23px]
```

when an existing token or reusable primitive exists.

Do not create new colors, spacing scales, component patterns, or interaction patterns merely to make one screen look different.

---

## 21. Visual Product Rules

The application should feel:

- calm,
- minimal,
- premium,
- quiet,
- focused.

Avoid generic AI SaaS visuals:

- unnecessary gradients,
- purple AI themes,
- sparkle icons,
- excessive glassmorphism,
- decorative cards,
- random animated backgrounds.

AI should not dominate the visual hierarchy.

---

## 22. Focus Mode UI Rules

When a user starts a focus session:

- remove unnecessary navigation,
- reduce visible options,
- show the current action clearly,
- show remaining time clearly,
- keep distraction recovery accessible,
- avoid analytics and unrelated information.

The focus experience should minimize interaction with the application itself.

---

## 23. AI Architecture

Application code should depend on domain-specific interfaces.

Examples:

```csharp
public interface ITaskDecomposer
{
    Task<TaskBreakdown> DecomposeAsync(
        TaskContext context,
        CancellationToken cancellationToken);
}
```

Possible application abstractions:

- `ITaskDecomposer`
- `ITaskInterpreter`
- `IInterventionPersonalizer`
- `IInsightSummarizer`

Do not make Application depend directly on:

- `OpenAIClient`,
- Gemini SDK,
- Anthropic SDK,
- provider-specific response types.

Provider implementations belong in Infrastructure.

---

## 24. AI Structured Output

Prefer structured output.

Example conceptual result:

```json
{
  "strategy": "TASK_DECOMPOSITION",
  "message": "Let's make this smaller.",
  "nextAction": "Open the assignment and identify question one.",
  "durationMinutes": 8
}
```

Validate AI responses.

Do not trust model output blindly.

Provide graceful fallback behavior for:

- malformed output,
- missing fields,
- provider errors,
- timeouts,
- unavailable model.

---

## 25. Behavior Engine vs LLM

This is a critical rule.

The Behavior Engine decides which approved intervention strategy should be used.

The LLM may:

- interpret user context,
- assist classification,
- personalize wording.

The LLM must not be the sole authority deciding arbitrary psychological interventions.

Conceptual flow:

```text
User Context
↓
Structured Analysis
↓
Behavior Engine
↓
Approved Strategy
↓
LLM Personalization
↓
User Message
```

---

## 26. AI Prompt Management

Do not scatter large prompt strings randomly across application code.

Keep prompts centralized and identifiable.

Prompt changes should be reviewable.

Prefer versionable prompt definitions.

If prompt complexity grows, organize it intentionally.

Do not create a large prompt infrastructure during the MVP unless needed.

---

## 27. Security

Never commit:

- API keys,
- secrets,
- passwords,
- production connection strings,
- private certificates.

Use environment variables, local secrets, or appropriate secret stores.

Never trust client-side authorization checks.

Enforce authorization on the backend.

Do not log secrets or sensitive user content unnecessarily.

---

## 28. Privacy

Collect only data required for product behavior and improvement.

Behavioral data may become sensitive from a user-trust perspective.

Do not collect unnecessary browsing, device, or personal data.

Any future monitoring or distraction-detection feature must be explicit and transparent.

---

## 29. Logging and Observability

Use structured logging.

Do not log large AI prompts or responses by default if they contain user content.

Log enough information to debug:

- request failures,
- AI provider failures,
- database failures,
- focus session state errors.

Avoid noisy logging.

OpenTelemetry may be used for useful application observability.

Do not build enterprise-grade telemetry infrastructure before it is needed.

---

## 30. Testing Strategy

The MVP does not require maximum test coverage.

Prioritize tests that protect meaningful behavior.

### Unit tests

Write unit tests for:

- Behavior Engine rules,
- non-trivial domain invariants,
- complex pure business logic.

Avoid low-value tests created only for coverage.

### Integration / functional tests

Prioritize critical API flows such as:

```text
Create task
Start focus session
Report distraction
Complete session
```

Prefer testing persistence behavior against a real database when practical.

### Frontend tests

Do not test trivial presentational components by default.

Test complex hooks or behavior when regression risk justifies it.

### E2E

A small number of critical happy-path tests is sufficient for MVP.

---

## 31. Dependency Rules

Before adding a package, ask:

1. What problem does it solve?
2. Can the existing stack solve this cleanly?
3. Does the package introduce significant maintenance cost?
4. Is it needed now?

Do not add libraries merely because they are popular.

Do not replace existing libraries without a concrete benefit.

---

## 32. No Premature Infrastructure

Do not introduce these during MVP unless a real requirement exists:

- RabbitMQ
- Kafka
- Redis
- Kubernetes
- service mesh
- microservices
- event sourcing
- distributed transactions
- elaborate message buses.

Add infrastructure in response to actual problems.

---

## 33. No Premature Abstractions

Avoid patterns such as:

```text
IUserService
UserService
IUserManager
UserManager
IUserProcessor
UserProcessor
```

unless they have distinct responsibilities.

An interface must have a reason.

Prefer concrete classes when substitution, boundary inversion, or testing does not justify an abstraction.

---

## 34. Refactoring Rules

Do not refactor unrelated code while implementing a feature.

Small local improvements are acceptable when necessary for the requested change.

Large refactors should be separate tasks.

Do not combine:

- feature work,
- dependency upgrades,
- architecture changes,
- formatting entire projects,

in one change unless explicitly requested.

---

## 35. Coding Agent Workflow

For non-trivial features:

1. Read `PRODUCT.md`.
2. Read this file.
3. Inspect the relevant existing implementation.
4. Understand the existing pattern before proposing a new one.
5. Produce a small implementation plan before editing when the task is architecturally meaningful.
6. Keep scope limited.
7. Implement the feature.
8. Build affected projects.
9. Run relevant tests / lint / type checks.
10. Review the change for architectural violations.
11. Summarize important decisions and changed areas.

Do not create unrelated files or features.

---

## 36. Planning Rules

When planning a feature, identify:

- owning feature / module,
- user-visible outcome,
- backend use case,
- API contract,
- persistence changes,
- frontend feature changes,
- validation,
- important failure paths.

Do not produce enterprise architecture diagrams for simple CRUD.

---

## 37. Change Scope

A task such as:

> "Add task capture"

does not authorize:

- adding analytics,
- redesigning navigation,
- introducing Redis,
- changing authentication,
- rewriting the design system,
- adding speculative abstractions.

Implement the smallest complete vertical slice.

---

## 38. Completion Checklist

Before considering a feature complete, verify:

### Backend

- project builds,
- migrations are valid if changed,
- authorization is enforced where required,
- cancellation tokens are propagated where relevant,
- domain/application boundaries remain valid,
- errors are handled consistently.

### Frontend

- TypeScript passes,
- lint passes,
- loading state is handled,
- error state is handled,
- empty state is considered where relevant,
- user-facing strings use localization,
- feature boundaries are respected,
- design system is respected.

### Product

- implementation matches `PRODUCT.md`,
- no unnecessary functionality was added,
- the primary user action remains clear,
- the application did not become more cognitively demanding without good reason.

---

## 39. Learning-Friendly Development

This repository is also used for deliberate engineering learning.

When explaining generated code:

- explain important architectural decisions,
- identify non-obvious .NET / React concepts,
- explain why a pattern was used,
- avoid hiding complexity behind "best practice" wording.

Do not add abstractions the developer cannot reasonably explain.

Generated code should remain understandable.

---

## 40. Final Engineering Rule

The most important rule:

> Do not optimize FocusPocus for architectural impressiveness. Optimize it for clarity, maintainability, product iteration speed, and real user value.

When choosing between two valid implementations, prefer the simpler one that preserves clear boundaries and can evolve later.
