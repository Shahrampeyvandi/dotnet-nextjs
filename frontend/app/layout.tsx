import type { Metadata } from "next";
import localFont from "next/font/local";
import "./globals.css";
import Navigation from "@/components/Navigation";
import { NextIntlClientProvider } from 'next-intl';
import { getMessages } from 'next-intl/server';
import { cookies } from 'next/headers';
import { defaultLocale, locales, type Locale } from '@/i18n';
import { checkAdminAction } from "./actions/adminActions";

// فونت فارسی IRANSans
const iranSans = localFont({
  src: [
    {
      path: "./fonts/IRANSansWeb(FaNum)_UltraLight.woff2",
      weight: "100",
      style: "normal",
    },
    {
      path: "./fonts/IRANSansWeb(FaNum)_Light.woff2",
      weight: "300",
      style: "normal",
    },
    {
      path: "./fonts/IRANSansWeb(FaNum).woff2",
      weight: "400",
      style: "normal",
    },
    {
      path: "./fonts/IRANSansWeb(FaNum)_Medium.woff2",
      weight: "500",
      style: "normal",
    },
    {
      path: "./fonts/IRANSansWeb(FaNum)_Bold.woff2",
      weight: "700",
      style: "normal",
    },
  ],
  variable: "--font-iran-sans",
  display: "swap",
});

export const metadata: Metadata = {
  title: "فروشگاه آنلاین",
  description: "سیستم مدیریت فروشگاه آنلاین",
};

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  // Get locale from cookie (set by middleware or language switcher)
  const cookieStore = await cookies();
  const cookieLocale = cookieStore.get('NEXT_LOCALE')?.value;
  const locale = (cookieLocale && locales.includes(cookieLocale as Locale))
    ? (cookieLocale as Locale)
    : defaultLocale;
  
  // Get messages for the locale
  // We need to pass locale explicitly since i18n.ts might not have it
  const messages = await getMessages({ locale });
  const user = await checkAdminAction();
  
  return (
    <html lang={locale} dir={locale === 'fa' ? 'rtl' : 'ltr'}>
      <body
        className={`${iranSans.variable} antialiased`}
      >
        <NextIntlClientProvider messages={messages} locale={locale}>
        <Navigation user={user}/>
        <main className="min-h-screen bg-gray-50">{children}</main>
        </NextIntlClientProvider>
      </body>
    </html>
  );
}
