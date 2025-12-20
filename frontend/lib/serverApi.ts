'use server';

import { cookies } from 'next/headers';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5028/api';

/**
 * تابع مشترک برای ارسال درخواست‌های HTTP در server actions
 * این تابع cookies را مدیریت می‌کند تا session authentication کار کند
 */
export async function fetchAPI<T>(endpoint: string, options?: RequestInit): Promise<T> {
  const cookieStore = await cookies();
  
  // دریافت cookies از cookieStore (که از مرورگر یا request قبلی آمده)
  // ساخت cookie header string از cookies موجود
  const cookiePairs: string[] = [];
  cookieStore.getAll().forEach(cookie => {
    cookiePairs.push(`${cookie.name}=${cookie.value}`);
  });
  const cookieHeader = cookiePairs.join('; ');
  
  // Debug: لاگ cookies ارسالی
  if (cookieHeader) {
    console.log('[serverApi] Sending cookies to API:', cookieHeader);
  } else {
    console.log('[serverApi] No cookies to send');
  }

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(cookieHeader && { Cookie: cookieHeader }),
      ...options?.headers,
    },
    credentials: 'include',
  });

  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || `HTTP error! status: ${response.status}`);
  }

  // دریافت و ذخیره cookies از response
  // ASP.NET Core session cookie را از response می‌گیریم
  let setCookieHeaders: string[] = [];
  
  // استفاده از getSetCookie اگر موجود باشد (Node.js 18+)
  if (typeof response.headers.getSetCookie === 'function') {
    setCookieHeaders = response.headers.getSetCookie();
  } else {
    // Fallback: استفاده از get('set-cookie')
    const setCookieHeader = response.headers.get('set-cookie');
    if (setCookieHeader) {
      setCookieHeaders = Array.isArray(setCookieHeader) ? setCookieHeader : [setCookieHeader];
    }
  }
  
  // Debug: لاگ cookies دریافتی
  if (setCookieHeaders && setCookieHeaders.length > 0) {
    console.log('[serverApi] Received cookies:', setCookieHeaders);
  }
  
  if (setCookieHeaders && setCookieHeaders.length > 0) {
    for (const cookieString of setCookieHeaders) {
      if (!cookieString || !cookieString.includes('=')) continue;
      
      // Parse cookie string: ".AspNetCore.Session=abc123; path=/; httponly; samesite=lax"
      const parts = cookieString.split(';').map((p: string) => p.trim());
      const [nameValue] = parts;
      
      if (!nameValue || !nameValue.includes('=')) continue;
      
      const equalIndex = nameValue.indexOf('=');
      const name = nameValue.substring(0, equalIndex).trim();
      const value = nameValue.substring(equalIndex + 1).trim();
      
      if (!name || !value) continue;
      
      // Extract cookie options
      const cookieOptions: {
        httpOnly?: boolean;
        secure?: boolean;
        sameSite?: 'strict' | 'lax' | 'none';
        path?: string;
        maxAge?: number;
        expires?: Date;
      } = {
        path: '/',
        sameSite: 'lax',
      };
      
      for (let i = 1; i < parts.length; i++) {
        const part = parts[i].toLowerCase().trim();
        if (part === 'httponly') {
          cookieOptions.httpOnly = true;
        } else if (part === 'secure') {
          cookieOptions.secure = true;
        } else if (part.startsWith('samesite=')) {
          const sameSiteValue = part.split('=')[1]?.trim();
          if (sameSiteValue === 'strict' || sameSiteValue === 'lax' || sameSiteValue === 'none') {
            cookieOptions.sameSite = sameSiteValue as 'strict' | 'lax' | 'none';
          }
        } else if (part.startsWith('path=')) {
          cookieOptions.path = part.split('=')[1]?.trim() || '/';
        } else if (part.startsWith('max-age=')) {
          const maxAge = parseInt(part.split('=')[1]?.trim() || '0', 10);
          if (!isNaN(maxAge) && maxAge > 0) {
            cookieOptions.maxAge = maxAge;
          }
        } else if (part.startsWith('expires=')) {
          const expiresStr = part.split('=').slice(1).join('=').trim();
          const expiresDate = new Date(expiresStr);
          if (!isNaN(expiresDate.getTime())) {
            cookieOptions.expires = expiresDate;
          }
        }
      }
      
      // Set cookie with all options
      // برای session cookies معمولاً httpOnly و path=/ نیاز است
      try {
        const finalOptions = {
          httpOnly: cookieOptions.httpOnly ?? true,
          secure: cookieOptions.secure ?? (process.env.NODE_ENV === 'production'),
          sameSite: cookieOptions.sameSite ?? 'lax',
          path: cookieOptions.path ?? '/',
          ...(cookieOptions.maxAge && { maxAge: cookieOptions.maxAge }),
          ...(cookieOptions.expires && { expires: cookieOptions.expires }),
        };
        
        cookieStore.set(name, value, finalOptions);
        console.log('[serverApi] Set cookie:', name, 'with options:', finalOptions);
      } catch (error) {
        // Log error but don't fail the request
        console.error('[serverApi] Error setting cookie:', name, error);
      }
    }
  }

  return response.json();
}

