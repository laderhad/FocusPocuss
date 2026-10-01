@FocusSession
Feature: Focus session
    Authenticated users can focus on one next action and explicitly complete the session

Scenario Outline: Starting another step does not silently open an unrelated session
    Given an authenticated user has a task start recommendation
    Then starting with an open session for a different "<scope>" requires explicit navigation

    Examples:
        | scope |
        | task  |
        | plan  |

Scenario: User reflects after completing an expired focus session
    Given an authenticated user has a task start recommendation
    When the user starts the focus session
    Then the focused next action is shown without the main navigation
    And the expired focus session remains available to complete
    When the user completes the focus session
    Then the focus session reflection question is shown
    When the user records that they focused well
    Then the focus session is shown as completed

Scenario: User skips reflection after completing a focus session
    Given an authenticated user has a task start recommendation
    When the user starts the focus session
    And the user completes the focus session
    Then the focus session reflection question is shown
    When the user skips the focus session reflection
    Then the focus session is shown as completed

Scenario: User reports why they became distracted
    Given an authenticated user has a task start recommendation
    When the user starts the focus session
    And the user reports an unclear next action distraction
    Then the selected recovery guidance is shown
    When the user returns to focus
    Then the recovery guidance is dismissed and the focus session remains active

Scenario: Recovery follows the server-selected strategy
    Given an authenticated user has a task start recommendation
    When the user starts the focus session
    And the user reports tiredness and the server selects clarification
    Then the selected recovery guidance is shown

Scenario: A parked thought survives reload
    Given an authenticated user has a task start recommendation
    When the user starts the focus session
    And the user parks a thought and returns
    Then the saved thought survives reload without changing the action

Scenario: A tired user can end without claiming completion
    Given an authenticated user has a task start recommendation
    When the user starts the focus session
    And the tired user chooses to end the session
    Then the session ends without a completion celebration

Scenario: Pending clarification survives reload on mobile
    Given an authenticated user has a task start recommendation
    When the user starts the focus session
    Then one clarification can be supplied on mobile after reload
