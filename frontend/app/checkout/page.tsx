'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useActionState } from 'react';
import { cartService } from '@/lib/services/cartService';
import { authService } from '@/lib/services/authService';
import { checkoutAction } from '@/app/actions/cartActions';
import type { CartItem, User } from '@/types';
import { useFormStatus } from 'react-dom';

function SubmitButton() {
  const { pending } = useFormStatus();
  return (
    <button
      type="submit"
      disabled={pending}
      className="w-full mt-6 bg-green-600 text-white py-3 rounded-lg hover:bg-green-700 disabled:bg-gray-300 disabled:cursor-not-allowed font-semibold"
    >
      {pending ? 'در حال پردازش...' : 'پرداخت'}
    </button>
  );
}

export default function CheckoutPage() {
  const router = useRouter();
  const [cart, setCart] = useState<{ items: CartItem[]; totalAmount: number } | null>(null);
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [state, formAction] = useActionState<{ success: boolean; order?: any; error?: string; cartUpdated?: boolean; errors?: string[] } | null, FormData>(
    async (prevState, formData) => {
      return await checkoutAction(formData);
    },
    null
  );

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    if (state?.success && state.order) {
      alert(`پرداخت با موفقیت انجام شد! شماره سفارش: ${state.order.orderNumber}`);
      router.push(`/orders/${state.order.id}`);
    } else if (state?.cartUpdated) {
      // اگر cart به‌روزرسانی شد، آن را reload کنیم
      loadData();
    }
  }, [state, router]);

  const loadData = async () => {
    try {
      setLoading(true);
      const [cartData, userData] = await Promise.all([
        cartService.getCart(),
        authService.getCurrentUser().catch(() => null),
      ]);
      setCart(cartData);
      setUser(userData);
    } catch (error) {
      console.error('Error loading data:', error);
      router.push('/login?redirect=/checkout');
    } finally {
      setLoading(false);
    }
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
        <div className="bg-white rounded-lg shadow-md p-8 text-center">
          <p className="text-gray-600 mb-4">سبد خرید شما خالی است</p>
          <button
            onClick={() => router.push('/products-list')}
            className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700"
          >
            مشاهده محصولات
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">تسویه حساب</h1>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div>
          <div className="bg-white rounded-lg shadow-md p-6 mb-6">
            <h2 className="text-xl font-bold text-gray-800 mb-4">اطلاعات ارسال</h2>
            {user && (
              <div className="mb-4">
                <p className="text-gray-600">
                  <strong>نام:</strong> {user.firstName} {user.lastName}
                </p>
                <p className="text-gray-600">
                  <strong>ایمیل:</strong> {user.email}
                </p>
                <p className="text-gray-600">
                  <strong>تلفن:</strong> {user.phone || '-'}
                </p>
              </div>
            )}
            <form action={formAction}>
              {state?.error && (
                <div className="mb-4 p-3 bg-red-100 text-red-700 rounded-lg">
                  <p className="font-semibold mb-2">{state.error}</p>
                  {state.errors && state.errors.length > 0 && (
                    <ul className="list-disc list-inside text-sm space-y-1">
                      {state.errors.map((err, index) => (
                        <li key={index}>{err}</li>
                      ))}
                    </ul>
                  )}
                  {state.cartUpdated && (
                    <p className="mt-2 text-sm font-semibold">
                      لطفا سبد خرید را بررسی کنید و دوباره تلاش کنید.
                    </p>
                  )}
                </div>
              )}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  آدرس ارسال
                </label>
                <textarea
                  name="shippingAddress"
                  defaultValue={user?.address || ''}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md"
                  rows={4}
                  required
                />
              </div>
              <div className="mt-6">
                <h2 className="text-xl font-bold text-gray-800 mb-4">روش پرداخت</h2>
                <div className="space-y-2">
                  <label className="flex items-center p-3 border rounded-lg cursor-pointer hover:bg-gray-50">
                    <input type="radio" name="payment" value="card" defaultChecked className="ml-2" />
                    <span>پرداخت آنلاین (فیک)</span>
                  </label>
                </div>
              </div>
              <SubmitButton />
            </form>
          </div>
        </div>

        <div>
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-xl font-bold text-gray-800 mb-4">خلاصه سفارش</h2>
            <div className="space-y-2 mb-4">
              {cart.items.map((item) => (
                <div key={item.productId} className="flex justify-between text-sm">
                  <span>{item.productName} × {item.quantity}</span>
                  <span>{item.totalPrice.toLocaleString('fa-IR')} تومان</span>
                </div>
              ))}
            </div>
            <div className="border-t pt-4">
              <div className="flex justify-between text-lg font-bold">
                <span>مبلغ کل:</span>
                <span className="text-green-600">
                  {cart.totalAmount.toLocaleString('fa-IR')} تومان
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
