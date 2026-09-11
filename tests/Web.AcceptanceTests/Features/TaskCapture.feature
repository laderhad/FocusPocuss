@TaskCapture
Feature: Task capture
    Authenticated users can capture a task and see it in their task history

Scenario: User captures a task using natural-language text
    Given an authenticated user visits the task capture page
    When the user captures a task using their own words
    Then the original task input is shown unchanged in the task history
