# Frontend - فروشگاه آنلاین

پروژه Frontend با Next.js برای سیستم مدیریت فروشگاه آنلاین

## ویژگی‌ها

- ✅ Next.js 14 با App Router
- ✅ TypeScript
- ✅ Tailwind CSS
- ✅ رابط کاربری فارسی (RTL)
- ✅ صفحات کامل برای مدیریت:
  - دسته‌بندی‌ها (Categories)
  - محصولات (Products)
  - مشتریان (Customers)
  - سفارشات (Orders)

## نصب و راه‌اندازی

1. نصب dependencies:
```bash
npm install
```

2. ایجاد فایل `.env.local`:
```env
NEXT_PUBLIC_API_URL=http://localhost:5028/api
```

3. اجرای پروژه:
```bash
npm run dev
```

پروژه روی `http://localhost:3000` اجرا می‌شود.

## ساختار پروژه

```
frontend/
├── app/                    # صفحات Next.js
│   ├── categories/         # صفحه دسته‌بندی‌ها
│   ├── products/          # صفحه محصولات
│   ├── customers/         # صفحه مشتریان
│   └── orders/            # صفحه سفارشات
├── components/             # کامپوننت‌های React
│   ├── Navigation.tsx
│   ├── CategoryModal.tsx
│   ├── ProductModal.tsx
│   ├── CustomerModal.tsx
│   └── OrderModal.tsx
├── lib/                    # Utilities و Services
│   ├── api.ts             # API client
│   └── services/          # Service layer
├── types/                  # TypeScript types
└── public/                 # فایل‌های استاتیک
```

## API Services

تمام ارتباطات با Backend از طریق Service Layer انجام می‌شود:

- `categoryService` - مدیریت دسته‌بندی‌ها
- `productService` - مدیریت محصولات
- `customerService` - مدیریت مشتریان
- `orderService` - مدیریت سفارشات

## نکات مهم

- مطمئن شوید Backend API روی `http://localhost:5028` در حال اجرا است
- CORS در Backend برای `http://localhost:3000` فعال شده است
- تمام صفحات از RTL (راست به چپ) پشتیبانی می‌کنند
