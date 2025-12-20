import { api } from '../api';
import type { Order, CreateOrderDto, UpdateOrderDto, PaginatedResponse } from '@/types';

export const orderService = {
  getAll: async (): Promise<Order[]> => {
    return api.get<Order[]>('/Orders');
  },

  getPaginated: async (page: number, pageSize: number): Promise<PaginatedResponse<Order>> => {
    return api.get<PaginatedResponse<Order>>(`/Orders?page=${page}&pageSize=${pageSize}`);
  },

  getById: async (id: number): Promise<Order> => {
    return api.get<Order>(`/Orders/${id}`);
  },

  create: async (data: CreateOrderDto): Promise<Order> => {
    return api.post<Order>('/Orders', data);
  },

  update: async (id: number, data: UpdateOrderDto): Promise<void> => {
    return api.put<void>(`/Orders/${id}`, data);
  },

  delete: async (id: number): Promise<void> => {
    return api.delete<void>(`/Orders/${id}`);
  },

  getInvoices: async (): Promise<Order[]> => {
    return api.get<Order[]>('/Orders/invoices');
  },
};

