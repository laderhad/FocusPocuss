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
  tasks: {
    title: 'Tasks',
    subtitle: 'Write down what you need to do while it is on your mind.',
    capture: {
      label: 'What do you need to do?',
      placeholder: 'Describe it in your own words...',
      submit: 'Save task',
      submitting: 'Saving...',
      saveError: 'The task could not be saved. Try again.',
      validation: {
        required: 'Enter what you need to do.',
        tooLong: 'The task cannot exceed {{count}} characters.',
      },
    },
    history: {
      title: 'Recent tasks',
      loading: 'Loading tasks...',
      loadError: 'Tasks could not be loaded.',
      retry: 'Try again',
      empty: 'No saved tasks yet.',
    },
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
