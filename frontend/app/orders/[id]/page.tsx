'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { orderService } from '@/lib/services/orderService';
import { checkAdminAction } from '@/app/actions/adminActions';
import Link from 'next/link';
import type { Order, UpdateOrderDto } from '@/types';
import { useTranslations } from 'next-intl';

const ORDER_STATUSES = ['Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled'] as const;

export default function OrderDetailPage() {
  const params = useParams();
  const router = useRouter();
  const t = useTranslations('order');
  const tCommon = useTranslations('common');
  const [order, setOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(true);
  const [isAdmin, setIsAdmin] = useState(false);
  const [updating, setUpdating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    if (params.id) {
      loadOrder(Number(params.id));
      checkAdmin();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [params.id]);

  const checkAdmin = async () => {
    try {
      const result = await checkAdminAction();
      setIsAdmin(result.isAdmin);
    } catch {
      setIsAdmin(false);
    }
  };

  const loadOrder = async (id: number) => {
    try {
      setLoading(true);
      const data = await orderService.getById(id);
      setOrder(data);
    } catch (error) {
      console.error('Error loading order:', error);
      router.push('/dashboard/orders');
    } finally {
      setLoading(false);
    }
  };

  const handleStatusUpdate = async (newStatus: string) => {
    if (!order) return;
    
    try {
      setUpdating(true);
      setError(null);
      setSuccess(null);
      
      const updateDto: UpdateOrderDto = {
        status: newStatus,
        shippingAddress: order.shippingAddress
      };
      
      await orderService.update(order.id, updateDto);
      setSuccess(t('statusUpdatedSuccess'));
      await loadOrder(order.id);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : t('statusUpdateError');
      setError(errorMessage);
      console.error('Error updating order status:', err);
    } finally {
      setUpdating(false);
    }
  };

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">{tCommon('loading')}</div>
      </div>
    );
  }

  if (!order) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">{t('orderNotFound')}</div>
      </div>
    );
  }

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

  return (
    <div className="container mx-auto px-4 py-8">
      <Link href="/dashboard/orders" className="text-blue-600 hover:text-blue-800 mb-4 inline-block">
        ← {t('backToOrders')}
      </Link>

      {error && (
        <div className="mb-4 p-3 bg-red-100 text-red-700 rounded-lg">
          {error}
        </div>
      )}

      {success && (
        <div className="mb-4 p-3 bg-green-100 text-green-700 rounded-lg">
          {success}
        </div>
      )}

      <div className="bg-white rounded-lg shadow-md p-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold text-gray-800">{t('orderDetails')}</h1>
          <div className="flex items-center gap-4">
            <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(order.status)}`}>
              {t(`status.${order.status.toLowerCase()}`)}
            </span>
            {isAdmin && (
              <select
                value={order.status}
                onChange={(e) => handleStatusUpdate(e.target.value)}
                disabled={updating}
                className="px-3 py-1 border border-gray-300 rounded-md text-sm bg-white"
              >
                {ORDER_STATUSES.map((status) => (
                  <option key={status} value={status}>
                    {t(`status.${status.toLowerCase()}`)}
                  </option>
                ))}
              </select>
            )}
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
          <div>
            <h2 className="text-xl font-semibold text-gray-800 mb-2">{t('orderInformation')}</h2>
            <p className="text-gray-600">
              <strong>{t('orderNumber')}:</strong> {order.orderNumber}
            </p>
            <p className="text-gray-600">
              <strong>{tCommon('date')}:</strong> {new Date(order.orderDate).toLocaleDateString('fa-IR')}
            </p>
            <p className="text-gray-600">
              <strong>{tCommon('amount')}:</strong>{' '}
              <span className="text-green-600 font-bold">
                {order.totalAmount.toLocaleString('fa-IR')} {t('currency')}
              </span>
            </p>
          </div>

          <div>
            <h2 className="text-xl font-semibold text-gray-800 mb-2">{t('shippingInformation')}</h2>
            <p className="text-gray-600">
              <strong>{t('customer')}:</strong> {order.customerName}
            </p>
            <p className="text-gray-600">
              <strong>{t('email')}:</strong> {order.customerEmail}
            </p>
            {order.shippingAddress && (
              <p className="text-gray-600">
                <strong>{t('address')}:</strong> {order.shippingAddress}
              </p>
            )}
          </div>
        </div>

        <div>
          <h2 className="text-xl font-semibold text-gray-800 mb-4">{t('products')}</h2>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    {t('product')}
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    {t('unitPrice')}
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    {t('quantity')}
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    {t('total')}
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {order.orderItems.map((item) => (
                  <tr key={item.id}>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      {item.productName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {item.unitPrice.toLocaleString('fa-IR')} {t('currency')}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {item.quantity}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {item.totalPrice.toLocaleString('fa-IR')} {t('currency')}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
}

