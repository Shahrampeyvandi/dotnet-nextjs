'use client';

import { useState, useEffect, useRef } from 'react';
import type { Product, CreateProductDto, Category } from '@/types';
import { fileUploadService } from '@/lib/services/fileUploadService';
import { getImageUrl } from '@/lib/utils/imageUtils';

interface ProductModalProps {
  product: Product | null;
  categories: Category[];
  onClose: () => void;
  onSave: (data: CreateProductDto) => void;
}

export default function ProductModal({ product, categories, onClose, onSave }: ProductModalProps) {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState('');
  const [discountPercentage, setDiscountPercentage] = useState('');
  const [discountStartDate, setDiscountStartDate] = useState('');
  const [discountEndDate, setDiscountEndDate] = useState('');
  const [stockQuantity, setStockQuantity] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [isActive, setIsActive] = useState(true);
  const [categoryId, setCategoryId] = useState('');
  const [uploading, setUploading] = useState(false);
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (product) {
      setName(product.name);
      setDescription(product.description || '');
      setPrice(product.price.toString());
      setDiscountPercentage(product.discountPercentage?.toString() || '');
      setDiscountStartDate(product.discountStartDate ? product.discountStartDate.substring(0, 16) : '');
      setDiscountEndDate(product.discountEndDate ? product.discountEndDate.substring(0, 16) : '');
      setStockQuantity(product.stockQuantity.toString());
      setImageUrl(product.imageUrl || '');
      setIsActive(product.isActive);
      setCategoryId(product.categoryId.toString());
      if (product.imageUrl) {
        setPreviewUrl(getImageUrl(product.imageUrl));
      }
    } else {
      // Reset form when creating new product
      setName('');
      setDescription('');
      setPrice('');
      setDiscountPercentage('');
      setDiscountStartDate('');
      setDiscountEndDate('');
      setStockQuantity('');
      setImageUrl('');
      setIsActive(true);
      setCategoryId('');
      setPreviewUrl(null);
    }
  }, [product]);

  const handleFileSelect = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    // Validate file type
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      setUploadError('فرمت فایل نامعتبر است. فقط تصاویر jpg, png, gif, webp مجاز است.');
      return;
    }

    // Validate file size (5MB)
    if (file.size > 5 * 1024 * 1024) {
      setUploadError('حجم فایل نباید بیشتر از 5 مگابایت باشد.');
      return;
    }

    try {
      setUploading(true);
      setUploadError(null);
      
      // Create preview
      const reader = new FileReader();
      reader.onloadend = () => {
        setPreviewUrl(reader.result as string);
      };
      reader.readAsDataURL(file);

      // Upload file
      const uploadedUrl = await fileUploadService.uploadProductImage(file);
      setImageUrl(uploadedUrl);
      setUploadError(null);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'خطا در آپلود تصویر';
      setUploadError(errorMessage);
      console.error('Error uploading image:', err);
    } finally {
      setUploading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    onSave({
      name,
      description,
      price: parseFloat(price),
      discountPercentage: discountPercentage ? parseFloat(discountPercentage) : undefined,
      discountStartDate: discountStartDate || undefined,
      discountEndDate: discountEndDate || undefined,
      stockQuantity: parseInt(stockQuantity),
      imageUrl,
      isActive,
      categoryId: parseInt(categoryId),
    });
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <h2 className="text-2xl font-bold mb-4">
          {product ? 'ویرایش محصول' : 'افزودن محصول'}
        </h2>
        <form onSubmit={handleSubmit}>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                نام
              </label>
              <input
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                دسته‌بندی
              </label>
              <select
                value={categoryId}
                onChange={(e) => setCategoryId(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
                required
              >
                <option value="">انتخاب کنید</option>
                {categories.map((cat) => (
                  <option key={cat.id} value={cat.id}>
                    {cat.name}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                قیمت
              </label>
              <input
                type="number"
                step="0.01"
                value={price}
                onChange={(e) => setPrice(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                موجودی
              </label>
              <input
                type="number"
                value={stockQuantity}
                onChange={(e) => setStockQuantity(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                درصد تخفیف (%)
              </label>
              <input
                type="number"
                step="0.01"
                min="0"
                max="100"
                value={discountPercentage}
                onChange={(e) => setDiscountPercentage(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
                placeholder="مثلاً 20 برای 20%"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                تاریخ شروع تخفیف
              </label>
              <input
                type="datetime-local"
                value={discountStartDate}
                onChange={(e) => setDiscountStartDate(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                تاریخ پایان تخفیف
              </label>
              <input
                type="datetime-local"
                value={discountEndDate}
                onChange={(e) => setDiscountEndDate(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
            <div className="col-span-2">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                توضیحات
              </label>
              <textarea
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
                rows={3}
              />
            </div>
            <div className="col-span-2">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                تصویر محصول
              </label>
              
              {previewUrl && (
                <div className="mb-3">
                  <img
                    src={previewUrl}
                    alt="Preview"
                    className="h-32 w-32 object-cover rounded border border-gray-300"
                  />
                </div>
              )}

              <div className="flex items-center space-x-4 space-x-reverse">
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/jpeg,image/jpg,image/png,image/gif,image/webp"
                  onChange={handleFileSelect}
                  className="hidden"
                />
                <button
                  type="button"
                  onClick={() => fileInputRef.current?.click()}
                  disabled={uploading}
                  className="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 disabled:opacity-50"
                >
                  {uploading ? 'در حال آپلود...' : 'انتخاب فایل'}
                </button>
                <input
                  type="text"
                  value={imageUrl}
                  onChange={(e) => {
                    setImageUrl(e.target.value);
                    setPreviewUrl(e.target.value || null);
                  }}
                  placeholder="یا آدرس URL تصویر را وارد کنید"
                  className="flex-1 px-3 py-2 border border-gray-300 rounded-md"
                />
              </div>
              
              {uploadError && (
                <p className="mt-2 text-sm text-red-600">{uploadError}</p>
              )}
              
              <p className="mt-1 text-xs text-gray-500">
                فرمت‌های مجاز: JPG, PNG, GIF, WEBP (حداکثر 5MB)
              </p>
            </div>
            <div className="col-span-2">
              <label className="flex items-center">
                <input
                  type="checkbox"
                  checked={isActive}
                  onChange={(e) => setIsActive(e.target.checked)}
                  className="ml-2"
                />
                <span className="text-sm font-medium text-gray-700">فعال</span>
              </label>
            </div>
          </div>
          <div className="flex justify-end space-x-4 mt-6">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 text-gray-700 bg-gray-200 rounded-lg hover:bg-gray-300"
            >
              انصراف
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700"
            >
              ذخیره
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

