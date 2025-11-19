# فروشگاه آنلاین - E-Commerce System

پروژه کامل فروشگاه آنلاین با Clean Architecture شامل Backend (ASP.NET Core) و Frontend (Next.js)

## 🏗️ معماری پروژه

این پروژه از **Clean Architecture** استفاده می‌کند و شامل لایه‌های زیر است:

### Backend (ASP.NET Core 7.0)
- **Domain Layer**: Entities و Domain Models
- **Application Layer**: DTOs, Interfaces, Services, Mappings
- **Infrastructure Layer**: Data Access, Repositories, DbContext
- **Presentation Layer**: Controllers, Middleware

### Frontend (Next.js 14)
- **App Router**: صفحات و Routing
- **Components**: کامپوننت‌های React
- **Services**: API Service Layer
- **Types**: TypeScript Types

## 📋 ویژگی‌ها

### Backend
- ✅ RESTful API با CRUD کامل
- ✅ Entity Framework Core با SQL Server
- ✅ Clean Architecture Pattern
- ✅ Repository Pattern
- ✅ Service Layer
- ✅ CORS Configuration
- ✅ Custom Middleware (Logging, Exception Handling, Rate Limiting)
- ✅ Swagger/OpenAPI Documentation

### Frontend
- ✅ Next.js 14 با App Router
- ✅ TypeScript
- ✅ Tailwind CSS
- ✅ رابط کاربری فارسی (RTL)
- ✅ صفحات مدیریت کامل:
  - دسته‌بندی‌ها (Categories)
  - محصولات (Products)
  - مشتریان (Customers)
  - سفارشات (Orders)

## 🚀 راه‌اندازی

### پیش‌نیازها
- .NET 7.0 SDK
- Node.js 18+ و npm
- SQL Server (LocalDB یا SQL Server Express)

### Backend

1. نصب پکیج‌ها:
```bash
cd testttt
dotnet restore
```

2. پیکربندی Connection String در `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ECommerceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

3. اجرای پروژه (دیتابیس به صورت خودکار ایجاد می‌شود):
```bash
dotnet run
```

Backend روی `http://localhost:5287` اجرا می‌شود.

### Frontend

1. نصب dependencies:
```bash
cd frontend
npm install
```

2. ایجاد فایل `.env.local`:
```env
NEXT_PUBLIC_API_URL=http://localhost:5287/api
```

3. اجرای پروژه:
```bash
npm run dev
```

Frontend روی `http://localhost:3000` اجرا می‌شود.

## 📁 ساختار پروژه

```
testttt/
├── Domain/                 # Domain Layer
│   └── Entities/          # Domain Entities
├── Application/           # Application Layer
│   ├── DTOs/             # Data Transfer Objects
│   ├── Interfaces/        # Repository & Service Interfaces
│   ├── Services/          # Business Logic
│   └── Mappings/          # Entity to DTO Mappings
├── Infrastructure/        # Infrastructure Layer
│   ├── Data/              # DbContext
│   └── Repositories/      # Repository Implementations
├── Presentation/          # Presentation Layer
│   └── Controllers/       # API Controllers
└── Middleware/            # Custom Middleware

frontend/
├── app/                   # Next.js Pages
├── components/            # React Components
├── lib/                   # Utilities & Services
│   ├── api.ts            # API Client
│   └── services/         # Service Layer
└── types/                 # TypeScript Types
```

## 🔌 API Endpoints

### Categories
- `GET /api/Categories` - لیست دسته‌بندی‌ها
- `GET /api/Categories/{id}` - دریافت دسته‌بندی
- `POST /api/Categories` - ایجاد دسته‌بندی
- `PUT /api/Categories/{id}` - به‌روزرسانی
- `DELETE /api/Categories/{id}` - حذف

### Products
- `GET /api/Products` - لیست محصولات
- `GET /api/Products/{id}` - دریافت محصول
- `POST /api/Products` - ایجاد محصول
- `PUT /api/Products/{id}` - به‌روزرسانی
- `DELETE /api/Products/{id}` - حذف

### Customers
- `GET /api/Customers` - لیست مشتریان
- `GET /api/Customers/{id}` - دریافت مشتری
- `POST /api/Customers` - ایجاد مشتری
- `PUT /api/Customers/{id}` - به‌روزرسانی
- `DELETE /api/Customers/{id}` - حذف

### Orders
- `GET /api/Orders` - لیست سفارشات
- `GET /api/Orders/{id}` - دریافت سفارش
- `POST /api/Orders` - ایجاد سفارش
- `PUT /api/Orders/{id}` - به‌روزرسانی
- `DELETE /api/Orders/{id}` - حذف
- `GET /api/Orders/invoices` - لیست فاکتورها

## 🛠️ تکنولوژی‌ها

### Backend
- ASP.NET Core 7.0
- Entity Framework Core 7.0
- SQL Server
- Swagger/OpenAPI

### Frontend
- Next.js 14
- React 18
- TypeScript
- Tailwind CSS

## 📝 License

این پروژه برای اهداف آموزشی ایجاد شده است.

## 👤 Author

Shahram Peyvandi

