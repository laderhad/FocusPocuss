@SessionHistory
Feature: Focus session history

    Authenticated users can review their recent focus sessions.

Scenario: User reviews focus sessions newest first
    Given an authenticated user has focus session history
    When the user opens focus session history
    Then completed and active sessions are shown newest first
    And the saved reflection is shown
