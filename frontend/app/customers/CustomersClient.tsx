'use client';

import { useEffect, useState } from 'react';
import { customerService } from '@/lib/services/customerService';
import type { Customer, CreateCustomerDto } from '@/types';
import CustomerModal from '@/components/CustomerModal';

export default function CustomersClient() {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingCustomer, setEditingCustomer] = useState<Customer | null>(null);

  useEffect(() => {
    loadCustomers();
  }, []);

  const loadCustomers = async () => {
    try {
      setLoading(true);
      const data = await customerService.getAll();
      setCustomers(data);
    } catch (error) {
      console.error('Error loading customers:', error);
      alert('خطا در بارگذاری مشتریان');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: CreateCustomerDto) => {
    try {
      await customerService.create(data);
      await loadCustomers();
      setIsModalOpen(false);
    } catch (error) {
      console.error('Error creating customer:', error);
      alert('خطا در ایجاد مشتری');
    }
  };

  const handleUpdate = async (id: number, data: CreateCustomerDto) => {
    try {
      await customerService.update(id, data);
      await loadCustomers();
      setIsModalOpen(false);
      setEditingCustomer(null);
    } catch (error) {
      console.error('Error updating customer:', error);
      alert('خطا در به‌روزرسانی مشتری');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('آیا از حذف این مشتری اطمینان دارید؟')) return;

    try {
      await customerService.delete(id);
      await loadCustomers();
    } catch (error) {
      console.error('Error deleting customer:', error);
      alert('خطا در حذف مشتری');
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
        <h1 className="text-3xl font-bold text-gray-800">مشتریان</h1>
        <button
          onClick={() => {
            setEditingCustomer(null);
            setIsModalOpen(true);
          }}
          className="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors"
        >
          افزودن مشتری
        </button>
      </div>

      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                نام
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                ایمیل
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                تلفن
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                شهر
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                عملیات
              </th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {customers.map((customer) => (
              <tr key={customer.id}>
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                  {customer.firstName} {customer.lastName}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {customer.email}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {customer.phone || '-'}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {customer.city || '-'}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <button
                    onClick={() => {
                      setEditingCustomer(customer);
                      setIsModalOpen(true);
                    }}
                    className="text-blue-600 hover:text-blue-900 mr-4"
                  >
                    ویرایش
                  </button>
                  <button
                    onClick={() => handleDelete(customer.id)}
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
        <CustomerModal
          customer={editingCustomer}
          onClose={() => {
            setIsModalOpen(false);
            setEditingCustomer(null);
          }}
          onSave={editingCustomer
            ? (data) => handleUpdate(editingCustomer.id, data)
            : handleCreate}
        />
      )}
    </div>
  );
}

