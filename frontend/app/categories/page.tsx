'use client';

import { useEffect, useState } from 'react';
import { categoryService } from '@/lib/services/categoryService';
import type { Category, CreateCategoryDto } from '@/types';
import CategoryModal from '@/components/CategoryModal';

export default function CategoriesPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<Category | null>(null);

  useEffect(() => {
    loadCategories();
  }, []);

  const loadCategories = async () => {
    try {
      setLoading(true);
      const data = await categoryService.getAll();
      setCategories(data);
    } catch (error) {
      console.error('Error loading categories:', error);
      alert('خطا در بارگذاری دسته‌بندی‌ها');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: CreateCategoryDto) => {
    try {
      await categoryService.create(data);
      await loadCategories();
      setIsModalOpen(false);
    } catch (error) {
      console.error('Error creating category:', error);
      alert('خطا در ایجاد دسته‌بندی');
    }
  };

  const handleUpdate = async (id: number, data: CreateCategoryDto) => {
    try {
      await categoryService.update(id, data);
      await loadCategories();
      setIsModalOpen(false);
      setEditingCategory(null);
    } catch (error) {
      console.error('Error updating category:', error);
      alert('خطا در به‌روزرسانی دسته‌بندی');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('آیا از حذف این دسته‌بندی اطمینان دارید؟')) return;

    try {
      await categoryService.delete(id);
      await loadCategories();
    } catch (error) {
      console.error('Error deleting category:', error);
      alert('خطا در حذف دسته‌بندی');
    }
  };

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">در حال بارگذاری...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800">دسته‌بندی‌ها</h1>
        <button
          onClick={() => {
            setEditingCategory(null);
            setIsModalOpen(true);
          }}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          افزودن دسته‌بندی
        </button>
      </div>

      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                نام
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                توضیحات
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                تاریخ ایجاد
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
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
                  <button
                    onClick={() => {
                      setEditingCategory(category);
                      setIsModalOpen(true);
                    }}
                    className="text-blue-600 hover:text-blue-900 mr-4"
                  >
                    ویرایش
                  </button>
                  <button
                    onClick={() => handleDelete(category.id)}
                    className="text-red-600 hover:text-red-900"
                  >
                    حذف
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {isModalOpen && (
        <CategoryModal
          category={editingCategory}
          onClose={() => {
            setIsModalOpen(false);
            setEditingCategory(null);
          }}
          onSave={editingCategory
            ? (data) => handleUpdate(editingCategory.id, data)
            : handleCreate}
        />
      )}
    </div>
  );
}

