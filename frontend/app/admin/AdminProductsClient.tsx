'use client';

import { useEffect, useState } from 'react';
import { productService } from '@/lib/services/productService';
import { categoryService } from '@/lib/services/categoryService';
import { getImageUrl } from '@/lib/utils/imageUtils';
import type { Product, CreateProductDto, Category, PaginatedResponse } from '@/types';
import ProductModal from '@/components/ProductModal';

const PAGE_SIZE = 5;

export default function AdminProductsClient() {
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  
  // Pagination state
  const [pageNumber, setPageNumber] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [hasPreviousPage, setHasPreviousPage] = useState(false);
  const [hasNextPage, setHasNextPage] = useState(false);

  useEffect(() => {
    loadData();
  }, [pageNumber]);

  const loadData = async () => {
    try {
      //setLoading(true);
      setError(null);
      const [productsResponse, categoriesData] = await Promise.all([
        productService.getPaginated(pageNumber, PAGE_SIZE, undefined, true), // includeInactive = true for admin
        categoryService.getAll(),
      ]);
      setProducts(productsResponse.data);
      setTotalCount(productsResponse.totalCount);
      setTotalPages(productsResponse.totalPages);
      setHasPreviousPage(productsResponse.hasPreviousPage ?? false);
      setHasNextPage(productsResponse.hasNextPage ?? false);
      setCategories(categoriesData);
    } catch (err) {
      setError('خطا در بارگذاری اطلاعات');
      console.error('Error loading data:', err);
    } finally {
      //setLoading(false);
    }
  };

  const handleCreate = async (data: CreateProductDto) => {
    try {
      setError(null);
      setSuccess(null);
      await productService.create(data);
      setSuccess('محصول با موفقیت ایجاد شد');
      // Reset to first page after creating new product
      setPageNumber(1);
      await loadData();
      setIsModalOpen(false);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطا در ایجاد محصول';
      setError(errorMessage);
      console.error('Error creating product:', err);
    }
  };

  const handleUpdate = async (id: number, data: CreateProductDto) => {
    try {
      setError(null);
      setSuccess(null);
      await productService.update(id, data);
      setSuccess('محصول با موفقیت به‌روزرسانی شد');
      await loadData();
      setIsModalOpen(false);
      setEditingProduct(null);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطا در به‌روزرسانی محصول';
      setError(errorMessage);
      console.error('Error updating product:', err);
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('آیا از حذف این محصول اطمینان دارید؟')) {
      return;
    }

    try {
      setError(null);
      setSuccess(null);
      await productService.delete(id);
      setSuccess('محصول با موفقیت حذف شد');
      
      // If current page becomes empty after deletion, go to previous page
      if (products.length === 1 && pageNumber > 1) {
        setPageNumber(pageNumber - 1);
      } else {
        await loadData();
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطا در حذف محصول';
      setError(errorMessage);
      console.error('Error deleting product:', err);
    }
  };

  const handlePageChange = (newPage: number) => {
    if (newPage < 1 || newPage > totalPages || loading) return;
    
    setPageNumber(newPage);
    // Scroll to top of table when page changes
    //window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleEdit = (product: Product) => {
    setEditingProduct(product);
    setIsModalOpen(true);
  };

  const handleAdd = () => {
    setEditingProduct(null);
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
        <div>
          <h2 className="text-2xl font-bold text-gray-800">مدیریت محصولات</h2>
          {totalCount > 0 && (
            <p className="text-sm text-gray-600 mt-1">
              نمایش {((pageNumber - 1) * PAGE_SIZE + 1).toLocaleString('fa-IR')} تا {Math.min(pageNumber * PAGE_SIZE, totalCount).toLocaleString('fa-IR')} از {totalCount.toLocaleString('fa-IR')} محصول
              {totalPages > 1 && ` (صفحه ${pageNumber.toLocaleString('fa-IR')} از ${totalPages.toLocaleString('fa-IR')})`}
            </p>
          )}
        </div>
        <button
          onClick={handleAdd}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
        >
          افزودن محصول
        </button>
      </div>

      {error && (
        <div className="mb-4 p-3 bg-red-100 text-red-700 rounded-lg">{error}</div>
      )}

      {success && (
        <div className="mb-4 p-3 bg-green-100 text-green-700 rounded-lg">{success}</div>
      )}

      {products.length === 0 && !loading && (
        <div className="bg-white rounded-lg shadow-md p-8 text-center">
          <p className="text-gray-600">هیچ محصولی یافت نشد</p>
        </div>
      )}

      {products.length > 0 && (
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  تصویر
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  نام
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  دسته‌بندی
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  قیمت
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  موجودی
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
                  <td className="px-6 py-4 whitespace-nowrap">
                    {product.imageUrl ? (
                      <img
                        src={getImageUrl(product.imageUrl)}
                        alt={product.name}
                        className="h-16 w-16 object-cover rounded"
                        onError={(e) => {
                          (e.target as HTMLImageElement).src = '/placeholder-image.png';
                        }}
                      />
                    ) : (
                      <div className="h-16 w-16 bg-gray-200 rounded flex items-center justify-center text-gray-400 text-xs">
                        بدون تصویر
                      </div>
                    )}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {product.name}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {product.categoryName || '-'}
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
                        {product.discountPercentage && (
                          <div className="text-xs text-green-600 font-bold">
                            {product.discountPercentage}% تخفیف
                          </div>
                        )}
                      </div>
                    ) : (
                      <div>{product.price.toLocaleString('fa-IR')} تومان</div>
                    )}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {product.stockQuantity}
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
                    <div className="flex space-x-2 space-x-reverse">
                      <button
                        onClick={() => handleEdit(product)}
                        className="text-blue-600 hover:text-blue-900"
                      >
                        ویرایش
                      </button>
                      <button
                        onClick={() => handleDelete(product.id)}
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
      )}

      {/* Pagination Controls */}
      {totalPages > 1 && (
        <div className="mt-6 bg-white rounded-lg shadow-md p-4">
          <div className="flex flex-col sm:flex-row justify-between items-center gap-4">
            {/* Pagination Info */}
            <div className="text-sm text-gray-600">
              <span className="font-medium">صفحه {pageNumber.toLocaleString('fa-IR')}</span>
              <span className="mx-2">از</span>
              <span className="font-medium">{totalPages.toLocaleString('fa-IR')}</span>
              <span className="mx-2">•</span>
              <span>{totalCount.toLocaleString('fa-IR')} محصول</span>
            </div>

            {/* Pagination Buttons */}
            <div className="flex items-center gap-2">
              {/* First Page Button */}
              <button
                onClick={() => handlePageChange(1)}
                disabled={!hasPreviousPage || loading}
                className={`px-3 py-2 rounded-lg text-sm font-medium ${
                  !hasPreviousPage || loading
                    ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                    : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
                }`}
                title="صفحه اول"
              >
                اول
              </button>

              {/* Previous Page Button */}
              <button
                onClick={() => handlePageChange(pageNumber - 1)}
                disabled={!hasPreviousPage || loading}
                className={`px-4 py-2 rounded-lg text-sm font-medium ${
                  !hasPreviousPage || loading
                    ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                    : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
                }`}
              >
                قبلی
              </button>
              
              {/* Page Numbers */}
              <div className="flex items-center gap-1">
                {(() => {
                  const pages: (number | string)[] = [];
                  
                  if (totalPages <= 7) {
                    // Show all pages if 7 or fewer
                    for (let i = 1; i <= totalPages; i++) {
                      pages.push(i);
                    }
                  } else {
                    // Always show first page
                    pages.push(1);
                    
                    if (pageNumber > 3) {
                      pages.push('...');
                    }
                    
                    // Show pages around current page
                    const start = Math.max(2, pageNumber - 1);
                    const end = Math.min(totalPages - 1, pageNumber + 1);
                    
                    for (let i = start; i <= end; i++) {
                      if (i !== 1 && i !== totalPages) {
                        pages.push(i);
                      }
                    }
                    
                    if (pageNumber < totalPages - 2) {
                      pages.push('...');
                    }
                    
                    // Always show last page
                    pages.push(totalPages);
                  }
                  
                  return pages.map((page, index) => {
                    if (page === '...') {
                      return (
                        <span key={`ellipsis-${index}`} className="px-2 text-gray-400">
                          ...
                        </span>
                      );
                    }
                    
                    const pageNum = page as number;
                    return (
                      <button
                        key={pageNum}
                        onClick={() => handlePageChange(pageNum)}
                        disabled={loading}
                        className={`px-3 py-2 rounded-lg text-sm font-medium min-w-[40px] ${
                          pageNumber === pageNum
                            ? 'bg-blue-600 text-white'
                            : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
                        } disabled:opacity-50 disabled:cursor-not-allowed`}
                      >
                        {pageNum.toLocaleString('fa-IR')}
                      </button>
                    );
                  });
                })()}
              </div>

              {/* Next Page Button */}
              <button
                onClick={() => handlePageChange(pageNumber + 1)}
                disabled={!hasNextPage || loading}
                className={`px-4 py-2 rounded-lg text-sm font-medium ${
                  !hasNextPage || loading
                    ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                    : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
                }`}
              >
                بعدی
              </button>

              {/* Last Page Button */}
              <button
                onClick={() => handlePageChange(totalPages)}
                disabled={!hasNextPage || loading}
                className={`px-3 py-2 rounded-lg text-sm font-medium ${
                  !hasNextPage || loading
                    ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                    : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
                }`}
                title="صفحه آخر"
              >
                آخر
              </button>
            </div>
          </div>
        </div>
      )}

      {isModalOpen && (
        <ProductModal
          product={editingProduct}
          categories={categories}
          onClose={() => {
            setIsModalOpen(false);
            setEditingProduct(null);
          }}
          onSave={(data) => {
            if (editingProduct) {
              handleUpdate(editingProduct.id, data);
            } else {
              handleCreate(data);
            }
          }}
        />
      )}
    </div>
  );
}

