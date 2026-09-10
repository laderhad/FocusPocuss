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
  tasks: {
    title: 'Görevler',
    subtitle: 'Aklındayken yapman gerekeni kendi sözlerinle yaz.',
    capture: {
      label: 'Ne yapman gerekiyor?',
      placeholder: 'Kendi sözlerinle yaz...',
      submit: 'Görevi kaydet',
      submitting: 'Kaydediliyor...',
      saveError: 'Görev kaydedilemedi. Tekrar dene.',
      validation: {
        required: 'Yapman gerekeni yaz.',
        tooLong: 'Görev en fazla {{count}} karakter olabilir.',
      },
    },
    history: {
      title: 'Son görevler',
      loading: 'Görevler yükleniyor...',
      loadError: 'Görevler yüklenemedi.',
      retry: 'Tekrar dene',
      empty: 'Henüz kaydedilmiş görev yok.',
    },
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
