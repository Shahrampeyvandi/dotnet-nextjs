'use client';

import { useEffect, useState } from 'react';
import { getAllUsersAction, getAllRolesAction, addRoleToUserAction, removeRoleFromUserAction } from '@/app/actions/roleActions';
import type { User } from '@/types';
import { useTranslations } from 'next-intl';
import CustomersClient from '@/app/customers/CustomersClient';
import AdminOrdersClient from './AdminOrdersClient';
import AdminProductsClient from './AdminProductsClient';
import AdminCategoriesClient from './AdminCategoriesClient';

type TabType = 'users' | 'customers' | 'orders' | 'products' | 'categories';

export default function AdminPanelClient() {
  const t = useTranslations('admin');
  const tCommon = useTranslations('common');
  const [activeTab, setActiveTab] = useState<TabType>('users');
  const [users, setUsers] = useState<User[]>([]);
  const [roles, setRoles] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    loadData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);
      
      const [usersResult, rolesResult] = await Promise.all([
        getAllUsersAction(),
        getAllRolesAction()
      ]);

      if (usersResult.success && usersResult.users) {
        setUsers(usersResult.users);
      } else {
        setError(usersResult.error || t('errorLoadingUsers'));
      }

      if (rolesResult.success && rolesResult.roles) {
        setRoles(rolesResult.roles);
      } else {
        setError(rolesResult.error || t('errorLoadingRoles'));
      }
    } catch (err) {
      setError(t('errorLoadingData'));
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleAddRole = async (userId: string, roleName: string) => {
    try {
      setError(null);
      setSuccess(null);
      
      const result = await addRoleToUserAction(userId, roleName);
      
      if (result.success) {
        setSuccess(t('roleAddedSuccess').replace('{role}', roleName));
        await loadData(); // Reload users to get updated roles
      } else {
        setError(result.error || t('errorAddingRole'));
      }
    } catch (err) {
      setError(t('errorAddingRole'));
      console.error('Error adding role:', err);
    }
  };

  const handleRemoveRole = async (userId: string, roleName: string) => {
    try {
      setError(null);
      setSuccess(null);
      
      if (!confirm(t('confirmRemoveRole').replace('{role}', roleName))) {
        return;
      }

      const result = await removeRoleFromUserAction(userId, roleName);
      
      if (result.success) {
        setSuccess(t('roleRemovedSuccess').replace('{role}', roleName));
        await loadData(); // Reload users to get updated roles
      } else {
        setError(result.error || t('errorRemovingRole'));
      }
    } catch (err) {
      setError(t('errorRemovingRole'));
      console.error('Error removing role:', err);
    }
  };

  if (loading && activeTab === 'users') {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">{tCommon('loading')}</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">{t('panelTitle')}</h1>

      {/* Tabs */}
      <div className="mb-6 border-b border-gray-200">
        <nav className="flex space-x-8" aria-label="Tabs">
          <button
            onClick={() => setActiveTab('users')}
            className={`py-4 px-1 border-b-2 font-medium text-sm ${
              activeTab === 'users'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            {t('userManagement')}
          </button>
          <button
            onClick={() => setActiveTab('customers')}
            className={`py-4 px-1 border-b-2 font-medium text-sm ${
              activeTab === 'customers'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            {t('customers')}
          </button>
          <button
            onClick={() => setActiveTab('orders')}
            className={`py-4 px-1 border-b-2 font-medium text-sm ${
              activeTab === 'orders'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            {t('orders')}
          </button>
          <button
            onClick={() => setActiveTab('products')}
            className={`py-4 px-1 border-b-2 font-medium text-sm ${
              activeTab === 'products'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            محصولات
          </button>
          <button
            onClick={() => setActiveTab('categories')}
            className={`py-4 px-1 border-b-2 font-medium text-sm ${
              activeTab === 'categories'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            دسته‌بندی‌ها
          </button>
        </nav>
      </div>

      {/* Tab Content */}
      {activeTab === 'users' && (
        <>
          {error && (
            <div className="mb-4 p-3 bg-red-100 text-red-700 rounded-lg">
              {error}
            </div>
          )}

          {success && (
            <div className="mb-4 p-3 bg-green-100 text-green-700 rounded-lg">
              {success}
            </div>
          )}

          <div className="bg-white rounded-lg shadow-md overflow-hidden">
            <div className="p-4 bg-gray-50 border-b">
              <h2 className="text-xl font-semibold text-gray-800">{t('usersList')}</h2>
            </div>
            
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      {t('username')}
                    </th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      {t('name')}
                    </th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      {t('email')}
                    </th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      {t('currentRoles')}
                    </th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      {t('addRole')}
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {users.map((user) => (
                    <tr key={user.id}>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                        {user.username}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {user.firstName} {user.lastName}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {user.email}
                      </td>
                      <td className="px-6 py-4 text-sm">
                        <div className="flex flex-wrap gap-2">
                          {user.roles && user.roles.length > 0 ? (
                            user.roles.map((role) => (
                              <span
                                key={role}
                                className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800"
                              >
                                {role}
                                <button
                                  onClick={() => handleRemoveRole(user.id, role)}
                                  className="ml-2 text-blue-600 hover:text-blue-900"
                                  title={t('removeRole')}
                                >
                                  ×
                                </button>
                              </span>
                            ))
                          ) : (
                            <span className="text-gray-400">{t('noRoles')}</span>
                          )}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm">
                        <select
                          onChange={(e) => {
                            if (e.target.value) {
                              handleAddRole(user.id, e.target.value);
                              e.target.value = ''; // Reset selection
                            }
                          }}
                          className="px-3 py-1 border border-gray-300 rounded-md text-sm"
                          defaultValue=""
                        >
                          <option value="">{t('selectRole')}</option>
                          {roles
                            .filter(role => !user.roles?.includes(role))
                            .map((role) => (
                              <option key={role} value={role}>
                                {role}
                              </option>
                            ))}
                        </select>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </>
      )}

      {activeTab === 'customers' && (
        <div className="bg-white rounded-lg shadow-md">
          <CustomersClient />
        </div>
      )}

      {activeTab === 'orders' && (
        <div className="bg-white rounded-lg shadow-md">
          <AdminOrdersClient />
        </div>
      )}

      {activeTab === 'products' && (
        <div className="bg-white rounded-lg shadow-md">
          <AdminProductsClient />
        </div>
      )}

      {activeTab === 'categories' && (
        <div className="bg-white rounded-lg shadow-md">
          <AdminCategoriesClient />
        </div>
      )}
    </div>
  );
}

