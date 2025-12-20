'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useActionState } from 'react';
import { updateUserAction } from '@/app/actions/userActions';
import type { User } from '@/types';
import { useFormStatus } from 'react-dom';
import { useTranslations } from 'next-intl';

interface EditProfileClientProps {
  initialUser: User;
}

function SubmitButton() {
  const { pending } = useFormStatus();
  const t = useTranslations('common');
  return (
    <button
      type="submit"
      disabled={pending}
      className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-300"
    >
      {pending ? t('saving') : t('save')}
    </button>
  );
}

export default function EditProfileClient({ initialUser }: EditProfileClientProps) {
  const router = useRouter();
  const t = useTranslations('editProfile');
  const tCommon = useTranslations('common');
  const [state, formAction] = useActionState<{ success: boolean; error?: string } | null, FormData>(
    async (prevState, formData) => {
      return await updateUserAction(formData);
    },
    null
  );

  // Handle successful update - redirect to dashboard
  useEffect(() => {
    if (state?.success) {
      router.push('/dashboard');
    }
  }, [state?.success, router]);

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">{t('title')}</h1>

      <div className="max-w-2xl mx-auto bg-white rounded-lg shadow-md p-8">
        {state?.success && (
          <div className="mb-4 p-3 bg-green-100 text-green-700 rounded-lg">
            {t('updateSuccess')}
          </div>
        )}
        {state?.error && (
          <div className="mb-4 p-3 bg-red-100 text-red-700 rounded-lg">
            {state.error}
          </div>
        )}
        <form action={formAction}>
          <div className="grid grid-cols-2 gap-4 mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                {t('firstName')}
              </label>
              <input
                type="text"
                name="firstName"
                defaultValue={initialUser.firstName}
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                {t('lastName')}
              </label>
              <input
                type="text"
                name="lastName"
                defaultValue={initialUser.lastName}
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
          </div>

          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              {t('email')}
            </label>
            <input
              type="email"
              name="email"
              defaultValue={initialUser.email}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md"
            />
          </div>

          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              {t('phone')}
            </label>
            <input
              type="tel"
              name="phone"
              defaultValue={initialUser.phone || ''}
              className="w-full px-3 py-2 border border-gray-300 rounded-md"
            />
          </div>

          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              {t('address')}
            </label>
            <textarea
              name="address"
              defaultValue={initialUser.address || ''}
              className="w-full px-3 py-2 border border-gray-300 rounded-md"
              rows={3}
            />
          </div>

          <div className="grid grid-cols-2 gap-4 mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                {t('city')}
              </label>
              <input
                type="text"
                name="city"
                defaultValue={initialUser.city || ''}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                {t('postalCode')}
              </label>
              <input
                type="text"
                name="postalCode"
                defaultValue={initialUser.postalCode || ''}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
          </div>

          <div className="flex justify-end gap-4">
            <button
              type="button"
              onClick={() => router.push('/dashboard')}
              className="px-4 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300"
            >
              {tCommon('cancel')}
            </button>
            <SubmitButton />
          </div>
        </form>
      </div>
    </div>
  );
}

