import { redirect } from 'next/navigation';
import { checkAuthAction } from '@/app/actions/authActions';
import { checkAdminAction } from '@/app/actions/adminActions';
import CustomersClient from './CustomersClient';

export default async function CustomersPage() {
  // Check if user is authenticated
  const authResult = await checkAuthAction();
  
  if (!authResult.authenticated) {
    redirect('/login?redirect=/customers');
  }

  // Check if user is admin
  const adminResult = await checkAdminAction();
  
  if (!adminResult.isAdmin) {
    // Redirect non-admin users to dashboard or home
    redirect('/dashboard?error=access_denied');
  }

  // If user is admin, show the customers page
  return <CustomersClient />;
}

