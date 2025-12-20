'use client';

import { useRouter } from 'next/navigation';
import { authService } from '@/lib/services/authService';
import Link from 'next/link';
import type { User, Order } from '@/types';
import { useTranslations } from 'next-intl';

interface DashboardClientProps {
  initialUser: User;
  initialOrders: Order[];
}

export default function DashboardClient({ initialUser, initialOrders }: DashboardClientProps) {
  const router = useRouter();
  const t = useTranslations('dashboard');
  const tCommon = useTranslations('common');
  const tCurrency = useTranslations('currency');

  const handleLogout = async () => {
    try {
      await authService.logout();
      router.push('/');
    } catch (error) {
      console.error('Logout error:', error);
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800">{t('title')}</h1>
        <button
          onClick={handleLogout}
          className="bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700"
        >
          {t('logout')}
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-2">{t('userInfo')}</h2>
          <p className="text-gray-600">{initialUser.firstName} {initialUser.lastName}</p>
          <p className="text-gray-600">{initialUser.email}</p>
          <Link
            href="/dashboard/edit"
            className="text-blue-600 hover:text-blue-800 mt-2 inline-block"
          >
            {t('editProfile')}
          </Link>
        </div>

        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-2">{t('orders')}</h2>
          <p className="text-3xl font-bold text-blue-600">{initialOrders.length}</p>
          <Link
            href="/dashboard/orders"
            className="text-blue-600 hover:text-blue-800 mt-2 inline-block"
          >
            {t('viewAll')}
          </Link>
        </div>

        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-2">{tCommon('operations')}</h2>
          <Link
            href="/products-list"
            className="block text-blue-600 hover:text-blue-800 mb-2"
          >
            {t('viewProducts')}
          </Link>
          <Link
            href="/cart"
            className="block text-blue-600 hover:text-blue-800 mb-2"
          >
            {t('cart')}
          </Link>
          {initialUser.roles?.includes('Admin') && (
            <Link
              href="/admin"
              className="block text-purple-600 hover:text-purple-800 font-semibold"
            >
              {t('adminPanel')}
            </Link>
          )}
        </div>
      </div>

      <div className="bg-white rounded-lg shadow-md p-6">
        <h2 className="text-xl font-semibold text-gray-800 mb-4">{t('recentOrders')}</h2>
        {initialOrders.length === 0 ? (
          <p className="text-gray-600">{t('noOrders')}</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    {tCommon('orderNumber')}
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    {tCommon('date')}
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    {tCommon('amount')}
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    {tCommon('status')}
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    {tCommon('operations')}
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {initialOrders.slice(0, 5).map((order) => (
                  <tr key={order.id}>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      {order.orderNumber}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {new Date(order.orderDate).toLocaleDateString('fa-IR')}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {order.totalAmount.toLocaleString('fa-IR')} {tCurrency('toman')}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className="px-2 py-1 text-xs rounded-full bg-blue-100 text-blue-800">
                        {order.status}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                      <Link
                        href={`/orders/${order.id}`}
                        className="text-blue-600 hover:text-blue-900"
                      >
                        {tCommon('view')}
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}

