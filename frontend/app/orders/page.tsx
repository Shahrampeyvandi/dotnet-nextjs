'use client';

import { useEffect, useState } from 'react';
import { orderService } from '@/lib/services/orderService';
import { customerService } from '@/lib/services/customerService';
import { productService } from '@/lib/services/productService';
import type { Order, CreateOrderDto, Customer, Product } from '@/types';
import OrderModal from '@/components/OrderModal';

export default function OrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);

  useEffect(() => {
    loadOrders();
  }, []);

  const loadOrders = async () => {
    try {
      setLoading(true);
      const data = await orderService.getAll();
      setOrders(data);
    } catch (error) {
      console.error('Error loading orders:', error);
      alert('خطا در بارگذاری سفارشات');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: CreateOrderDto) => {
    try {
      await orderService.create(data);
      await loadOrders();
      setIsModalOpen(false);
    } catch (error) {
      console.error('Error creating order:', error);
      alert('خطا در ایجاد سفارش');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('آیا از حذف این سفارش اطمینان دارید؟')) return;

    try {
      await orderService.delete(id);
      await loadOrders();
    } catch (error) {
      console.error('Error deleting order:', error);
      alert('خطا در حذف سفارش');
    }
  };

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">در حال بارگذاری...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800">سفارشات</h1>
        <button
          onClick={() => setIsModalOpen(true)}
          className="bg-orange-600 text-white px-4 py-2 rounded-lg hover:bg-orange-700 transition-colors"
        >
          ایجاد سفارش
        </button>
      </div>

      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                شماره سفارش
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                مشتری
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                تاریخ
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                مبلغ کل
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                وضعیت
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                عملیات
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
                  {order.customerName}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {new Date(order.orderDate).toLocaleDateString('fa-IR')}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {order.totalAmount.toLocaleString('fa-IR')} تومان
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <span className="px-2 py-1 text-xs rounded-full bg-blue-100 text-blue-800">
                    {order.status}
                  </span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <button
                    onClick={() => handleDelete(order.id)}
                    className="text-red-600 hover:text-red-900"
                  >
                    حذف
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {isModalOpen && (
        <OrderModal
          onClose={() => setIsModalOpen(false)}
          onSave={handleCreate}
        />
      )}
    </div>
  );
}

