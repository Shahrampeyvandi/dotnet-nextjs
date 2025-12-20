'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useEffect, useState } from 'react';
import { checkAdminAction } from '@/app/actions/adminActions';
import LanguageSwitcher from './LanguageSwitcher';
import { useTranslations } from 'next-intl';
import { User } from '@/types';

interface UserDto {
  user?: User
  isAdmin: boolean
}
export default function Navigation({user}: {user: UserDto}) {
  const pathname = usePathname();
  const t = useTranslations('navigation');
  const [isAdmin, setIsAdmin] = useState(false);

  useEffect(() => {
    console.log(user);
    
    console.log('set isadmin');
    
    setIsAdmin(user.isAdmin)
  }, [user]);

  const links = [
    { href: '/', label: t('home') },
    { href: '/products-list', label: t('products') },
    { href: '/cart', label: t('cart') },
  ];

  return (
    <nav className="bg-blue-600 text-white shadow-lg">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <div className="flex items-center space-x-8">
            <Link href="/" className="text-xl font-bold">
              {t('storeName')}
            </Link>
            <Link
              href="/dashboard"
              className="px-3 py-2 rounded-md text-sm font-medium hover:bg-blue-700"
            >
              {t('dashboard')}
            </Link>
            {isAdmin && (
              <Link
                href="/admin"
                className="px-3 py-2 rounded-md text-sm font-medium hover:bg-blue-700 bg-blue-800"
              >
                {t('adminPanel')}
              </Link>
            )}
            <div className="flex space-x-4">
              {links.map((link) => (
                <Link
                  key={link.href}
                  href={link.href}
                  className={`px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                    pathname === link.href
                      ? 'bg-blue-700'
                      : 'hover:bg-blue-700'
                  }`}
                >
                  {link.label}
                </Link>
              ))}
            </div>
          </div>
          <LanguageSwitcher />
        </div>
      </div>
    </nav>
  );
}

