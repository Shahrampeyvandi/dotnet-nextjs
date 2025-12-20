'use client';

import { useActionState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { registerAction } from '@/app/actions/authActions';
import Link from 'next/link';
import { useFormStatus } from 'react-dom';
import type { User } from '@/types';

function SubmitButton() {
  const { pending } = useFormStatus();
  return (
    <button
      type="submit"
      disabled={pending}
      className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed"
    >
      {pending ? 'در حال ثبت‌نام...' : 'ثبت‌نام'}
    </button>
  );
}

export default function RegisterPage() {
  const router = useRouter();
  const [state, formAction] = useActionState<{ success: boolean; user?: User; error?: string } | null, FormData>(
    async (prevState, formData) => {
      return await registerAction(formData);
    },
    null
  );

  useEffect(() => {
    if (state?.success) {
      router.push('/dashboard');
    }
  }, [state, router]);

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="max-w-md mx-auto bg-white rounded-lg shadow-md p-8">
        <h1 className="text-3xl font-bold text-gray-800 mb-6 text-center">ثبت‌نام</h1>
        <form action={formAction}>
          {state?.error && (
            <div className="mb-4 p-3 bg-red-100 text-red-700 rounded-lg">
              {state.error}
            </div>
          )}
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              نام کاربری
            </label>
            <input
              type="text"
              name="username"
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              ایمیل
            </label>
            <input
              type="email"
              name="email"
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div className="grid grid-cols-2 gap-4 mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                نام
              </label>
              <input
                type="text"
                name="firstName"
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                نام خانوادگی
              </label>
              <input
                type="text"
                name="lastName"
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              رمز عبور
            </label>
            <input
              type="password"
              name="password"
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <SubmitButton />
        </form>
        <p className="mt-4 text-center text-gray-600">
          قبلاً ثبت‌نام کرده‌اید؟{' '}
          <Link href="/login" className="text-blue-600 hover:text-blue-800">
            وارد شوید
          </Link>
        </p>
      </div>
    </div>
  );
}
