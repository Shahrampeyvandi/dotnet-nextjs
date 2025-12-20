import { NextRequest, NextResponse } from 'next/server';
import { locales, defaultLocale, type Locale } from './i18n';

export function middleware(request: NextRequest) {
  // Read locale from cookie
  const cookieLocale = request.cookies.get('NEXT_LOCALE')?.value;
  const locale = (cookieLocale && locales.includes(cookieLocale as Locale))
    ? cookieLocale
    : defaultLocale;

  // Create response
  const response = NextResponse.next();
  
  // Set locale in cookie if not set (for first visit)
  if (!cookieLocale) {
    response.cookies.set('NEXT_LOCALE', locale, {
      path: '/',
      maxAge: 60 * 60 * 24 * 365, // 1 year
      sameSite: 'lax'
    });
  }
  
  // Set locale in header for next-intl to read
  response.headers.set('x-next-intl-locale', locale);
  
  return response;
}

export const config = {
  matcher: [
    // Match all pathnames except for
    // - … if they start with `/api`, `/_next` or `/_vercel`
    // - … the ones containing a dot (e.g. `favicon.ico`)
    '/((?!api|_next|_vercel|.*\\..*).*)'
  ]
};

