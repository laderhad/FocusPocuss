# Distraction recovery execution

The Behavior Engine deterministically selects a versioned intervention from the
existing distraction reason and the session's **current** action. The task start
planner remains separate and unchanged. Recovery is now connected to persisted
state and the Focus UI, rather than stopping at localized advice.

## Reason → behavior

| Reason | Intervention | Execution |
| --- | --- | --- |
| TaskTooDifficult | ShrinkCurrentAction | Propose one smaller grounded action; apply it only on return. |
| UnclearNextAction | ClarifyCurrentAction | Propose one concrete action or request one short clarification. |
| PhoneOrSocialMedia | EnvironmentalReset | Deterministic instruction to put the phone out of sight/reach; keep the action. |
| AnotherThought | ParkThought | Persist a thought of up to 1000 characters and return to the same action. |
| Tired | RecoveryChoice | Short reset, smaller action, or explicit early end. |
| Other | ReconnectToCurrentAction | Show the existing action without inventing a cause. |

Existing reason values and the legacy `strategy` response field remain compatible.
New recovery enums now live in Domain because persisted state transitions use
them. Selection stays in Application. Strategy version is
`distraction-recovery-v1`; transformer prompt version is `recovery-action-v1`.

## State and persistence

`TaskStartPlan.NextAction` and `FocusSession.Action` retain the original action.
`FocusSession.RecoveryAction` holds a subsequently accepted action. Session DTOs
expose the effective action as `action` and the original as `originalAction`.

Each new `DistractionEvent` stores intervention type/version, action at distraction,
proposed action, language, requirement, chosen option, clarification-used flag,
resolution and UTC resolution time. Parked thought text is stored on that same
owned event, not in browser storage or a new general notes system. Old events
retain null recovery metadata; no historical strategy is invented for them.

Only one unresolved recovery may exist per session. A repeated report resumes it.
Pending recovery and saved thoughts are returned by the owned session query, so
reload does not discard them. Accepted actions survive reload as well. Existing
query authorization also protects saved thoughts. Session history exposes only a
thought count and a link to the owned session, not the text itself.

Explicit resolution distinguishes return, keeping the current action, early end,
and dismissal. Dismissing with Escape/close does not record a successful return or
apply a proposal. Resending the same resolved command returns current session state
without reapplying an obsolete action. Other stale resolutions are rejected.
`RecoveryRevision` is an EF concurrency token shared by recovery changes and
session completion; conflicting writes cannot silently overwrite one another.

Early end sets `EndedEarlyAtUtc`, leaving `CompletedAtUtc` null. It frees the active
session slot without claiming success, producing a reflection prompt, or increasing
the weekly completion count. History distinguishes early end from an active session.
The focus timer continues during recovery, as before; the short reset is voluntary,
not a hidden timer restart or an enforced countdown.

## AI boundary and failure behavior

`IRecoveryActionPlanner` transforms content only. It cannot select a strategy and
is called only for shrink/clarify (including the tiredness smaller-action choice).
The adapter uses the existing `IChatClient`, a separate developer prompt, untrusted
JSON user context, and the structured `status` / `action` contract. Status is one
of Ready, NeedsClarification, Unavailable. Actions must be nonempty, different from
the current action, and at most 500 characters. Application validates results again.

The prompt requires meaningful, atomic, naturally bounded and grounded work; it
forbids invented artifact details, hidden plans, coaching and rubric labels. These
semantic requirements still need real-model manual evaluation; length/schema tests
cannot prove them.

A 20-second provider timeout, expected transport/configuration errors, malformed
responses or invalid actions result in Unavailable. Caller cancellation propagates.
No fabricated generic action is substituted. The user can explicitly keep the
current action or dismiss recovery. Only one clarification input (max 500 chars)
is accepted per intervention; if it still cannot yield a grounded action, the
fallback is Unavailable, not another question or a chatbot conversation.

No provider exception contents, prompts or responses are logged. Request payloads
were removed from normal, slow-request and exception pipeline log templates so
parked thoughts and clarification text are not logged by those pipelines. Existing
identity/request-name/timing diagnostics remain.

## API and frontend

- `POST /api/focus-sessions/{id}/distractions`: accepts the reason and `language`
  (`tr`/`en`, default `en` for legacy callers); returns the persisted intervention.
- `PUT /api/focus-sessions/{id}/distractions/{distractionId}/prepare`: accepts a
  reset/smaller-action choice or one clarification; never both.
- `PUT /api/focus-sessions/{id}/distractions/{distractionId}/resolve`: accepts a
  resolution and optional thought, returning the updated session.
- Owned session responses add pending recovery, original action, early-end time,
  and saved thoughts. History adds early-end time and thought count.

The generated TypeScript client is regenerated from OpenAPI, not hand-edited.
TanStack Query owns the response state. Mutation errors refresh session state;
unsaved input stays local. The modal retains keyboard/Escape operation, visible
focus and TR/EN localization. Existing tokens/styles/artwork are reused. Saved
thoughts are under restrained disclosure on the session and linked from history.
No routes, authentication scheme, task-planning behavior or unrelated layouts change.

## Migration and validation

`20261001084815_AddRecoveryExecution` adds nullable recovery fields and a revision
counter and updates active-session/pending-recovery indexes. It does not rewrite
existing plans, session actions or historical events. Apply through the project's
normal deployment process; development startup already runs migrations. Automated
functional tests apply the migration only to their isolated PostgreSQL database.

Tests cover deterministic selection, TR/EN transformation context, strict output,
provider failure/cancellation, durable action application, immutable originals,
thought storage/retrieval, one clarification, tiredness choices, early end, ownership,
ended sessions, idempotent returns and EF concurrency. Browser tests use controlled
API fixtures and exercise the new presentation; they do not call a live model.
No real OpenAI smoke test is claimed.

## Validation and exact change inventory

This inventory compares the working tree at the start of slices B–D, not HEAD; the pre-existing Task Start Planner v2 work is excluded.

- Application unit tests: 61 passed.
- Domain unit tests: 17 passed.
- Functional tests (`FocusSessions` and `CreateTaskStartPlanTests`): 65 passed.
- FocusSession browser acceptance tests: 9 passed.
- TypeScript passed; lint has no errors and two pre-existing Fast Refresh warnings.
- Frontend build passed with the existing Pico/Sass deprecation warning.
- Backend build passed; EF reports no pending model changes.
- `git diff --check` passed.

Commands used: `dotnet test` with `--no-restore`; functional/browser builds used `-p:OpenApiGenerateDocuments=false`. Browser tests used `npm_config_ignore_scripts=true` to avoid regenerating clients during startup. Frontend checks used `npx --no-install tsc --noEmit`, `npm run lint`, and `npm --ignore-scripts run build`. The API client was separately generated with `npm run generate-api` after the OpenAPI-producing backend build.

- Updated: [docs/recovery-interventions.md](../docs/recovery-interventions.md)
- Updated: [src/Application/Behavior/Interventions/DistractionInterventionSelector.cs](../src/Application/Behavior/Interventions/DistractionInterventionSelector.cs)
- Added: [src/Application/Behavior/Interventions/IRecoveryActionPlanner.cs](../src/Application/Behavior/Interventions/IRecoveryActionPlanner.cs)
- Added: [src/Application/Behavior/Interventions/RecoveryActionPreparation.cs](../src/Application/Behavior/Interventions/RecoveryActionPreparation.cs)
- Moved from Application to Domain: `src/Application/Behavior/Interventions/RecoveryChoice.cs`
- Updated: [src/Application/Behavior/Interventions/RecoveryIntervention.cs](../src/Application/Behavior/Interventions/RecoveryIntervention.cs)
- Moved from Application to Domain: `src/Application/Behavior/Interventions/RecoveryInterventionType.cs`
- Moved from Application to Domain: `src/Application/Behavior/Interventions/RecoveryRequirement.cs`
- Updated: [src/Application/Common/Behaviours/LoggingBehaviour.cs](../src/Application/Common/Behaviours/LoggingBehaviour.cs)
- Updated: [src/Application/Common/Behaviours/PerformanceBehaviour.cs](../src/Application/Common/Behaviours/PerformanceBehaviour.cs)
- Updated: [src/Application/Common/Behaviours/UnhandledExceptionBehaviour.cs](../src/Application/Common/Behaviours/UnhandledExceptionBehaviour.cs)
- Added: [src/Application/Common/Exceptions/ConflictException.cs](../src/Application/Common/Exceptions/ConflictException.cs)
- Updated: [src/Application/DependencyInjection.cs](../src/Application/DependencyInjection.cs)
- Updated: [src/Application/FocusSessions/Commands/CompleteFocusSession/CompleteFocusSession.cs](../src/Application/FocusSessions/Commands/CompleteFocusSession/CompleteFocusSession.cs)
- Added: [src/Application/FocusSessions/Commands/PrepareRecovery/PrepareRecovery.cs](../src/Application/FocusSessions/Commands/PrepareRecovery/PrepareRecovery.cs)
- Updated: [src/Application/FocusSessions/Commands/RecordFocusSessionReflection/RecordFocusSessionReflection.cs](../src/Application/FocusSessions/Commands/RecordFocusSessionReflection/RecordFocusSessionReflection.cs)
- Updated: [src/Application/FocusSessions/Commands/ReportDistraction/ReportDistraction.cs](../src/Application/FocusSessions/Commands/ReportDistraction/ReportDistraction.cs)
- Updated: [src/Application/FocusSessions/Commands/ReportDistraction/ReportDistractionCommandValidator.cs](../src/Application/FocusSessions/Commands/ReportDistraction/ReportDistractionCommandValidator.cs)
- Added: [src/Application/FocusSessions/Commands/ResolveRecovery/ResolveRecovery.cs](../src/Application/FocusSessions/Commands/ResolveRecovery/ResolveRecovery.cs)
- Updated: [src/Application/FocusSessions/Commands/StartFocusSession/StartFocusSession.cs](../src/Application/FocusSessions/Commands/StartFocusSession/StartFocusSession.cs)
- Updated: [src/Application/FocusSessions/DistractionReportDto.cs](../src/Application/FocusSessions/DistractionReportDto.cs)
- Updated: [src/Application/FocusSessions/FocusSessionDto.cs](../src/Application/FocusSessions/FocusSessionDto.cs)
- Updated: [src/Application/FocusSessions/FocusSessionHistoryItemDto.cs](../src/Application/FocusSessions/FocusSessionHistoryItemDto.cs)
- Updated: [src/Application/FocusSessions/Queries/GetFocusSession/GetFocusSession.cs](../src/Application/FocusSessions/Queries/GetFocusSession/GetFocusSession.cs)
- Updated: [src/Application/FocusSessions/Queries/GetFocusSessionHistory/GetFocusSessionHistory.cs](../src/Application/FocusSessions/Queries/GetFocusSessionHistory/GetFocusSessionHistory.cs)
- Updated: [src/Domain/Entities/DistractionEvent.cs](../src/Domain/Entities/DistractionEvent.cs)
- Updated: [src/Domain/Entities/FocusSession.cs](../src/Domain/Entities/FocusSession.cs)
- Added: [src/Domain/Enums/RecoveryChoice.cs](../src/Domain/Enums/RecoveryChoice.cs)
- Added: [src/Domain/Enums/RecoveryInterventionType.cs](../src/Domain/Enums/RecoveryInterventionType.cs)
- Added: [src/Domain/Enums/RecoveryRequirement.cs](../src/Domain/Enums/RecoveryRequirement.cs)
- Added: [src/Domain/Enums/RecoveryResolution.cs](../src/Domain/Enums/RecoveryResolution.cs)
- Added: [src/Infrastructure/AI/OpenAiRecoveryActionPlanner.cs](../src/Infrastructure/AI/OpenAiRecoveryActionPlanner.cs)
- Added: [src/Infrastructure/AI/Prompts/RecoveryActionPrompt.cs](../src/Infrastructure/AI/Prompts/RecoveryActionPrompt.cs)
- Updated: [src/Infrastructure/Data/Configurations/DistractionEventConfiguration.cs](../src/Infrastructure/Data/Configurations/DistractionEventConfiguration.cs)
- Updated: [src/Infrastructure/Data/Configurations/FocusSessionConfiguration.cs](../src/Infrastructure/Data/Configurations/FocusSessionConfiguration.cs)
- Added: [src/Infrastructure/Data/Migrations/20261001084815_AddRecoveryExecution.Designer.cs](../src/Infrastructure/Data/Migrations/20261001084815_AddRecoveryExecution.Designer.cs)
- Added: [src/Infrastructure/Data/Migrations/20261001084815_AddRecoveryExecution.cs](../src/Infrastructure/Data/Migrations/20261001084815_AddRecoveryExecution.cs)
- Updated: [src/Infrastructure/Data/Migrations/ApplicationDbContextModelSnapshot.cs](../src/Infrastructure/Data/Migrations/ApplicationDbContextModelSnapshot.cs)
- Updated: [src/Infrastructure/DependencyInjection.cs](../src/Infrastructure/DependencyInjection.cs)
- Updated: [src/Web/ClientApp/src/app/locales/en.ts](../src/Web/ClientApp/src/app/locales/en.ts)
- Updated: [src/Web/ClientApp/src/app/locales/tr.ts](../src/Web/ClientApp/src/app/locales/tr.ts)
- Updated: [src/Web/ClientApp/src/features/focus/api/focusApi.ts](../src/Web/ClientApp/src/features/focus/api/focusApi.ts)
- Updated: [src/Web/ClientApp/src/features/focus/api/focusQueries.ts](../src/Web/ClientApp/src/features/focus/api/focusQueries.ts)
- Updated: [src/Web/ClientApp/src/features/focus/components/DistractionReporter.tsx](../src/Web/ClientApp/src/features/focus/components/DistractionReporter.tsx)
- Updated: [src/Web/ClientApp/src/features/focus/components/FocusSessionHistoryList.tsx](../src/Web/ClientApp/src/features/focus/components/FocusSessionHistoryList.tsx)
- Updated: [src/Web/ClientApp/src/features/focus/components/FocusSessionView.tsx](../src/Web/ClientApp/src/features/focus/components/FocusSessionView.tsx)
- Added: [src/Web/ClientApp/src/features/focus/components/ParkedThoughts.tsx](../src/Web/ClientApp/src/features/focus/components/ParkedThoughts.tsx)
- Added: [src/Web/ClientApp/src/features/focus/components/RecoveryInterventionView.tsx](../src/Web/ClientApp/src/features/focus/components/RecoveryInterventionView.tsx)
- Updated: [src/Web/ClientApp/src/pages/FocusSessionPage.tsx](../src/Web/ClientApp/src/pages/FocusSessionPage.tsx)
- Updated: [src/Web/ClientApp/src/styles.scss](../src/Web/ClientApp/src/styles.scss)
- Updated: [src/Web/DependencyInjection.cs](../src/Web/DependencyInjection.cs)
- Updated: [src/Web/Endpoints/FocusSessions.cs](../src/Web/Endpoints/FocusSessions.cs)
- Updated: [src/Web/Infrastructure/FocusSessionReflectionSchemaTransformer.cs](../src/Web/Infrastructure/FocusSessionReflectionSchemaTransformer.cs)
- Updated: [src/Web/Infrastructure/ProblemDetailsExceptionHandler.cs](../src/Web/Infrastructure/ProblemDetailsExceptionHandler.cs)
- Added: [tests/Application.FunctionalTests/FocusSessions/Commands/RecoveryExecutionTests.cs](../tests/Application.FunctionalTests/FocusSessions/Commands/RecoveryExecutionTests.cs)
- Updated: [tests/Application.FunctionalTests/Infrastructure/TestApp.cs](../tests/Application.FunctionalTests/Infrastructure/TestApp.cs)
- Added: [tests/Application.FunctionalTests/Infrastructure/TestRecoveryActionPlanner.cs](../tests/Application.FunctionalTests/Infrastructure/TestRecoveryActionPlanner.cs)
- Updated: [tests/Application.FunctionalTests/Infrastructure/WebApiFactory.cs](../tests/Application.FunctionalTests/Infrastructure/WebApiFactory.cs)
- Added: [tests/Application.UnitTests/Behavior/Recovery/OpenAiRecoveryActionPlannerTests.cs](../tests/Application.UnitTests/Behavior/Recovery/OpenAiRecoveryActionPlannerTests.cs)
- Added: [tests/Domain.UnitTests/Entities/RecoveryExecutionTests.cs](../tests/Domain.UnitTests/Entities/RecoveryExecutionTests.cs)
- Updated: [tests/Web.AcceptanceTests/Features/FocusSession.feature](../tests/Web.AcceptanceTests/Features/FocusSession.feature)
- Updated: [tests/Web.AcceptanceTests/Pages/FocusSessionPage.cs](../tests/Web.AcceptanceTests/Pages/FocusSessionPage.cs)
- Updated: [tests/Web.AcceptanceTests/StepDefinitions/FocusSessionStepDefinitions.cs](../tests/Web.AcceptanceTests/StepDefinitions/FocusSessionStepDefinitions.cs)

Regenerated build artifacts (ignored by Git):

- `src/Web/ClientApp/src/web-api-client.ts`
- `src/Web/wwwroot/openapi/v1.json`
