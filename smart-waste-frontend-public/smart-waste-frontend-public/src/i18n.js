import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import Backend from 'i18next-http-backend';

export const langs = ["en", "hr", "no", "fr", "de", "ro", "pt", "es", "sv", "it"]


i18n
  .use(Backend)
  .use(initReactI18next)
  .init({
    supportedLngs: langs,
    nonExplicitSupportedLngs: true,
    fallbackLng: "en",
    backend: {
      loadPath: "/locales/{{lng}}/{{ns}}.json"
    },
    interpolation: {
      escapeValue: false
    },
    debug: false
  });
export default i18n;