'use client';

import { useEffect, useState } from 'react';
import { productService } from '@/lib/services/productService';
import { categoryService } from '@/lib/services/categoryService';
import type { Product, CreateProductDto, Category } from '@/types';
import ProductModal from '@/components/ProductModal';

export default function ProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [productsData, categoriesData] = await Promise.all([
        productService.getAll(),
        categoryService.getAll(),
      ]);
      setProducts(productsData);
      setCategories(categoriesData);
    } catch (error) {
      console.error('Error loading data:', error);
      alert('خطا در بارگذاری اطلاعات');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (data: CreateProductDto) => {
    try {
      await productService.create(data);
      await loadData();
      setIsModalOpen(false);
    } catch (error) {
      console.error('Error creating product:', error);
      alert('خطا در ایجاد محصول');
    }
  };

  const handleUpdate = async (id: number, data: CreateProductDto) => {
    try {
      await productService.update(id, data);
      await loadData();
      setIsModalOpen(false);
      setEditingProduct(null);
    } catch (error) {
      console.error('Error updating product:', error);
      alert('خطا در به‌روزرسانی محصول');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('آیا از حذف این محصول اطمینان دارید؟')) return;

    try {
      await productService.delete(id);
      await loadData();
    } catch (error) {
      console.error('Error deleting product:', error);
      alert('خطا در حذف محصول');
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
        <h1 className="text-3xl font-bold text-gray-800">محصولات</h1>
        <button
          onClick={() => {
            setEditingProduct(null);
            setIsModalOpen(true);
          }}
          className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors"
        >
          افزودن محصول
        </button>
      </div>

      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                نام
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                قیمت
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                موجودی
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                دسته‌بندی
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                وضعیت
              </th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                عملیات
              </th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {products.map((product) => (
              <tr key={product.id}>
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                  {product.name}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {product.hasActiveDiscount && product.finalPrice !== undefined ? (
                    <div>
                      <div className="text-gray-400 line-through text-xs">
                        {product.price.toLocaleString('fa-IR')} تومان
                      </div>
                      <div className="text-red-600 font-semibold">
                        {product.finalPrice.toLocaleString('fa-IR')} تومان
                      </div>
                    </div>
                  ) : (
                    <div>{product.price.toLocaleString('fa-IR')} تومان</div>
                  )}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {product.stockQuantity}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {product.categoryName || '-'}
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <span
                    className={`px-2 py-1 text-xs rounded-full ${
                      product.isActive
                        ? 'bg-green-100 text-green-800'
                        : 'bg-red-100 text-red-800'
                    }`}
                  >
                    {product.isActive ? 'فعال' : 'غیرفعال'}
                  </span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <button
                    onClick={() => {
                      setEditingProduct(product);
                      setIsModalOpen(true);
                    }}
                    className="text-blue-600 hover:text-blue-900 mr-4"
                  >
                    ویرایش
                  </button>
                  <button
                    onClick={() => handleDelete(product.id)}
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
        <ProductModal
          product={editingProduct}
          categories={categories}
          onClose={() => {
            setIsModalOpen(false);
            setEditingProduct(null);
          }}
          onSave={editingProduct
            ? (data) => handleUpdate(editingProduct.id, data)
            : handleCreate}
        />
      )}
    </div>
  );
}

