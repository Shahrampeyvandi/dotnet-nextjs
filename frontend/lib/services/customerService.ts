import { api } from '../api';
import type { Customer, CreateCustomerDto, UpdateCustomerDto } from '@/types';

export const customerService = {
  getAll: async (): Promise<Customer[]> => {
    return api.get<Customer[]>('/Customers');
  },

  getById: async (id: number): Promise<Customer> => {
    return api.get<Customer>(`/Customers/${id}`);
  },

  create: async (data: CreateCustomerDto): Promise<Customer> => {
    return api.post<Customer>('/Customers', data);
  },

  update: async (id: number, data: UpdateCustomerDto): Promise<void> => {
    return api.put<void>(`/Customers/${id}`, data);
  },

  delete: async (id: number): Promise<void> => {
    return api.delete<void>(`/Customers/${id}`);
  },
};

