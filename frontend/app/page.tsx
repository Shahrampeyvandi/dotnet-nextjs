import Link from 'next/link';

export default function Home() {
  return (
    <div className="container mx-auto px-4 py-8">
      <div className="text-center">
        <h1 className="text-4xl font-bold text-gray-800 mb-4">
          به فروشگاه آنلاین خوش آمدید
        </h1>
        <p className="text-xl text-gray-600 mb-8">
          سیستم مدیریت کامل فروشگاه
        </p>
        
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mt-12">
          <Link
            href="/categories"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow"
          >
            <h2 className="text-2xl font-semibold text-blue-600 mb-2">
              دسته‌بندی‌ها
            </h2>
            <p className="text-gray-600">مدیریت دسته‌بندی محصولات</p>
          </Link>

          <Link
            href="/products-list"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow"
          >
            <h2 className="text-2xl font-semibold text-green-600 mb-2">
              محصولات
            </h2>
            <p className="text-gray-600">مشاهده و خرید محصولات</p>
          </Link>

          <Link
            href="/customers"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow"
          >
            <h2 className="text-2xl font-semibold text-purple-600 mb-2">
              مشتریان
            </h2>
            <p className="text-gray-600">مدیریت اطلاعات مشتریان</p>
          </Link>

          <Link
            href="/orders"
            className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow"
          >
            <h2 className="text-2xl font-semibold text-orange-600 mb-2">
              سفارشات
            </h2>
            <p className="text-gray-600">مدیریت سفارشات</p>
          </Link>
        </div>
      </div>
    </div>
  );
}
