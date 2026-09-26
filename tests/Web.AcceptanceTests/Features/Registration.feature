@Registration
Feature: Registration
    Users receive clear validation and can create an account

Scenario: User is shown the complete password requirements
    Given a logged out user visits the registration page
    When the user submits registration with a weak password
    Then the complete password requirements are shown

Scenario: User registers with valid credentials
    Given a logged out user visits the registration page
    When the user registers with valid credentials
    Then the login page is shown

Scenario: User cannot register an existing email address
    Given a logged out user visits the registration page
    When the user registers with an existing email address
    Then an email already in use error is shown
