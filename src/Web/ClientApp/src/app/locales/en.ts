export const en = {
  language: {
    label: 'Language',
  },
  navigation: {
    home: 'Home',
    counter: 'Counter',
    weather: 'Weather',
    tasks: 'Tasks',
    login: 'Log in',
    register: 'Register',
    logout: 'Log out',
  },
  theme: {
    auto: 'Use system theme',
    light: 'Use light theme',
    dark: 'Use dark theme',
  },
  auth: {
    login: {
      title: 'Log in',
      email: 'Email',
      password: 'Password',
      submit: 'Log in',
      invalid: 'Invalid email or password.',
      noAccount: "Don't have an account?",
      registerLink: 'Register',
    },
    register: {
      title: 'Register',
      email: 'Email',
      password: 'Password',
      submit: 'Register',
      failed: 'Registration failed. Please try again.',
      invalidEmail: 'Please enter a valid email address.',
      passwordMinimum: 'Password must be at least {{count}} characters.',
      hasAccount: 'Already have an account?',
      loginLink: 'Log in',
    },
  },
} as const;
