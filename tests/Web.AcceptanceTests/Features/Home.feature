@Home
Feature: Home

Scenario: Product promise is displayed
    Given a user visits the home page
    Then the heading "For the moments when starting feels hard." is visible

Scenario: Home copy follows the selected language
    Given a user visits the home page
    When the home language is changed to "tr"
    Then the heading "Başlamanın zor olduğu anlar için." is visible
    When the home language is changed to "en"
    Then the heading "For the moments when starting feels hard." is visible

Scenario Outline: Theme preferences apply to the canvas and primary action
    Given a user visits the home page
    When the system appearance is "<system>" and the selected theme is "<theme>"
    Then the home canvas is "<canvas>" and the primary action is "<primary>"

    Examples:
        | system | theme | canvas             | primary            |
        | dark   | light | rgb(246, 251, 245) | rgb(36, 59, 54)   |
        | light  | dark  | rgb(24, 29, 26)    | rgb(179, 204, 198) |
        | light  | auto  | rgb(246, 251, 245) | rgb(36, 59, 54)   |
        | dark   | auto  | rgb(24, 29, 26)    | rgb(179, 204, 198) |
