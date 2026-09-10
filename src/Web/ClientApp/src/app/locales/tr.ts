export const tr = {
  language: {
    label: 'Dil',
  },
  navigation: {
    home: 'Ana sayfa',
    counter: 'Sayaç',
    weather: 'Hava durumu',
    tasks: 'Görevler',
    login: 'Giriş yap',
    register: 'Kayıt ol',
    logout: 'Çıkış yap',
  },
  theme: {
    auto: 'Sistem temasını kullan',
    light: 'Açık temayı kullan',
    dark: 'Koyu temayı kullan',
  },
  auth: {
    login: {
      title: 'Giriş yap',
      email: 'E-posta',
      password: 'Parola',
      submit: 'Giriş yap',
      invalid: 'E-posta veya parola geçersiz.',
      noAccount: 'Hesabın yok mu?',
      registerLink: 'Kayıt ol',
    },
    register: {
      title: 'Kayıt ol',
      email: 'E-posta',
      password: 'Parola',
      submit: 'Kayıt ol',
      failed: 'Kayıt tamamlanamadı. Lütfen tekrar dene.',
      invalidEmail: 'Geçerli bir e-posta adresi gir.',
      passwordMinimum: 'Parola en az {{count}} karakter olmalı.',
      hasAccount: 'Zaten hesabın var mı?',
      loginLink: 'Giriş yap',
    },
  },
} as const;
