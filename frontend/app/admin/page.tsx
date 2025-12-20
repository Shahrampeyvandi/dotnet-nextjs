import { redirect } from 'next/navigation';
import { checkAuthAction } from '@/app/actions/authActions';
import { checkAdminAction } from '@/app/actions/adminActions';
import AdminPanelClient from './AdminPanelClient';

export default async function AdminPanelPage() {
  // Check if user is authenticated
  const authResult = await checkAuthAction();
  
  if (!authResult.authenticated) {
    redirect('/login?redirect=/admin');
  }

  // Check if user is admin
  const adminResult = await checkAdminAction();
  
  if (!adminResult.isAdmin) {
    // Redirect non-admin users to dashboard
    redirect('/dashboard?error=access_denied');
  }

  // If user is admin, show the admin panel
  return <AdminPanelClient />;
}

