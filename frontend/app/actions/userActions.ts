'use server';

import { fetchAPI } from '@/lib/serverApi';
import type { Order } from '@/types';

export async function updateUserAction(formData: FormData) {
  try {
    const data = {
      firstName: formData.get('firstName') as string,
      lastName: formData.get('lastName') as string,
      email: formData.get('email') as string,
      phone: formData.get('phone') as string || undefined,
      address: formData.get('address') as string || undefined,
      city: formData.get('city') as string || undefined,
      postalCode: formData.get('postalCode') as string || undefined,
    };

    await fetchAPI('/Users/me', {
      method: 'PUT',
      body: JSON.stringify(data),
    });

    return { success: true };
  } catch (error: unknown) {
    const errorMessage = error instanceof Error ? error.message : 'خطا در به‌روزرسانی اطلاعات';
    return { success: false, error: errorMessage };
  }
}

// دریافت سفارشات کاربر در سمت سرور
export async function getUserOrdersAction() {
  try {
    const orders = await fetchAPI<Order[]>('/Users/me/orders');
    return { success: true, orders };
  } catch (error: unknown) {
    const errorMessage = error instanceof Error ? error.message : 'خطا در دریافت سفارشات';
    return { success: false, orders: [], error: errorMessage };
  }
}

