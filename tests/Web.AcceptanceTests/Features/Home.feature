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
        | dark   | light | rgb(247, 247, 242) | rgb(91, 77, 232)   |
        | light  | dark  | rgb(17, 18, 22)    | rgb(169, 158, 255) |
        | light  | auto  | rgb(247, 247, 242) | rgb(91, 77, 232)   |
        | dark   | auto  | rgb(17, 18, 22)    | rgb(169, 158, 255) |
