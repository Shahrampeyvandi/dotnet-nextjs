import { api } from '../api';
import type { Product, CreateProductDto, UpdateProductDto, PaginatedResponse } from '@/types';

export const productService = {
  getAll: async (): Promise<Product[]> => {
    return api.get<Product[]>('/Products');
  },

  getById: async (id: number): Promise<Product> => {
    return api.get<Product>(`/Products/${id}`);
  },

  getPaginated: async (
    pageNumber: number = 1,
    pageSize: number = 10,
    categoryId?: number,
    includeInactive: boolean = false
  ): Promise<PaginatedResponse<Product>> => {
    const params = new URLSearchParams({
      pageNumber: pageNumber.toString(),
      pageSize: pageSize.toString(),
    });
    
    if (categoryId) {
      params.append('categoryId', categoryId.toString());
    }
    
    if (includeInactive) {
      params.append('includeInactive', 'true');
    }
    
    return api.get<PaginatedResponse<Product>>(`/Products/paginated?${params.toString()}`);
  },

  create: async (data: CreateProductDto): Promise<Product> => {
    return api.post<Product>('/Products', data);
  },

  update: async (id: number, data: UpdateProductDto): Promise<void> => {
    return api.put<void>(`/Products/${id}`, data);
  },

  delete: async (id: number): Promise<void> => {
    return api.delete<void>(`/Products/${id}`);
  },
};

