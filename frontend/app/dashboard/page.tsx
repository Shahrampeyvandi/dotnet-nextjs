import { redirect } from 'next/navigation';
import { checkAuthAction } from '@/app/actions/authActions';
import { getUserOrdersAction } from '@/app/actions/userActions';
import DashboardClient from './DashboardClient';

export default async function DashboardPage() {
  // بررسی authentication در سمت سرور
  const authResult = await checkAuthAction();
  
  // اگر کاربر لاگین نباشد، به صفحه login هدایت شود
  if (!authResult.authenticated) {
    redirect('/login?redirect=/dashboard');
  }

  // دریافت سفارشات کاربر در سمت سرور
  const ordersResult = await getUserOrdersAction();
  const orders = ordersResult.success ? ordersResult.orders : [];

  // اگر کاربر لاگین باشد، کامپوننت client را نمایش بده
  return <DashboardClient initialUser={authResult.user} initialOrders={orders} />;
}
