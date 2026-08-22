# FocusPocus Product Specification

## 1. Product Vision

FocusPocus is a behavioral focus and task-initiation application.

Its primary purpose is to help people who already know what they need to do, but struggle with one or more of the following:

- starting a task,
- reducing overwhelm,
- identifying the next concrete action,
- staying focused,
- returning to work after distraction,
- choosing a realistic focus duration,
- understanding their own work patterns over time.

FocusPocus should create the following user habit:

> "I'm stuck → I open FocusPocus → I start moving again."

FocusPocus is not primarily a traditional todo application, calendar application, Pomodoro timer, project-management suite, habit tracker, or AI chatbot.

The product should feel like a calm behavioral execution system that helps the user move from intention to action.

---

## 2. Core Product Promise

The main product promise is:

> Help the user start, stay focused, recover from distraction, and gradually learn how they work best.

The product should reduce the gap between:

- "I need to do this"
- and
- "I am actually doing this."

FocusPocus succeeds when the user starts meaningful work with less friction.

---

## 3. Core User Problems

### 3.1 Task Initiation

The user knows what needs to be done but cannot start.

Example:

> "I need to work on my backend assignment, but I keep avoiding it."

The product should avoid giving a long explanation.

Instead, it should reduce the task into a clear first action.

Example:

> Open the project and identify the first endpoint.
>
> 8 minutes.
>
> Start.

---

### 3.2 Overwhelm

The task feels too large, ambiguous, emotionally heavy, or difficult.

Example:

> "I need to finish my graduation project."

The product should recognize that this is not a useful immediate action and help reduce the task into something concrete.

---

### 3.3 Unclear Next Action

The user may not actually know what to do next.

Example:

> "I need to work on authentication."

A better next step may be:

> Open the authentication requirements and list the endpoints that are missing.

---

### 3.4 Distraction

The user starts working but becomes distracted.

FocusPocus should not treat distraction as a simple timer failure.

The application should help determine why the user became distracted and choose an appropriate recovery strategy.

---

### 3.5 Failure to Return

A major problem is not only becoming distracted, but failing to return to the task.

FocusPocus should actively support recovery.

The desired mental model is:

> "I got distracted, but the session is not lost."

---

### 3.6 Poor Session Sizing

Different users may work better with different focus durations.

The application should eventually learn that one user may perform well with 12-minute sessions while another performs better with 35-minute sessions.

FocusPocus should not assume that every user should use a fixed Pomodoro interval.

---

## 4. Core Product Loop

The core loop is:

```text
User describes what they need to do
        ↓
System understands the task
        ↓
System identifies friction
        ↓
Task is clarified or reduced if needed
        ↓
One actionable next step is proposed
        ↓
User starts a focus session
        ↓
User works
        ↓
If distracted, user reports the problem
        ↓
Behavior Engine selects an intervention
        ↓
User returns to the task
        ↓
Session outcome is recorded
        ↓
Short reflection
        ↓
Behavioral data is learned
        ↓
Future sessions become more personalized
```

The product should optimize this loop before expanding into unrelated productivity features.

---

## 5. MVP Scope

The first production-ready MVP should contain only the functionality required to validate the core product hypothesis.

### Required MVP capabilities

1. Authentication
2. Task capture
3. Task listing
4. AI-assisted task understanding
5. AI-assisted task decomposition
6. Focus session creation
7. Focus timer / active focus experience
8. "I'm distracted" flow
9. Basic behavioral intervention selection
10. Session completion
11. Short post-session reflection
12. Basic user progress / session history
13. Turkish localization
14. English localization

The MVP should remain intentionally small.

The MVP is successful if real users repeatedly use the application to start and continue work.

---

## 6. Explicit MVP Non-Goals

Do not add the following unless explicitly requested:

- complex calendar management,
- Kanban boards,
- Gantt charts,
- team collaboration,
- social feeds,
- friends,
- leaderboards,
- marketplace,
- advanced habit tracking,
- note-taking system,
- project-management suite,
- Slack integration,
- Teams integration,
- Discord integration,
- native mobile application,
- browser extension,
- website blocking,
- mobile app blocking,
- advanced gamification,
- AI voice coach,
- multiple productivity methodologies,
- enterprise organization management,
- complex role hierarchy,
- microservices,
- event sourcing.

These may be explored later only when real user behavior shows that they improve the core loop.

---

## 7. Task Capture

Task capture should be extremely low friction.

The user should be able to enter a natural-language task such as:

> "I need to study algorithms but I don't know where to start."

The system should preserve the user's original input.

Task capture should not force the user to define:

- project,
- category,
- priority,
- labels,
- tags,
- estimated duration,
- due date,
- subtasks,
- productivity methodology.

Those can be added later if evidence shows they are valuable.

The primary goal is:

> Capture the user's intention before the user loses momentum.

---

## 8. Task Decomposition

Task decomposition is one of the core capabilities.

The system should prefer concrete, immediately actionable steps.

Bad:

> Study algorithms.

Better:

> Open your Dynamic Programming notes and solve the first example.

Bad:

> Finish the backend.

Better:

> Open the API project and identify the first missing endpoint.

Task decomposition should reduce cognitive load, not create a second planning task.

The system should generally prefer:

- fewer steps,
- smaller steps,
- clear verbs,
- concrete actions,
- realistic durations.

---

## 9. Focus Session

The focus experience should be visually and cognitively minimal.

During an active focus session, prioritize:

- current action,
- remaining time,
- completion action,
- pause / exit only when necessary,
- "I'm distracted" action.

Normal navigation, analytics, settings, dashboards, badges, and unrelated information should not compete for attention.

The focus screen should feel different from the normal application shell.

The product should help the user work, not invite the user to explore the application.

---

## 10. Distraction Recovery

Distraction recovery is a central differentiator.

The user should be able to report:

- the task feels too difficult,
- the task feels too large,
- I do not know what to do next,
- I am bored,
- I am tired,
- my phone / social media distracted me,
- another thought distracted me,
- I feel overwhelmed,
- other.

The application should not simply restart the timer.

The system should choose an intervention based on the reported context.

Possible outcomes may include:

- reduce the task,
- clarify the next action,
- shorten the focus sprint,
- remove friction,
- recommend a short break,
- help the user restart,
- defer the task intentionally.

---

## 11. Behavior Engine

The Behavior Engine is application logic.

It is responsible for selecting an approved behavioral strategy based on structured context.

The LLM must not independently invent behavioral interventions.

Example intervention identifiers:

```text
TASK_DECOMPOSITION
CLARIFY_NEXT_ACTION
SHORTER_SPRINT
REMOVE_FRICTION
DISTRACTION_RECOVERY
BREAK_RECOMMENDATION
RESTART_SESSION
```

The exact set of strategies may evolve.

Each strategy should eventually have:

- a stable identifier,
- a clear purpose,
- conditions under which it may be selected,
- versioning,
- supporting evidence or rationale where appropriate,
- measurable outcome data.

Behavioral strategies should be evidence-informed.

Do not make medical or clinical treatment claims.

FocusPocus is a productivity and behavioral support product, not a medical treatment product.

---

## 12. AI Philosophy

AI should be mostly invisible.

Avoid turning the product into:

- "Ask AI",
- "AI Assistant",
- "AI Coach Chat",
- a chatbot-first experience.

Avoid unnecessary AI visual language such as:

- sparkle icons everywhere,
- purple AI gradients,
- constant "AI generated" labels,
- generic chatbot panels.

The user should experience intelligence through the behavior of the product.

Desired feeling:

> "This system understands what I need."

Not:

> "I am chatting with another AI bot."

---

## 13. AI Responsibilities

AI may be used for:

- understanding natural-language task input,
- classifying task context,
- assisting task decomposition,
- generating concise actionable wording,
- adapting wording to the user's language,
- summarizing behavioral insights,
- assisting non-critical classification.

AI should not own:

- core business invariants,
- authorization decisions,
- persistence rules,
- billing rules,
- session state transitions,
- approved intervention selection logic,
- medical or clinical diagnosis,
- unrestricted psychological advice.

Where practical, AI output should use structured responses instead of unrestricted free text.

---

## 14. Personalization Vision

Long-term personalization is one of the strongest product differentiators.

The system should gradually learn patterns such as:

- preferred focus duration,
- focus completion rate by duration,
- task categories associated with procrastination,
- common distraction reasons,
- time-of-day focus patterns,
- successful recovery interventions,
- task sizes associated with completion,
- frequency of session abandonment,
- preferred language and communication style.

Example learned pattern:

```text
Coding tasks longer than 25 minutes:
43% completion

Coding sessions between 12–15 minutes:
81% completion
```

A future recommendation could then be:

> "You usually do better with shorter starts for this type of task. Let's begin with 12 minutes."

Personalization should be based on actual user behavior rather than arbitrary personality labels.

---

## 15. User Behavioral Profile

A future Focus Profile may include concepts such as:

```text
Task initiation strength
Sustained focus pattern
Preferred session duration
Common friction type
Common distraction trigger
Most effective intervention
Best working windows
Recovery success rate
```

This profile must remain understandable to the user.

Avoid presenting pseudo-scientific scores without clear meaning.

---

## 16. Product Differentiation

FocusPocus should not attempt to win by having more productivity features.

Its differentiation should come from the combination of:

```text
Task initiation
+
Task decomposition
+
Focus execution
+
Distraction recovery
+
Behavioral interventions
+
Long-term personalization
+
Low-friction UX
```

The intended product category is closer to:

> Behavioral execution system

than:

> Todo application

or:

> AI planner

---

## 17. UX Principles

### 17.1 One Dominant Action

Each screen should ideally have one obvious primary action.

The user should rarely wonder:

> "What am I supposed to click?"

---

### 17.2 Minimal Cognitive Load

Avoid:

- excessive navigation,
- unnecessary settings,
- dense dashboards,
- too many cards,
- too many statistics,
- multi-step configuration,
- configuration before value.

---

### 17.3 Progressive Disclosure

Advanced functionality should appear only when needed.

Do not expose every capability at once.

---

### 17.4 Low Maintenance

FocusPocus must not become another productivity system the user needs to maintain.

Avoid requiring the user to constantly:

- organize tasks,
- assign categories,
- manage boards,
- clean up metadata,
- configure workflows.

---

### 17.5 Calm Communication

The application should communicate concisely.

Avoid:

- excessive encouragement,
- long motivational paragraphs,
- childish gamification language,
- judgment,
- guilt,
- aggressive productivity messaging.

Preferred:

> Let's make this smaller.

Avoid:

> Amazing! You're doing fantastic! Let's crush your productivity goals!

---

## 18. Visual Direction

The visual system should feel:

- calm,
- modern,
- premium,
- minimal,
- intentional,
- trustworthy,
- distraction-resistant.

Avoid stereotypical AI SaaS design:

- excessive gradients,
- purple-heavy AI branding,
- glassmorphism everywhere,
- giant rounded cards,
- random shadows,
- sparkle iconography,
- decorative dashboards with no purpose.

The interface should be visually attractive without demanding attention.

---

## 19. Localization

Initial languages:

- Turkish
- English

Localization must be architectural, not an afterthought.

All user-visible interface strings should support localization.

The system should be able to support future separation between:

- interface language,
- coaching / intervention language.

AI-generated wording should feel natural in the selected language rather than being literal translation.

---

## 20. Platform Strategy

### Initial platform

Desktop-first responsive web application.

Primary initial environment:

- desktop browser,
- laptop browser.

### Later possibilities

Only after product validation:

- PWA,
- desktop application,
- mobile companion,
- mobile quick capture,
- cross-device focus support,
- distraction blocking.

The first product should focus on the device where users actually perform work.

---

## 21. Product Metrics

Important metrics include:

### Activation

Percentage of users who successfully create a task and begin their first focus session.

### Start Success Rate

Percentage of users who arrive in a "stuck" state and begin meaningful work within a short period.

### Session Completion Rate

Percentage of started sessions that are completed successfully.

### Recovery Success Rate

Percentage of distraction events after which the user returns to meaningful work.

### D7 Retention

Percentage of users who return within seven days.

### D30 Retention

Percentage of users who remain active after thirty days.

### Paid Conversion

Percentage of active users who become paying users.

---

## 22. Initial Business Goal

The first financial milestone is approximately:

> $400 MRR

The goal is not to maximize scale immediately.

The goal is to prove:

- people receive repeated value,
- people return,
- a meaningful subset is willing to pay.

A small number of happy paying users is more valuable than a large number of inactive signups.

---

## 23. Product Decision Rule

Before adding a feature, ask:

> Does this materially help the user start, stay focused, recover from distraction, or learn how they work?

If the answer is no, the feature should normally remain outside the core product.

---

## 24. Current Product Priority

The immediate priority is not advanced personalization, mobile applications, complex analytics, or growth infrastructure.

The immediate priority is to build the smallest reliable loop:

```text
Authenticate
↓
Capture task
↓
Understand task
↓
Start focus session
↓
Handle distraction
↓
Complete session
↓
Reflect
```

This loop must work well before the product expands.
