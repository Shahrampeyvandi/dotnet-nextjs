'use client';

import { useEffect, useState } from 'react';
import { orderService } from '@/lib/services/orderService';
import type { Order, PaginatedResponse } from '@/types';
import { useTranslations } from 'next-intl';
import Link from 'next/link';

export default function AdminOrdersClient() {
  const t = useTranslations('adminOrders');
  const tOrder = useTranslations('order');
  const tCommon = useTranslations('common');
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(10);
  const [pagination, setPagination] = useState<{
    totalCount: number;
    totalPages: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
  }>({
    totalCount: 0,
    totalPages: 0,
    hasPreviousPage: false,
    hasNextPage: false,
  });

  useEffect(() => {
    loadOrders();
  }, [pageNumber]);

  const loadOrders = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await orderService.getPaginated(pageNumber, pageSize);
      setOrders(response.data);
      setPagination({
        totalCount: response.totalCount,
        totalPages: response.totalPages,
        hasPreviousPage: response.hasPreviousPage,
        hasNextPage: response.hasNextPage,
      });
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : t('errorLoadingOrders');
      setError(errorMessage);
      console.error('Error loading orders:', err);
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'Processing':
        return 'bg-blue-100 text-blue-800';
      case 'Shipped':
        return 'bg-purple-100 text-purple-800';
      case 'Delivered':
        return 'bg-green-100 text-green-800';
      case 'Cancelled':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  if (loading && orders.length === 0) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">{tCommon('loading')}</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h2 className="text-2xl font-bold text-gray-800 mb-6">{t('title')}</h2>

      {error && (
        <div className="mb-4 p-3 bg-red-100 text-red-700 rounded-lg">
          {error}
        </div>
      )}

      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  {tOrder('orderNumber')}
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  {tOrder('customer')}
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
              {orders.map((order) => (
                <tr key={order.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {order.orderNumber}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {order.customerName || '-'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {new Date(order.orderDate).toLocaleDateString('fa-IR')}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {order.totalAmount.toLocaleString('fa-IR')} {tOrder('currency')}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm">
                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(order.status)}`}>
                      {tOrder(`status.${order.status.toLowerCase()}`)}
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

        {/* Pagination */}
        {pagination.totalPages > 1 && (
          <div className="bg-gray-50 px-4 py-3 flex items-center justify-between border-t border-gray-200">
            <div className="flex-1 flex justify-between sm:hidden">
              <button
                onClick={() => setPageNumber(pageNumber - 1)}
                disabled={!pagination.hasPreviousPage || loading}
                className="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {t('previous')}
              </button>
              <button
                onClick={() => setPageNumber(pageNumber + 1)}
                disabled={!pagination.hasNextPage || loading}
                className="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {t('next')}
              </button>
            </div>
            <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
              <div>
                <p className="text-sm text-gray-700">
                  {t('showing')} <span className="font-medium">{(pageNumber - 1) * pageSize + 1}</span> {t('to')}{' '}
                  <span className="font-medium">
                    {Math.min(pageNumber * pageSize, pagination.totalCount)}
                  </span>{' '}
                  {t('of')} <span className="font-medium">{pagination.totalCount}</span> {t('results')}
                </p>
              </div>
              <div>
                <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
                  <button
                    onClick={() => setPageNumber(pageNumber - 1)}
                    disabled={!pagination.hasPreviousPage || loading}
                    className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {t('previous')}
                  </button>
                  {Array.from({ length: pagination.totalPages }, (_, i) => i + 1).map((page) => {
                    // Show first page, last page, current page, and pages around current
                    if (
                      page === 1 ||
                      page === pagination.totalPages ||
                      (page >= pageNumber - 1 && page <= pageNumber + 1)
                    ) {
                      return (
                        <button
                          key={page}
                          onClick={() => setPageNumber(page)}
                          disabled={loading}
                          className={`relative inline-flex items-center px-4 py-2 border text-sm font-medium ${
                            page === pageNumber
                              ? 'z-10 bg-blue-50 border-blue-500 text-blue-600'
                              : 'bg-white border-gray-300 text-gray-500 hover:bg-gray-50'
                          } disabled:opacity-50 disabled:cursor-not-allowed`}
                        >
                          {page}
                        </button>
                      );
                    } else if (page === pageNumber - 2 || page === pageNumber + 2) {
                      return (
                        <span key={page} className="relative inline-flex items-center px-4 py-2 border border-gray-300 bg-white text-sm font-medium text-gray-700">
                          ...
                        </span>
                      );
                    }
                    return null;
                  })}
                  <button
                    onClick={() => setPageNumber(pageNumber + 1)}
                    disabled={!pagination.hasNextPage || loading}
                    className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {t('next')}
                  </button>
                </nav>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

