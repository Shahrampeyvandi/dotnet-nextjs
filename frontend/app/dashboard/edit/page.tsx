import { redirect } from 'next/navigation';
import { checkAuthAction } from '@/app/actions/authActions';
import EditProfileClient from './EditProfileClient';

export default async function EditProfilePage() {
  // بررسی authentication در سمت سرور
  const authResult = await checkAuthAction();
  
  // اگر کاربر لاگین نباشد، به صفحه login هدایت شود
  if (!authResult.authenticated || !authResult.user) {
    redirect('/login?redirect=/dashboard/edit');
  }

  // اگر کاربر لاگین باشد، کامپوننت client را نمایش بده
  return <EditProfileClient initialUser={authResult.user} />;
}
