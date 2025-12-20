'use server';

import { fetchAPI } from '@/lib/serverApi';
import type { User } from '@/types';

/**
 * Check if the current user is an admin
 */
export async function checkAdminAction(): Promise<{ isAdmin: boolean; user?: User }> {
  try {
    const user = await fetchAPI<User>('/Auth/me');
    
    // Check if user has Admin role
    // Backend returns roles as an array in user.roles (or Roles with capital R)
    const isAdmin = user?.roles?.includes('Admin') || 
                    (user as { Roles?: string[] })?.Roles?.includes('Admin') || 
                    false;
    
    return { isAdmin, user };
  } catch {
    return { isAdmin: false };
  }
}

