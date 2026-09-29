@FocusSession
Feature: Focus session
    Authenticated users can focus on one next action and explicitly complete the session

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
