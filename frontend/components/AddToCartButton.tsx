'use client';

import { useState, useEffect } from 'react';
import { cartService } from '@/lib/services/cartService';
import type { Cart } from '@/types';

export default function AddToCartButton({ 
  productId, 
  quantity = 1,
  onSuccess,
  cart: initialCart,
  compact = false
}: { 
  productId: number; 
  quantity?: number;
  onSuccess?: (cart: Cart) => void;
  cart?: Cart | null;
  compact?: boolean;
}) {
  const [pending, setPending] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [cart, setCart] = useState<Cart | null>(initialCart ?? null);
  const [loadingCart, setLoadingCart] = useState(initialCart === undefined);

  useEffect(() => {
    if (initialCart === undefined) {
      loadCart();
    } else {
      setCart(initialCart);
      setLoadingCart(false);
    }
  }, [initialCart]);

  const loadCart = async () => {
    try {
      const cartData = await cartService.getCart();
      setCart(cartData);
    } catch (err) {
      console.error('Error loading cart:', err);
      setCart(null);
    } finally {
      setLoadingCart(false);
    }
  };

  const isInCart = cart?.items.some(item => item.productId === productId) ?? false;
  const cartItem = cart?.items.find(item => item.productId === productId);

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    setPending(true);
    setError(null);

    try {
      await cartService.addToCart({ productId, quantity });
      // Reload cart after adding
      const updatedCart = await cartService.getCart();
      setCart(updatedCart);
      // Pass updated cart to parent to avoid reloading again
      if (onSuccess) {
        onSuccess(updatedCart);
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطا در افزودن به سبد خرید';
      setError(errorMessage);
    } finally {
      setPending(false);
    }
  };

  const handleRemove = async (e: React.FormEvent) => {
    e.preventDefault();
    setPending(true);
    setError(null);

    try {
      await cartService.removeFromCart(productId);
      // Reload cart after removing
      const updatedCart = await cartService.getCart();
      setCart(updatedCart);
      // Pass updated cart to parent to avoid reloading again
      if (onSuccess) {
        onSuccess(updatedCart);
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطا در حذف از سبد خرید';
      setError(errorMessage);
    } finally {
      setPending(false);
    }
  };

  const buttonClass = compact 
    ? "w-full bg-blue-600 text-white py-1.5 rounded text-xs font-semibold hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed"
    : "w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed";
  
  const removeButtonClass = compact
    ? "flex-1 bg-red-600 text-white py-1.5 rounded text-xs font-semibold hover:bg-red-700 disabled:bg-gray-300 disabled:cursor-not-allowed"
    : "flex-1 bg-red-600 text-white py-2 rounded-lg hover:bg-red-700 disabled:bg-gray-300 disabled:cursor-not-allowed";

  if (loadingCart) {
    return (
      <button
        disabled
        className={compact 
          ? "w-full bg-gray-300 text-gray-500 py-1.5 rounded text-xs cursor-not-allowed"
          : "w-full bg-gray-300 text-gray-500 py-2 rounded-lg cursor-not-allowed"
        }
      >
        در حال بررسی...
      </button>
    );
  }

  return (
    <div>
      {error && (
        <div className="mb-2 p-2 bg-red-100 text-red-700 text-sm rounded">
          {error}
        </div>
      )}
      {isInCart ? (
        <form onSubmit={handleRemove}>
          <div className="flex items-center gap-2">
            <button
              type="submit"
              disabled={pending}
              className={removeButtonClass}
            >
              {pending ? 'در حال حذف...' : compact ? 'حذف' : 'حذف از سبد خرید'}
            </button>
            {cartItem && (
              <span className={compact 
                ? "text-[10px] text-gray-600 bg-gray-100 px-2 py-1 rounded"
                : "text-sm text-gray-600 bg-gray-100 px-3 py-2 rounded-lg"
              }>
                {cartItem.quantity} عدد
              </span>
            )}
          </div>
        </form>
      ) : (
        <form onSubmit={handleAdd}>
          <input type="hidden" name="quantity" value={quantity} />
          <button
            type="submit"
            disabled={pending}
            className={buttonClass}
          >
            {pending ? 'در حال افزودن...' : compact ? 'افزودن به سبد' : 'افزودن به سبد خرید'}
          </button>
        </form>
      )}
    </div>
  );
}

