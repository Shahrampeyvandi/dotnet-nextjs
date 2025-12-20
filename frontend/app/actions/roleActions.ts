'use server';

import { fetchAPI } from '@/lib/serverApi';
import type { User } from '@/types';

/**
 * Get all users (Admin only)
 */
export async function getAllUsersAction(): Promise<{ success: boolean; users?: User[]; error?: string }> {
  try {
    const users = await fetchAPI<User[]>('/Admin/users');
    return { success: true, users };
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : 'خطا در دریافت لیست کاربران';
    return { success: false, error: errorMessage };
  }
}

/**
 * Get all available roles
 */
export async function getAllRolesAction(): Promise<{ success: boolean; roles?: string[]; error?: string }> {
  try {
    const roles = await fetchAPI<string[]>('/Admin/roles');
    return { success: true, roles };
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : 'خطا در دریافت لیست نقش‌ها';
    return { success: false, error: errorMessage };
  }
}

/**
 * Add a role to a user
 */
export async function addRoleToUserAction(userId: string, roleName: string): Promise<{ success: boolean; error?: string }> {
  try {
    await fetchAPI(`/Admin/users/${userId}/roles/${roleName}`, {
      method: 'POST',
    });
    return { success: true };
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : 'خطا در افزودن نقش';
    return { success: false, error: errorMessage };
  }
}

/**
 * Remove a role from a user
 */
export async function removeRoleFromUserAction(userId: string, roleName: string): Promise<{ success: boolean; error?: string }> {
  try {
    await fetchAPI(`/Admin/users/${userId}/roles/${roleName}`, {
      method: 'DELETE',
    });
    return { success: true };
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : 'خطا در حذف نقش';
    return { success: false, error: errorMessage };
  }
}

