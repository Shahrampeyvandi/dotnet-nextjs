'use client';

import { useState, useEffect } from 'react';
import { customerService } from '@/lib/services/customerService';
import { productService } from '@/lib/services/productService';
import type { CreateOrderDto, CreateOrderItemDto, Customer, Product } from '@/types';

interface OrderModalProps {
  onClose: () => void;
  onSave: (data: CreateOrderDto) => void;
}

export default function OrderModal({ onClose, onSave }: OrderModalProps) {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [customerId, setCustomerId] = useState('');
  const [shippingAddress, setShippingAddress] = useState('');
  const [orderItems, setOrderItems] = useState<CreateOrderItemDto[]>([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [customersData, productsData] = await Promise.all([
        customerService.getAll(),
        productService.getAll(),
      ]);
      setCustomers(customersData);
      setProducts(productsData.filter(p => p.isActive));
    } catch (error) {
      console.error('Error loading data:', error);
      alert('خطا در بارگذاری اطلاعات');
    }
  };

  const addOrderItem = () => {
    setOrderItems([...orderItems, { productId: 0, quantity: 1 }]);
  };

  const removeOrderItem = (index: number) => {
    setOrderItems(orderItems.filter((_, i) => i !== index));
  };

  const updateOrderItem = (index: number, field: keyof CreateOrderItemDto, value: number) => {
    const updated = [...orderItems];
    updated[index] = { ...updated[index], [field]: value };
    setOrderItems(updated);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (orderItems.length === 0) {
      alert('لطفا حداقل یک محصول اضافه کنید');
      return;
    }
    onSave({ customerId: parseInt(customerId), shippingAddress, orderItems });
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-3xl max-h-[90vh] overflow-y-auto">
        <h2 className="text-2xl font-bold mb-4">ایجاد سفارش</h2>
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              مشتری
            </label>
            <select
              value={customerId}
              onChange={(e) => setCustomerId(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-md"
              required
            >
              <option value="">انتخاب کنید</option>
              {customers.map((customer) => (
                <option key={customer.id} value={customer.id}>
                  {customer.firstName} {customer.lastName} - {customer.email}
                </option>
              ))}
            </select>
          </div>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              آدرس ارسال
            </label>
            <textarea
              value={shippingAddress}
              onChange={(e) => setShippingAddress(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-md"
              rows={3}
            />
          </div>
          <div className="mb-4">
            <div className="flex justify-between items-center mb-2">
              <label className="block text-sm font-medium text-gray-700">
                محصولات
              </label>
              <button
                type="button"
                onClick={addOrderItem}
                className="text-blue-600 hover:text-blue-800 text-sm"
              >
                + افزودن محصول
              </button>
            </div>
            {orderItems.map((item, index) => (
              <div key={index} className="flex gap-2 mb-2">
                <select
                  value={item.productId}
                  onChange={(e) => updateOrderItem(index, 'productId', parseInt(e.target.value))}
                  className="flex-1 px-3 py-2 border border-gray-300 rounded-md"
                  required
                >
                  <option value="0">انتخاب محصول</option>
                  {products.map((product) => (
                    <option key={product.id} value={product.id}>
                      {product.name} - {product.price.toLocaleString('fa-IR')} تومان (موجودی: {product.stockQuantity})
                    </option>
                  ))}
                </select>
                <input
                  type="number"
                  min="1"
                  value={item.quantity}
                  onChange={(e) => updateOrderItem(index, 'quantity', parseInt(e.target.value))}
                  className="w-24 px-3 py-2 border border-gray-300 rounded-md"
                  required
                />
                <button
                  type="button"
                  onClick={() => removeOrderItem(index)}
                  className="px-3 py-2 bg-red-600 text-white rounded-md hover:bg-red-700"
                >
                  حذف
                </button>
              </div>
            ))}
          </div>
          <div className="flex justify-end space-x-4">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300"
            >
              انصراف
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-orange-600 text-white rounded-lg hover:bg-orange-700"
            >
              ایجاد سفارش
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

