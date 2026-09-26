@FocusSession
Feature: Focus session
    Authenticated users can focus on one next action and explicitly complete the session

Scenario: User completes an expired focus session explicitly
    Given an authenticated user has a task start recommendation
    When the user starts the focus session
    Then the focused next action is shown without the main navigation
    And the expired focus session remains available to complete
    When the user completes the focus session
    Then the focus session is shown as completed
