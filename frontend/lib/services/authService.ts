import { api } from '../api';
import type { User, RegisterDto, LoginDto } from '@/types';

export const authService = {
  register: async (data: RegisterDto): Promise<User> => {
    return api.post<User>('/Auth/register', data);
  },

  login: async (data: LoginDto): Promise<User> => {
    return api.post<User>('/Auth/login', data);
  },

  logout: async (): Promise<void> => {
    return api.post<void>('/Auth/logout', {});
  },

  getCurrentUser: async (): Promise<User> => {
    return api.get<User>('/Auth/me');
  },
};

