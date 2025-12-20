'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { cartService } from '@/lib/services/cartService';
import { authService } from '@/lib/services/authService';
import Link from 'next/link';
import type { CartItem } from '@/types';

export default function CartPage() {
  const router = useRouter();
  const [cart, setCart] = useState<{ items: CartItem[]; totalAmount: number; totalItems: number } | null>(null);
  const [loading, setLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    loadCart();
    checkAuth();
  }, []);

  const loadCart = async () => {
    try {
      setLoading(false);
      const data = await cartService.getCart();
      setCart(data);
    } catch (error) {
      console.error('Error loading cart:', error);
    } finally {
      setLoading(false);
    }
  };

  const checkAuth = async () => {
    try {
      await authService.getCurrentUser();
      setIsAuthenticated(true);
    } catch (error) {
      setIsAuthenticated(false);
    }
  };

  const handleRemoveItem = async (productId: number) => {
    try {
      await cartService.removeFromCart(productId);
      await loadCart();
    } catch (error) {
      console.error('Error removing item:', error);
      alert('خطا در حذف محصول');
    }
  };

  const handleUpdateQuantity = async (productId: number, quantity: number) => {
    if (quantity <= 0) {
      await handleRemoveItem(productId);
      return;
    }

    try {
      await cartService.updateCartItem(productId, quantity);
      await loadCart();
    } catch (error) {
      console.error('Error updating quantity:', error);
      alert('خطا در به‌روزرسانی تعداد');
    }
  };

  const handleCheckout = () => {
    if (!isAuthenticated) {
      router.push('/login?redirect=/cart');
      return;
    }
    router.push('/checkout');
  };

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">در حال بارگذاری...</div>
      </div>
    );
  }

  if (!cart || cart.items.length === 0) {
    return (
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold text-gray-800 mb-6">سبد خرید</h1>
        <div className="bg-white rounded-lg shadow-md p-8 text-center">
          <p className="text-gray-600 mb-4">سبد خرید شما خالی است</p>
          <Link
            href="/products-list"
            className="inline-block bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700"
          >
            مشاهده محصولات
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">سبد خرید</h1>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2">
          <div className="bg-white rounded-lg shadow-md overflow-hidden">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    محصول
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    قیمت
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    تعداد
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    مجموع
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                    عملیات
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {cart.items.map((item) => (
                  <tr key={item.productId}>
                    <td className="px-6 py-4">
                      <Link href={`/products/${item.productId}`} className="hover:text-blue-600">
                        {item.productName}
                      </Link>
                    </td>
                    <td className="px-6 py-4">
                      {item.price.toLocaleString('fa-IR')} تومان
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-2">
                        <button
                          onClick={() => handleUpdateQuantity(item.productId, item.quantity - 1)}
                          className="w-8 h-8 bg-gray-200 rounded hover:bg-gray-300"
                        >
                          -
                        </button>
                        <span>{item.quantity}</span>
                        <button
                          onClick={() => handleUpdateQuantity(item.productId, item.quantity + 1)}
                          className="w-8 h-8 bg-gray-200 rounded hover:bg-gray-300"
                        >
                          +
                        </button>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      {item.totalPrice.toLocaleString('fa-IR')} تومان
                    </td>
                    <td className="px-6 py-4">
                      <button
                        onClick={() => handleRemoveItem(item.productId)}
                        className="text-red-600 hover:text-red-800"
                      >
                        حذف
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        <div className="lg:col-span-1">
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-xl font-bold text-gray-800 mb-4">خلاصه سفارش</h2>
            <div className="space-y-2 mb-4">
              <div className="flex justify-between">
                <span>تعداد کل:</span>
                <span>{cart.totalItems} عدد</span>
              </div>
              <div className="flex justify-between text-lg font-bold">
                <span>مبلغ کل:</span>
                <span className="text-green-600">
                  {cart.totalAmount.toLocaleString('fa-IR')} تومان
                </span>
              </div>
            </div>
            <button
              onClick={handleCheckout}
              className="w-full bg-green-600 text-white py-3 rounded-lg hover:bg-green-700 font-semibold"
            >
              ادامه خرید
            </button>
            {!isAuthenticated && (
              <p className="text-sm text-gray-600 mt-2 text-center">
                برای ادامه باید وارد شوید
              </p>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

