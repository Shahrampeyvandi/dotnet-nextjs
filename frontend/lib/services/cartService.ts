import { api } from '../api';
import type { Cart, AddToCartDto } from '@/types';

export const cartService = {
  getCart: async (): Promise<Cart> => {
    return api.get<Cart>('/Cart');
  },

  addToCart: async (data: AddToCartDto): Promise<void> => {
    return api.post<void>('/Cart/add', data);
  },

  removeFromCart: async (productId: number): Promise<void> => {
    return api.delete<void>(`/Cart/${productId}`);
  },

  updateCartItem: async (productId: number, quantity: number): Promise<void> => {
    return api.put<void>(`/Cart/${productId}`, quantity);
  },

  clearCart: async (): Promise<void> => {
    return api.post<void>('/Cart/clear', {});
  },
};

