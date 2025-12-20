import { api } from '../api';
import type { Category, CreateCategoryDto, UpdateCategoryDto } from '@/types';

export const categoryService = {
  getAll: async (): Promise<Category[]> => {
    return api.get<Category[]>('/Categories');
  },

  getById: async (id: number): Promise<Category> => {
    return api.get<Category>(`/Categories/${id}`);
  },

  create: async (data: CreateCategoryDto): Promise<Category> => {
    return api.post<Category>('/Categories', data);
  },

  update: async (id: number, data: UpdateCategoryDto): Promise<void> => {
    return api.put<void>(`/Categories/${id}`, data);
  },

  delete: async (id: number): Promise<void> => {
    return api.delete<void>(`/Categories/${id}`);
  },
};

