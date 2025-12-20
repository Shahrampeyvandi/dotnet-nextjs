import { api } from '../api';
import type { User, UpdateUserDto, Order } from '@/types';

export const userService = {
  getCurrentUser: async (): Promise<User> => {
    return api.get<User>('/Users/me');
  },

  updateUser: async (data: UpdateUserDto): Promise<void> => {
    return api.put<void>('/Users/me', data);
  },

  getUserOrders: async (): Promise<Order[]> => {
    return api.get<Order[]>('/Users/me/orders');
  },
};

