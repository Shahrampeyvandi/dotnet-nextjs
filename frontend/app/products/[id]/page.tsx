'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { productService } from '@/lib/services/productService';
import { getImageUrl } from '@/lib/utils/imageUtils';
import Link from 'next/link';
import AddToCartButton from '@/components/AddToCartButton';
import type { Product } from '@/types';

export default function ProductDetailPage() {
  const params = useParams();
  const router = useRouter();
  const [product, setProduct] = useState<Product | null>(null);
  const [quantity, setQuantity] = useState(1);
  const [loading, setLoading] = useState(true);
  const [selectedColor, setSelectedColor] = useState('black');
  const [insuranceSelected, setInsuranceSelected] = useState(false);

  useEffect(() => {
    if (params.id) {
      loadProduct(Number(params.id));
    }
  }, [params.id]);

  const loadProduct = async (id: number) => {
    try {
      setLoading(true);
      const data = await productService.getById(id);
      setProduct(data);
    } catch (error) {
      console.error('Error loading product:', error);
      router.push('/products-list');
    } finally {
      setLoading(false);
    }
  };

  const handleAddToCartSuccess = () => {
    router.push('/cart');
  };

  // Mock data for features not in Product type
  const getRating = () => 4.5;
  const getBuyerCount = () => 2298;
  const getCommentCount = () => 2252;
  const getQuestionCount = () => 1593;
  const getDiscount = () => {
    if (product?.hasActiveDiscount && product.discountPercentage) {
      return product.discountPercentage;
    }
    return null;
  };
  const getDisplayPrice = () => {
    if (product?.hasActiveDiscount && product.finalPrice !== undefined) {
      return product.finalPrice;
    }
    return product?.price || 0;
  };
  const getOriginalPrice = () => {
    const discount = getDiscount();
    if (discount && product) {
      return product.price;
    }
    return null;
  };
  const getSpecifications = () => [
    { label: 'فناوری صفحه نمایش', value: 'Super AMOLED' },
    { label: 'نسخه سیستم عامل', value: 'Android 14' },
    { label: 'رزولوشن دوربین اصلی', value: '50 مگاپیکسل' },
    { label: 'اندازه', value: '6.78' },
  ];
  const getColors = () => [
    { name: 'خاکستری', value: 'gray', color: 'bg-gray-300' },
    { name: 'سبز', value: 'green', color: 'bg-green-400' },
    { name: 'مشکی', value: 'black', color: 'bg-black' },
  ];
  const getInsurancePrice = () => 191400;
  const getInsuranceOriginalPrice = () => 382800;
  const getInsuranceDiscount = () => 50;

  if (loading) {
    return (
      <div className="bg-gray-50 min-h-screen">
        <div className="container mx-auto px-4 py-8">
          <div className="text-center">در حال بارگذاری...</div>
        </div>
      </div>
    );
  }

  if (!product) {
    return (
      <div className="bg-gray-50 min-h-screen">
        <div className="container mx-auto px-4 py-8">
          <div className="text-center">محصول یافت نشد</div>
        </div>
      </div>
    );
  }

  const discount = getDiscount();
  const originalPrice = getOriginalPrice();
  const displayPrice = getDisplayPrice();
  const rating = getRating();

  return (
    <div className="bg-gray-50 min-h-screen">
      <div className="container mx-auto px-4 py-6">
        {/* Breadcrumb */}
        <div className="mb-4 text-sm text-gray-600">
          <Link href="/" className="hover:text-blue-600">دیجی کالا</Link>
          <span className="mx-2">/</span>
          {product.categoryName && product.categoryId ? (
            <>
              <Link 
                href={`/products-list?category=${product.categoryId}`} 
                className="hover:text-blue-600"
              >
                {product.categoryName}
              </Link>
              <span className="mx-2">/</span>
            </>
          ) : null}
          <span className="text-gray-800">{product.name}</span>
        </div>

        {/* Special Sale Banner */}
        <div className="bg-red-600 text-white p-3 rounded-lg mb-4 flex items-center justify-between">
          <div className="flex items-center gap-2">
            <span className="font-bold">فروش ویژه</span>
            <span className="text-sm">۳۸٪ فروش رفته</span>
          </div>
          <div className="flex-1 max-w-xs mr-4">
            <div className="w-full bg-red-800 rounded-full h-2">
              <div className="bg-white h-2 rounded-full" style={{ width: '38%' }}></div>
            </div>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
          {/* Main Content - Left Side */}
          <div className="lg:col-span-8">
            <div className="bg-white rounded-lg p-6 mb-4">
              {/* Category Label */}
              <div className="mb-3">
                <span className="text-blue-600 text-sm font-semibold">
                  {product.categoryName || 'داریا'} / {product.categoryName || 'گوشی موبایل داریا'}
                </span>
              </div>

              {/* Product Title */}
              <h1 className="text-xl font-bold text-gray-900 mb-4">{product.name}</h1>

              {/* Rating Section */}
              <div className="flex items-center gap-4 mb-6 flex-wrap">
                <div className="flex items-center gap-2">
                  <div className="flex items-center">
                    {[...Array(5)].map((_, i) => (
                      <svg
                        key={i}
                        className={`w-5 h-5 ${i < Math.floor(rating) ? 'text-yellow-400' : 'text-gray-300'}`}
                        fill="currentColor"
                        viewBox="0 0 20 20"
                      >
                        <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                      </svg>
                    ))}
                  </div>
                  <span className="text-sm font-semibold text-gray-800">
                    {rating.toFixed(1)} امتیاز {getBuyerCount().toLocaleString('fa-IR')} خریدار
                  </span>
                </div>
                <div className="flex gap-2">
                  <button className="px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-xs hover:bg-gray-200">
                    خلاصه دیدگاه ها
                  </button>
                  <button className="px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-xs hover:bg-gray-200">
                    {getCommentCount().toLocaleString('fa-IR')} دیدگاه
                  </button>
                  <button className="px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-xs hover:bg-gray-200">
                    {getQuestionCount().toLocaleString('fa-IR')} پرسش
                  </button>
                </div>
              </div>

              {/* Color Selection */}
              <div className="mb-6">
                <div className="text-sm text-gray-700 mb-2">رنگ: {getColors().find(c => c.value === selectedColor)?.name}</div>
                <div className="flex gap-3">
                  {getColors().map((color) => (
                    <button
                      key={color.value}
                      onClick={() => setSelectedColor(color.value)}
                      className={`w-10 h-10 rounded-full ${color.color} border-2 ${
                        selectedColor === color.value ? 'border-green-500' : 'border-gray-300'
                      } relative`}
                    >
                      {selectedColor === color.value && (
                        <svg className="absolute -top-1 -right-1 w-5 h-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                          <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                        </svg>
                      )}
                    </button>
                  ))}
                </div>
              </div>

              {/* Insurance Option */}
              <div className="mb-6 p-4 bg-gray-50 rounded-lg">
                <div className="flex items-center justify-between">
                  <div>
                    <div className="text-sm font-semibold text-gray-800 mb-1">بیمه</div>
                    <div className="text-xs text-gray-600">بیمه تجهیزات دیجیتال - بیمه سامان</div>
                  </div>
                  <div className="flex items-center gap-3">
                    <div className="text-left">
                      <div className="text-xs text-gray-400 line-through">
                        {getInsuranceOriginalPrice().toLocaleString('fa-IR')} تومان
                      </div>
                      <div className="text-sm font-bold text-gray-800">
                        {getInsurancePrice().toLocaleString('fa-IR')} تومان
                      </div>
                      <div className="text-xs text-red-600 font-semibold">{getInsuranceDiscount()}%</div>
                    </div>
                    <input
                      type="checkbox"
                      checked={insuranceSelected}
                      onChange={(e) => setInsuranceSelected(e.target.checked)}
                      className="w-5 h-5 text-blue-600 rounded"
                    />
                  </div>
                </div>
              </div>

              {/* Key Specifications */}
              <div className="mb-6">
                <div className="text-sm font-semibold text-gray-800 mb-3">ویژگی ها</div>
                <div className="grid grid-cols-3 gap-4 mb-4">
                  {getSpecifications().slice(0, 3).map((spec, index) => (
                    <div key={index} className="text-center p-3 bg-gray-50 rounded-lg">
                      <div className="text-xs text-gray-600 mb-1">{spec.label}</div>
                      <div className="text-sm font-semibold text-gray-800">{spec.value}</div>
                    </div>
                  ))}
                </div>
                <div className="mb-4">
                  <div className="inline-block p-3 bg-gray-50 rounded-lg">
                    <div className="text-xs text-gray-600 mb-1">اندازه</div>
                    <div className="text-sm font-semibold text-gray-800">{getSpecifications()[3].value}</div>
                  </div>
                </div>
                <button className="text-sm text-blue-600 hover:text-blue-800 flex items-center gap-1">
                  مشاهده همه ویژگی ها
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                  </svg>
                </button>
              </div>
            </div>
          </div>

          {/* Right Side - Image and Sidebar */}
          <div className="lg:col-span-4">
            <div className="grid grid-cols-1 gap-6">
              {/* Product Image */}
              <div className="bg-white rounded-lg p-4">
                <div className="relative">
                  {product.imageUrl ? (
                    <img
                      src={getImageUrl(product.imageUrl)}
                      alt={product.name}
                      className="w-full h-auto object-contain"
                      onError={(e) => {
                        (e.target as HTMLImageElement).src = '/placeholder-image.png';
                      }}
                    />
                  ) : (
                    <div className="w-full h-96 bg-gray-200 rounded-lg flex items-center justify-center">
                      <span className="text-gray-400">بدون تصویر</span>
                    </div>
                  )}
                  
                  {/* Overlay badges on image */}
                  <div className="absolute top-4 left-4 flex flex-col gap-2">
                    <div className="bg-gray-800 bg-opacity-75 text-white px-3 py-2 rounded text-sm font-semibold">
                      256 GB
                    </div>
                    <div className="bg-gray-800 bg-opacity-75 text-white px-3 py-2 rounded text-sm font-semibold">
                      8 GB 5G
                    </div>
                  </div>

                  {/* Action buttons */}
                  <div className="absolute top-4 right-4 flex flex-col gap-2">
                    <button className="w-10 h-10 bg-white rounded-full shadow-md flex items-center justify-center hover:bg-gray-100">
                      <svg className="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                      </svg>
                    </button>
                    <button className="w-10 h-10 bg-white rounded-full shadow-md flex items-center justify-center hover:bg-gray-100">
                      <svg className="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
                      </svg>
                    </button>
                    <button className="w-10 h-10 bg-white rounded-full shadow-md flex items-center justify-center hover:bg-gray-100">
                      <svg className="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                      </svg>
                    </button>
                  </div>
                </div>
              </div>

              {/* Seller and Pricing Sidebar */}
              <div className="bg-gray-100 rounded-lg p-5">
                {/* Seller Info */}
                <div className="mb-6">
                  <div className="text-sm font-semibold text-gray-800 mb-3">فروشنده</div>
                  <div className="flex items-center gap-3 mb-2">
                    <div className="w-10 h-10 bg-red-600 rounded-full flex items-center justify-center text-white font-bold">
                      د
                    </div>
                    <div>
                      <div className="font-semibold text-gray-800">دیجی کالا</div>
                      <div className="text-xs text-gray-600">۹۲٪ رضایت از کالا</div>
                      <div className="text-xs text-green-600 font-semibold">عملکرد عالی</div>
                    </div>
                  </div>
                  <Link href="#" className="text-xs text-blue-600 hover:text-blue-800">
                    ۸ فروشنده دیگر
                  </Link>
                </div>

                {/* Pricing */}
                <div className="mb-6 pb-6 border-b border-gray-300">
                  {discount && originalPrice && (
                    <div className="flex items-center gap-2 mb-2">
                      <span className="bg-red-600 text-white px-2 py-0.5 rounded text-xs font-bold">
                        {discount}%
                      </span>
                      <span className="text-sm text-gray-400 line-through">
                        {originalPrice.toLocaleString('fa-IR')} تومان
                      </span>
                    </div>
                  )}
                  <div className="text-2xl font-bold text-red-600 mb-2">
                    {displayPrice.toLocaleString('fa-IR')} تومان
                  </div>
                  <div className="flex items-center gap-1 text-xs text-yellow-600">
                    <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                    </svg>
                    <span>بهترین قیمت در ۳۰ روز گذشته</span>
                  </div>
                </div>

                {/* Quantity Selector */}
                {product.stockQuantity > 0 && (
                  <div className="mb-4">
                    <label className="block text-sm text-gray-700 mb-2">تعداد:</label>
                    <input
                      type="number"
                      min="1"
                      max={product.stockQuantity}
                      value={quantity}
                      onChange={(e) => setQuantity(Math.max(1, Math.min(product.stockQuantity, parseInt(e.target.value) || 1)))}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg text-center"
                    />
                  </div>
                )}

                {/* Add to Cart */}
                {product.stockQuantity > 0 ? (
                  <div className="mb-6">
                    <AddToCartButton 
                      productId={product.id} 
                      quantity={quantity}
                      onSuccess={handleAddToCartSuccess}
                    />
                  </div>
                ) : (
                  <button
                    disabled
                    className="w-full bg-gray-300 text-gray-500 py-3 rounded-lg cursor-not-allowed text-sm font-semibold mb-6"
                  >
                    ناموجود
                  </button>
                )}

                {/* Warranty */}
                <div className="mb-4 flex items-start gap-2">
                  <svg className="w-5 h-5 text-green-600 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                  </svg>
                  <div>
                    <div className="text-sm text-gray-800">گارانتی ۱۸ ماهه هماهنگ (داریا همراه)</div>
                    <Link href="#" className="text-xs text-blue-600 hover:text-blue-800">
                      جزئیات
                      <svg className="w-3 h-3 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                      </svg>
                    </Link>
                  </div>
                </div>

                {/* Shipping */}
                <div className="mb-4">
                  <div className="flex items-center gap-2 mb-2">
                    <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
                    </svg>
                    <Link href="#" className="text-sm text-blue-600 hover:text-blue-800">
                      روشها و هزینه های ارسال
                    </Link>
                  </div>
                  <div className="text-xs text-gray-700 mr-7 space-y-1">
                    <div>• توسط دیجی کالا</div>
                    <div>• ارسال سریع دیجی کالا</div>
                  </div>
                  <div className="flex items-center gap-2 mt-2 mr-7">
                    <svg className="w-4 h-4 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                    </svg>
                    <span className="text-xs text-gray-700">ویژه اعضای پلاس</span>
                  </div>
                  <div className="text-xs text-gray-600 mr-7 mt-1">
                    ارسال سریع و رایگان دیجی کالا (فقط تهران و کرج)
                  </div>
                </div>

                {/* Digiclub Points */}
                <div className="flex items-center gap-2">
                  <svg className="w-5 h-5 text-yellow-500" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clipRule="evenodd" />
                  </svg>
                  <span className="text-sm text-gray-800">۱۵۰ امتیاز دیجی کلاب</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

