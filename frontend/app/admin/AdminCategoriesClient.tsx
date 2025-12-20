'use client';

import { useEffect, useState } from 'react';
import { categoryService } from '@/lib/services/categoryService';
import type { Category, CreateCategoryDto } from '@/types';
import CategoryModal from '@/components/CategoryModal';

export default function AdminCategoriesClient() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<Category | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      setError(null);
      const categoriesData = await categoryService.getAll();
      setCategories(categoriesData);
    } catch (err) {
      setError('خطا در بارگذاری اطلاعات');
      console.error('Error loading data:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: CreateCategoryDto) => {
    try {
      setError(null);
      setSuccess(null);
      await categoryService.create(data);
      setSuccess('دسته‌بندی با موفقیت ایجاد شد');
      await loadData();
      setIsModalOpen(false);
    } catch (err: any) {
      setError(err.message || 'خطا در ایجاد دسته‌بندی');
      console.error('Error creating category:', err);
    }
  };

  const handleUpdate = async (id: number, data: CreateCategoryDto) => {
    try {
      setError(null);
      setSuccess(null);
      await categoryService.update(id, data);
      setSuccess('دسته‌بندی با موفقیت به‌روزرسانی شد');
      await loadData();
      setIsModalOpen(false);
      setEditingCategory(null);
    } catch (err: any) {
      setError(err.message || 'خطا در به‌روزرسانی دسته‌بندی');
      console.error('Error updating category:', err);
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('آیا از حذف این دسته‌بندی اطمینان دارید؟')) {
      return;
    }

    try {
      setError(null);
      setSuccess(null);
      await categoryService.delete(id);
      setSuccess('دسته‌بندی با موفقیت حذف شد');
      await loadData();
    } catch (err: any) {
      setError(err.message || 'خطا در حذف دسته‌بندی');
      console.error('Error deleting category:', err);
    }
  };

  const handleEdit = (category: Category) => {
    setEditingCategory(category);
    setIsModalOpen(true);
  };

  const handleAdd = () => {
    setEditingCategory(null);
    setIsModalOpen(true);
  };

  if (loading) {
    return (
      <div className="p-6 text-center">
        <div className="text-gray-600">در حال بارگذاری...</div>
      </div>
    );
  }

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-bold text-gray-800">مدیریت دسته‌بندی‌ها</h2>
        <button
          onClick={handleAdd}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
        >
          افزودن دسته‌بندی
        </button>
      </div>

      {error && (
        <div className="mb-4 p-3 bg-red-100 text-red-700 rounded-lg">{error}</div>
      )}

      {success && (
        <div className="mb-4 p-3 bg-green-100 text-green-700 rounded-lg">{success}</div>
      )}

      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  نام
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  توضیحات
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  تاریخ ایجاد
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  عملیات
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {categories.map((category) => (
                <tr key={category.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {category.name}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    {category.description || '-'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {new Date(category.createdAt).toLocaleDateString('fa-IR')}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <div className="flex space-x-2 space-x-reverse">
                      <button
                        onClick={() => handleEdit(category)}
                        className="text-blue-600 hover:text-blue-900"
                      >
                        ویرایش
                      </button>
                      <button
                        onClick={() => handleDelete(category.id)}
                        className="text-red-600 hover:text-red-900"
                      >
                        حذف
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {isModalOpen && (
        <CategoryModal
          category={editingCategory}
          onClose={() => {
            setIsModalOpen(false);
            setEditingCategory(null);
          }}
          onSave={(data) => {
            if (editingCategory) {
              handleUpdate(editingCategory.id, data);
            } else {
              handleCreate(data);
            }
          }}
        />
      )}
    </div>
  );
}

