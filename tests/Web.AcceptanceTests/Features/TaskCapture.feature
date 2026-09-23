@TaskCapture
Feature: Task capture
    Authenticated users can capture a task and receive a starting recommendation

Scenario: User captures a task using natural-language text
    Given an authenticated user visits the task capture page
    When the user captures a task using their own words
    Then the original task input is shown unchanged on the task detail page
    And a starting recommendation is shown
