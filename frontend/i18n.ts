import { getRequestConfig } from 'next-intl/server';

// Can be imported from a shared config
export const locales = ['fa', 'en'] as const;
export type Locale = (typeof locales)[number];
export const defaultLocale: Locale = 'fa';

export default getRequestConfig(async ({ requestLocale }) => {
  // Use requestLocale if provided, otherwise default
  // Since we're using custom middleware, requestLocale might be undefined
  // In that case, the layout will handle locale detection from cookies
  const resolvedLocale = requestLocale ? await Promise.resolve(requestLocale) : undefined;
  let locale: string = resolvedLocale || defaultLocale;
  
  // Validate locale
  if (!locales.includes(locale as Locale)) {
    locale = defaultLocale;
  }

  const validLocale = locale as Locale;

  return {
    locale: validLocale,
    messages: (await import(`./messages/${validLocale}.json`)).default
  };
});

