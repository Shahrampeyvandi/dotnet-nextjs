'use server';

import { fetchAPI } from '@/lib/serverApi';

export async function registerAction(formData: FormData) {
  try {
    const data = {
      username: formData.get('username') as string,
      email: formData.get('email') as string,
      password: formData.get('password') as string,
      firstName: formData.get('firstName') as string,
      lastName: formData.get('lastName') as string,
    };

    const user = await fetchAPI<any>('/Auth/register', {
      method: 'POST',
      body: JSON.stringify(data),
    });

    return { success: true, user };
  } catch (error: any) {
    return { success: false, error: error.message || 'خطا در ثبت‌نام' };
  }
}

export async function loginAction(formData: FormData) {
  try {
    const data = {
      username: formData.get('username') as string,
      password: formData.get('password') as string,
    };

    const user = await fetchAPI<any>('/Auth/login', {
      method: 'POST',
      body: JSON.stringify(data),
    });

    return { success: true, user };
  } catch (error: any) {
    return { success: false, error: error.message || 'نام کاربری یا رمز عبور اشتباه است' };
  }
}

export async function logoutAction() {
  try {
    await fetchAPI('/Auth/logout', {
      method: 'POST',
    });
    return { success: true };
  } catch (error: any) {
    return { success: false, error: error.message };
  }
}

// بررسی authentication در سمت سرور
export async function checkAuthAction(): Promise<{ authenticated: boolean; user?: any }> {
  try {
    const user = await fetchAPI<any>('/Auth/me');
    return { authenticated: true, user };
  } catch (error: any) {
    return { authenticated: false };
  }
}

