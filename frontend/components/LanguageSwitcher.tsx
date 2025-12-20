'use client';

import { useLocale } from 'next-intl';
import { useRouter } from 'next/navigation';
import { locales, type Locale } from '@/i18n';

export default function LanguageSwitcher() {
  const locale = useLocale() as Locale;
  const router = useRouter();

  const switchLocale = (newLocale: Locale) => {
    if (newLocale === locale) return;
    
    // Set locale in cookie
    document.cookie = `NEXT_LOCALE=${newLocale}; path=/; max-age=31536000; SameSite=Lax`;
    
    // Reload the page to apply new locale
    window.location.reload();
  };

  const languageNames: Record<Locale, string> = {
    fa: 'فارسی',
    en: 'English'
  };

  return (
    <div className="flex items-center gap-2">
      {locales.map((loc) => (
        <button
          key={loc}
          onClick={() => switchLocale(loc)}
          className={`px-3 py-1 rounded-md text-sm font-medium transition-colors ${
            locale === loc
              ? 'bg-blue-700 text-white'
              : 'bg-blue-500 text-white hover:bg-blue-600'
          }`}
        >
          {languageNames[loc]}
        </button>
      ))}
    </div>
  );
}

