'use server';

import { fetchAPI } from '@/lib/serverApi';
import type { Cart } from '@/types';

export async function getCartAction() {
  try {
    const cart = await fetchAPI<Cart>('/Cart');
    return { success: true, cart };
  } catch (error: unknown) {
    const errorMessage = error instanceof Error ? error.message : 'خطا در دریافت سبد خرید';
    return { success: false, cart: null, error: errorMessage };
  }
}

export async function addToCartAction(productId: number, quantity: number = 1) {
  try {
    await fetchAPI('/Cart/add', {
      method: 'POST',
      body: JSON.stringify({ productId, quantity }),
    });

    return { success: true };
  } catch (error: unknown) {
    const errorMessage = error instanceof Error ? error.message : 'خطا در افزودن به سبد خرید';
    return { success: false, error: errorMessage };
  }
}

export async function checkoutAction(formData: FormData) {
  try {
    const shippingAddress = formData.get('shippingAddress') as string;

    const order = await fetchAPI<any>('/Checkout', {
      method: 'POST',
      body: JSON.stringify({ shippingAddress }),
    });

    return { success: true, order };
  } catch (error: any) {
    // Parse error response if it contains structured error information
    let errorMessage = 'خطا در پردازش سفارش';
    let cartUpdated = false;
    let errors: string[] = [];

    try {
      // Try to parse error message as JSON
      const errorText = error.message || '';
      if (errorText) {
        try {
          // Try to parse as JSON object
          const errorObj = JSON.parse(errorText);
          if (errorObj.message) {
            errorMessage = errorObj.message;
          }
          if (errorObj.errors && Array.isArray(errorObj.errors)) {
            errors = errorObj.errors;
          }
          if (errorObj.cartUpdated !== undefined) {
            cartUpdated = errorObj.cartUpdated;
          }
        } catch {
          // If not JSON, use as plain text
          errorMessage = errorText;
        }
      }
    } catch {
      errorMessage = error.message || 'خطا در پردازش سفارش';
    }

    return { 
      success: false, 
      error: errorMessage,
      cartUpdated,
      errors 
    };
  }
}

