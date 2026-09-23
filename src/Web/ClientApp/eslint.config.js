import eslint from '@eslint/js';
import { defineConfig, globalIgnores } from 'eslint/config';
import react from 'eslint-plugin-react';
import reactHooks from 'eslint-plugin-react-hooks';
import reactRefresh from 'eslint-plugin-react-refresh';
import globals from 'globals';
import tseslint from 'typescript-eslint';

const reactConfigs = [
  {
    plugins: { react },
    rules: { 'react/jsx-uses-vars': 'error' },
  },
  reactHooks.configs.flat.recommended,
  reactRefresh.configs.vite,
];

const compatibilityRules = {
  'react-hooks/set-state-in-effect': 'off',
  'react-refresh/only-export-components': 'warn',
};

export default defineConfig([
  globalIgnores(['build', 'src/web-api-client.ts']),
  {
    files: ['src/**/*.{js,jsx}'],
    extends: [eslint.configs.recommended, ...reactConfigs],
    languageOptions: {
      ecmaVersion: 'latest',
      globals: globals.browser,
      parserOptions: {
        ecmaFeatures: { jsx: true },
      },
    },
    rules: compatibilityRules,
  },
  {
    files: ['src/**/*.{ts,tsx}'],
    extends: [
      eslint.configs.recommended,
      ...tseslint.configs.recommended,
      ...reactConfigs,
    ],
    languageOptions: {
      ecmaVersion: 'latest',
      globals: globals.browser,
    },
    rules: compatibilityRules,
  },
]);
