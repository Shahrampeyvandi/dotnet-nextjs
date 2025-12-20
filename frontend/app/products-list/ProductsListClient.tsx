'use client';

import { useEffect, useState, useRef } from 'react';
import { useSearchParams, useRouter } from 'next/navigation';
import { productService } from '@/lib/services/productService';
import { cartService } from '@/lib/services/cartService';
import { getImageUrl } from '@/lib/utils/imageUtils';
import Link from 'next/link';
import AddToCartButton from '@/components/AddToCartButton';
import type { Product, Category, Cart, PaginatedResponse } from '@/types';

type SortOption = 'relevant' | 'bestselling' | 'mostViewed' | 'newest' | 'cheapest' | 'mostExpensive';

interface ProductsListClientProps {
  initialProducts: Product[];
  initialCategories: Category[];
  initialPageNumber: number;
  initialSelectedCategory: number | null;
  initialTotalCount: number;
  initialTotalPages: number;
  pageSize: number;
}

export default function ProductsListClient({
  initialProducts,
  initialCategories,
  initialPageNumber,
  initialSelectedCategory,
  initialTotalCount,
  initialTotalPages,
  pageSize,
}: ProductsListClientProps) {
  const searchParams = useSearchParams();
  const router = useRouter();
  const [products, setProducts] = useState<Product[]>(initialProducts);
  const [categories] = useState<Category[]>(initialCategories);
  const [selectedCategory, setSelectedCategory] = useState<number | null>(initialSelectedCategory);
  const [loading, setLoading] = useState(false);
  const [cartCount, setCartCount] = useState(0);
  const [cart, setCart] = useState<Cart | null>(null);
  const [sortBy, setSortBy] = useState<SortOption>('relevant');
  const [showSortMenu, setShowSortMenu] = useState(false);
  const sortMenuRef = useRef<HTMLDivElement>(null);
  
  // Pagination state
  const [pageNumber, setPageNumber] = useState(initialPageNumber);
  const [totalCount, setTotalCount] = useState(initialTotalCount);
  const [totalPages, setTotalPages] = useState(initialTotalPages);

  useEffect(() => {
    loadCart();
    
    // Read category and page from URL params
    const categoryParam = searchParams.get('category');
    const pageParam = searchParams.get('page');
    
    if (categoryParam) {
      const categoryId = parseInt(categoryParam, 10);
      if (!isNaN(categoryId)) {
        setSelectedCategory(categoryId);
      }
    } else {
      setSelectedCategory(null);
    }
    
    if (pageParam) {
      const page = parseInt(pageParam, 10);
      if (!isNaN(page) && page > 0) {
        setPageNumber(page);
      }
    } else {
      setPageNumber(1);
    }
  }, [searchParams]);

  useEffect(() => {
    // Only reload if URL params changed
    const categoryParam = searchParams.get('category');
    const pageParam = searchParams.get('page');
    const currentCategory = categoryParam ? parseInt(categoryParam, 10) : null;
    const currentPage = pageParam ? parseInt(pageParam, 10) : 1;
    
    if (currentCategory !== selectedCategory || currentPage !== pageNumber) {
      loadProducts();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchParams]);

  const handleCategoryChange = (categoryId: number | null) => {
    if (categoryId) {
      router.push(`/products-list?category=${categoryId}&page=1`);
    } else {
      router.push('/products-list?page=1');
    }
  };

  const handlePageChange = (newPage: number) => {
    const params = new URLSearchParams();
    if (selectedCategory) {
      params.append('category', selectedCategory.toString());
    }
    params.append('page', newPage.toString());
    router.push(`/products-list?${params.toString()}`);
  };

  // Close sort menu when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (sortMenuRef.current && !sortMenuRef.current.contains(event.target as Node)) {
        setShowSortMenu(false);
      }
    };

    if (showSortMenu) {
      document.addEventListener('mousedown', handleClickOutside);
    }

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [showSortMenu]);

  const loadProducts = async () => {
    try {
      setLoading(true);
      const categoryParam = searchParams.get('category');
      const pageParam = searchParams.get('page');
      const categoryId = categoryParam ? parseInt(categoryParam, 10) : null;
      const page = pageParam ? parseInt(pageParam, 10) : 1;
      
      const response: PaginatedResponse<Product> = await productService.getPaginated(
        page,
        pageSize,
        categoryId || undefined
      );
      setProducts(response.data);
      setTotalCount(response.totalCount);
      setTotalPages(response.totalPages);
      setPageNumber(page);
      setSelectedCategory(categoryId);
    } catch (error) {
      console.error('Error loading products:', error);
    } finally {
      setLoading(false);
    }
  };

  const loadCart = async () => {
    try {
      const cartData = await cartService.getCart();
      setCart(cartData);
      setCartCount(cartData.totalItems);
    } catch (error) {
      console.error('Error loading cart:', error);
      setCart(null);
      setCartCount(0);
    }
  };

  const handleAddToCartSuccess = (updatedCart: Cart) => {
    setCart(updatedCart);
    setCartCount(updatedCart.totalItems);
  };

  // Note: Sorting is currently client-side. For better performance with large datasets,
  // consider implementing server-side sorting in the backend
  const sortedProducts = [...products].sort((a, b) => {
    switch (sortBy) {
      case 'cheapest':
        return a.price - b.price;
      case 'mostExpensive':
        return b.price - a.price;
      case 'newest':
        return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
      case 'bestselling':
        // Mock: sort by stock quantity (lower stock = more popular)
        return a.stockQuantity - b.stockQuantity;
      case 'mostViewed':
        // Mock: sort by ID (higher ID = newer = more viewed)
        return b.id - a.id;
      default:
        return 0;
    }
  });

  // Mock rating function (you can replace this with real ratings later)
  const getRating = (productId: number) => {
    const ratings = [4.6, 4.5, 4.4, 4.3, 4.2, 4.1, 4.0];
    return ratings[productId % ratings.length];
  };

  // Get actual discount from product
  const getDiscount = (product: Product): number | null => {
    return product.hasActiveDiscount && product.discountPercentage ? product.discountPercentage : null;
  };

  // Mock special tags
  const getSpecialTag = (productId: number, discount: number | null) => {
    if (productId % 5 === 0) {
      return { text: 'پیشنهاد شگفت انگیز', icon: '📢', type: 'amazing', hasTimer: true };
    }
    if (discount && discount > 10) {
      return { text: 'فروش ویژه', icon: '', type: 'special', hasTimer: false };
    }
    return null;
  };

  // Countdown timer component
  const CountdownTimer = () => {
    const [time, setTime] = useState({ hours: 13, minutes: 49, seconds: 50 });
    
    useEffect(() => {
      const interval = setInterval(() => {
        setTime(prev => {
          
          let { hours, minutes, seconds } = prev;
          seconds--;
          if (seconds < 0) {
            seconds = 59;
            minutes--;
            if (minutes < 0) {
              minutes = 59;
              hours--;
              if (hours < 0) {
                hours = 23;
              }
            }
          }
          return { hours, minutes, seconds };
        });
      }, 1000);
      
      return () => clearInterval(interval);
    }, []);
    
    return (
      <div className="text-xs font-bold text-gray-800 mt-1">
        {String(time.hours).padStart(2, '0')} : {String(time.minutes).padStart(2, '0')} : {String(time.seconds).padStart(2, '0')}
      </div>
    );
  };

  const sortOptions: { value: SortOption; label: string }[] = [
    { value: 'relevant', label: 'مرتبط ترین' },
    { value: 'bestselling', label: 'پرفروش ترین' },
    { value: 'mostViewed', label: 'پربازدیدترین' },
    { value: 'newest', label: 'جدیدترین' },
    { value: 'cheapest', label: 'ارزان ترین' },
    { value: 'mostExpensive', label: 'گران ترین' },
  ];

  return (
    <div className="bg-gray-50 min-h-screen">
      <div className="container mx-auto px-4 py-6">
        {/* Header with cart */}
        <div className="flex justify-between items-center mb-4">
          <h1 className="text-2xl font-bold text-gray-800">محصولات</h1>
          <Link
            href="/cart"
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 relative"
          >
            سبد خرید
            {cartCount > 0 && (
              <span className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full w-6 h-6 flex items-center justify-center text-xs">
                {cartCount}
              </span>
            )}
          </Link>
        </div>

        {/* Categories filter */}
        <div className="mb-4">
          <div className="flex gap-2 flex-wrap">
            <button
              onClick={() => handleCategoryChange(null)}
              className={`px-4 py-2 rounded-lg text-sm ${
                selectedCategory === null
                  ? 'bg-blue-600 text-white'
                  : 'bg-white text-gray-700 border border-gray-300'
              }`}
            >
              همه
            </button>
            {categories.map((category) => (
              <button
                key={category.id}
                onClick={() => handleCategoryChange(category.id)}
                className={`px-4 py-2 rounded-lg text-sm ${
                  selectedCategory === category.id
                    ? 'bg-blue-600 text-white'
                    : 'bg-white text-gray-700 border border-gray-300'
                }`}
              >
                {category.name}
              </button>
            ))}
          </div>
        </div>

        {/* Products header with count and sort */}
        <div className="bg-white rounded-lg p-3 mb-3 flex justify-between items-center border-b border-gray-200">
          <div className="text-gray-700 text-sm">
            <span className="font-bold">{totalCount.toLocaleString('fa-IR')}</span>
            <span className="mr-1"> کالا</span>
          </div>
          <div className="flex items-center gap-4">
            <div className="relative" ref={sortMenuRef}>
              <button
                onClick={() => setShowSortMenu(!showSortMenu)}
                className="flex items-center gap-1 px-3 py-1.5 text-sm text-gray-700 hover:text-blue-600"
              >
                <span>مرتب سازی</span>
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                </svg>
              </button>
              {showSortMenu && (
                <div className="absolute left-0 mt-1 w-40 bg-white rounded-lg shadow-xl border border-gray-200 z-20">
                  {sortOptions.map((option) => (
                    <button
                      key={option.value}
                      onClick={() => {
                        setSortBy(option.value);
                        setShowSortMenu(false);
                      }}
                      className={`w-full text-right px-4 py-2 text-sm hover:bg-gray-50 first:rounded-t-lg last:rounded-b-lg ${
                        sortBy === option.value ? 'bg-blue-50 text-blue-600 font-semibold' : 'text-gray-700'
                      }`}
                    >
                      {option.label}
                    </button>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>

        {loading && (
          <div className="text-center py-8">
            <div className="text-gray-600">در حال بارگذاری...</div>
          </div>
        )}

        {/* Products grid */}
        {!loading && (
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-3">
            {sortedProducts.map((product) => {
              const rating = getRating(product.id);
              const discount = getDiscount(product);
              const specialTag = getSpecialTag(product.id, discount);
              const displayPrice = product.hasActiveDiscount && product.finalPrice !== undefined 
                ? product.finalPrice 
                : product.price;
              const originalPrice = discount ? product.price : null;

              return (
                <div
                  key={product.id}
                  className="bg-white rounded-lg border border-gray-200 overflow-hidden hover:border-blue-400 hover:shadow-md transition-all relative flex flex-col"
                >
                  {/* Special tag - top right */}
                  {specialTag && (
                    <div className="absolute top-1 right-1 z-10 flex flex-col items-end">
                      <div className={`px-2 py-0.5 rounded text-[10px] font-bold mb-1 ${
                        specialTag.type === 'amazing' 
                          ? 'bg-orange-500 text-white' 
                          : 'bg-red-600 text-white'
                      }`}>
                        {specialTag.text}
                      </div>
                      {specialTag.type === 'amazing' && (
                        <div className="bg-orange-500 text-white px-1.5 py-0.5 rounded text-[10px] font-semibold">
                          سفارشی
                        </div>
                      )}
                    </div>
                  )}

                  {/* Discount badge - top left */}
                  {discount && (
                    <div className="absolute top-1 left-1 z-10 bg-red-600 text-white px-1.5 py-0.5 rounded text-[10px] font-bold">
                      {discount}%
                    </div>
                  )}

                  {/* Product image */}
                  <Link href={`/products/${product.id}`} className="block">
                    <div className="relative w-full h-40 bg-white flex items-center justify-center p-2">
                      {product.imageUrl ? (
                        <img
                          src={getImageUrl(product.imageUrl)}
                          alt={product.name}
                          className="w-full h-full object-contain"
                          onError={(e) => {
                            (e.target as HTMLImageElement).src = '/placeholder-image.png';
                          }}
                        />
                      ) : (
                        <div className="w-full h-full flex items-center justify-center text-gray-400 text-xs">
                          بدون تصویر
                        </div>
                      )}
                    </div>
                  </Link>

                  {/* Product info */}
                  <div className="p-2.5 flex-1 flex flex-col">
                    <Link href={`/products/${product.id}`}>
                      <h3 className="text-xs font-semibold text-gray-800 mb-1 hover:text-blue-600 line-clamp-2 leading-5 min-h-10">
                        {product.name}
                      </h3>
                    </Link>
                    
                    {product.description && (
                      <p className="text-[10px] text-gray-600 mb-1.5 line-clamp-2 leading-4">
                        {product.description}
                      </p>
                    )}

                    {/* Rating */}
                    {rating > 0 && (
                      <div className="flex items-center gap-1 mb-1.5">
                        <div className="flex items-center">
                          {[...Array(5)].map((_, i) => (
                            <svg
                              key={i}
                              className={`w-2.5 h-2.5 ${
                                i < Math.floor(rating) ? 'text-yellow-400 fill-current' : 'text-gray-300 fill-current'
                              }`}
                              viewBox="0 0 20 20"
                            >
                              <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                            </svg>
                          ))}
                        </div>
                        <span className="text-[10px] text-gray-600 font-medium">{rating.toFixed(1)}</span>
                      </div>
                    )}

                    {/* Stock indicator */}
                    {product.stockQuantity > 0 && product.stockQuantity <= 5 && (
                      <div className="text-[10px] text-orange-600 mb-1.5 font-bold">
                        تنها {product.stockQuantity} عدد در انبار باقی مانده
                      </div>
                    )}

                    {/* Countdown timer for special offers */}
                    {specialTag && specialTag.hasTimer && (
                      <div className="mb-1.5">
                        <CountdownTimer />
                      </div>
                    )}

                    {/* Price */}
                    <div className="mb-2 mt-auto">
                      {originalPrice && (
                        <div className="text-[10px] text-gray-400 line-through mb-0.5">
                          {originalPrice.toLocaleString('fa-IR')} تومان
                        </div>
                      )}
                      <div className="text-base font-bold text-green-600 leading-tight">
                        {displayPrice.toLocaleString('fa-IR')} تومان
                      </div>
                    </div>

                    {/* Shipping info */}
                    <div className="text-[10px] text-green-600 mb-2 font-bold">
                      ارسال سریع دیجی کالا
                    </div>

                    {/* Add to cart button */}
                    {product.stockQuantity > 0 ? (
                      <AddToCartButton 
                        productId={product.id} 
                        quantity={1}
                        onSuccess={handleAddToCartSuccess}
                        cart={cart}
                        compact={true}
                      />
                    ) : (
                      <button
                        disabled
                        className="w-full bg-gray-300 text-gray-500 py-1.5 rounded text-xs cursor-not-allowed font-semibold"
                      >
                        ناموجود
                      </button>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        )}

        {/* Pagination Controls */}
        {totalPages > 1 && (
          <div className="mt-6 flex justify-center items-center gap-2 flex-wrap">
            <button
              onClick={() => handlePageChange(pageNumber - 1)}
              disabled={pageNumber === 1}
              className={`px-4 py-2 rounded-lg text-sm ${
                pageNumber === 1
                  ? 'bg-gray-200 text-gray-400 cursor-not-allowed'
                  : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
              }`}
            >
              قبلی
            </button>
            
            <div className="flex gap-1">
              {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                let pageNum: number;
                if (totalPages <= 5) {
                  pageNum = i + 1;
                } else if (pageNumber <= 3) {
                  pageNum = i + 1;
                } else if (pageNumber >= totalPages - 2) {
                  pageNum = totalPages - 4 + i;
                } else {
                  pageNum = pageNumber - 2 + i;
                }
                
                return (
                  <button
                    key={pageNum}
                    onClick={() => handlePageChange(pageNum)}
                    className={`px-3 py-2 rounded-lg text-sm ${
                      pageNumber === pageNum
                        ? 'bg-blue-600 text-white'
                        : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
                    }`}
                  >
                    {pageNum.toLocaleString('fa-IR')}
                  </button>
                );
              })}
            </div>

            <button
              onClick={() => handlePageChange(pageNumber + 1)}
              disabled={pageNumber === totalPages}
              className={`px-4 py-2 rounded-lg text-sm ${
                pageNumber === totalPages
                  ? 'bg-gray-200 text-gray-400 cursor-not-allowed'
                  : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
              }`}
            >
              بعدی
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

