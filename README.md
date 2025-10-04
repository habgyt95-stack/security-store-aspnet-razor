# فروشگاه امنیتی - Security Store

یک سیستم فروشگاه آنلاین کامل برای فروش محصولات امنیتی (دوربین مداربسته، دزدگیر، سنسور) با استفاده از ASP.NET Core 9 Razor Pages.

A complete e-commerce system for selling security products (CCTV cameras, alarms, sensors) built with ASP.NET Core 9 Razor Pages.

## 🏗️ معماری - Architecture

این پروژه با معماری تمیز (Clean Architecture) پیاده‌سازی شده است:

- **SecurityStore.Domain**: موجودیت‌های دامنه و قوانین کسب‌وکار
- **SecurityStore.Application**: منطق اپلیکیشن، رابط‌ها، DTOها
- **SecurityStore.Infrastructure**: پیاده‌سازی دیتابیس، Identity، سرویس‌ها
- **SecurityStore.Web**: لایه ارائه با Razor Pages

## 🚀 ویژگی‌ها - Features

### مدیریت محصولات
- ✅ CRUD کامل محصولات
- ✅ آپلود و مدیریت تصاویر محصول
- ✅ انواع محصول (Variants) با قیمت‌های متفاوت
- ✅ ایمپورت محصولات از CSV
- ✅ دسته‌بندی سلسله مراتبی
- ✅ برندها و تگ‌ها
- ✅ محصولات ویژه (Featured)

### مدیریت موجودی
- ✅ پیگیری موجودی بصورت Real-time
- ✅ گزارش تاریخچه تغییرات موجودی (Stock Logs)
- ✅ هشدار کمبود موجودی

### سیستم سفارش
- ✅ سبد خرید مبتنی بر Session
- ✅ فرایند Checkout کامل
- ✅ مدیریت سفارشات
- ✅ پیگیری وضعیت سفارش
- ✅ یکپارچه‌سازی با سیستم پرداخت (IPaymentService)
- ✅ پرداخت تستی (FakePayment) برای توسعه

### پنل مدیریت
- ✅ داشبورد با KPIهای کلیدی
- ✅ مدیریت محصولات
- ✅ مدیریت سفارشات
- ✅ مدیریت مشتریان
- ✅ مدیریت تبلیغات و تخفیف‌ها
- ✅ مدیریت منو و تنظیمات سایت
- ✅ مدیریت رسانه (Media Manager)
- ✅ گزارش‌های حسابرسی (Audit Logs)

### احراز هویت و مجوزها
- ✅ ASP.NET Core Identity
- ✅ سه نقش: Admin, Manager, Customer
- ✅ ثبت‌نام و ورود کاربران
- ✅ مدیریت پروفایل

### سیستم مدیریت محتوا (CMS)
- ✅ مدیریت منوها
- ✅ تنظیمات سایت
- ✅ مدیریت فایل‌های رسانه‌ای

## 🛠️ تکنولوژی‌ها - Technologies

- **ASP.NET Core 9.0**: فریم‌ورک اصلی
- **Razor Pages**: لایه ارائه
- **Entity Framework Core 9.0**: ORM
- **SQL Server**: دیتابیس (با پشتیبانی از LocalDB)
- **ASP.NET Core Identity**: احراز هویت و مجوزها
- **MediatR**: پیاده‌سازی الگوی Mediator
- **AutoMapper**: نگاشت اشیاء
- **Bootstrap 5**: رابط کاربری
- **Bootstrap Icons**: آیکون‌ها
- **CsvHelper**: ایمپورت/اکسپورت CSV
- **FluentValidation**: اعتبارسنجی

## 📋 پیش‌نیازها - Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) یا SQL Server LocalDB
- یک IDE مانند [Visual Studio 2022](https://visualstudio.microsoft.com/) یا [VS Code](https://code.visualstudio.com/)

## 🚀 راه‌اندازی - Getting Started

### 1. کلون کردن مخزن
```bash
git clone https://github.com/habgyt95-stack/security-store-aspnet-razor.git
cd security-store-aspnet-razor
```

### 2. تنظیم Connection String

فایل `appsettings.json` در پروژه `SecurityStore.Web` را ویرایش کنید:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SecurityStoreDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. اجرای Migrations

```bash
cd SecurityStore.Web
dotnet ef database update --project ../SecurityStore.Infrastructure
```

یا از Package Manager Console در Visual Studio:

```powershell
Update-Database
```

### 4. اجرای برنامه

```bash
dotnet run --project SecurityStore.Web
```

برنامه روی `https://localhost:5001` اجرا خواهد شد.

## 👤 کاربران پیش‌فرض - Default Users

بعد از اولین اجرا، کاربران زیر بصورت خودکار ایجاد می‌شوند:

| نقش | ایمیل | رمز عبور |
|-----|-------|---------|
| Admin | admin@securitystore.com | Admin@123 |
| Manager | manager@securitystore.com | Manager@123 |

## 📁 ساختار پروژه - Project Structure

```
security-store-aspnet-razor/
├── SecurityStore.Domain/          # موجودیت‌های دامنه و Enums
│   ├── Entities/
│   ├── Enums/
│   └── Common/
├── SecurityStore.Application/     # منطق اپلیکیشن
│   ├── Interfaces/
│   ├── DTOs/
│   └── Features/
├── SecurityStore.Infrastructure/  # پیاده‌سازی دیتابیس و سرویس‌ها
│   ├── Data/
│   ├── Identity/
│   └── Services/
└── SecurityStore.Web/            # لایه ارائه
    ├── Pages/
    │   ├── Account/
    │   ├── Admin/
    │   ├── Products/
    │   └── Cart/
    └── wwwroot/
```

## 🗃️ مدل دیتا - Data Model

### موجودیت‌های اصلی:

- **Category**: دسته‌بندی محصولات (سلسله مراتبی)
- **Product**: محصولات با قیمت، موجودی، تصاویر
- **ProductImage**: تصاویر محصول
- **ProductVariant**: انواع محصول
- **Order**: سفارشات
- **OrderItem**: آیتم‌های سفارش
- **StockLog**: گزارش تغییرات موجودی
- **Promotion**: تبلیغات و تخفیف‌ها
- **SiteSetting**: تنظیمات سایت
- **MenuItem**: آیتم‌های منو
- **MediaFile**: فایل‌های رسانه‌ای
- **AuditLog**: گزارش‌های حسابرسی

## 💳 سیستم پرداخت - Payment System

پروژه شامل یک رابط `IPaymentService` است که می‌توانید آن را با درگاه پرداخت واقعی پیاده‌سازی کنید. در حال حاضر `FakePaymentService` برای توسعه و تست استفاده می‌شود.

## 🔐 امنیت - Security

- ✅ رمزهای عبور با هش شده با ASP.NET Core Identity
- ✅ مجوزدهی مبتنی بر نقش
- ✅ محافظت از CSRF
- ✅ اعتبارسنجی ورودی‌ها
- ✅ گزارش‌های حسابرسی برای عملیات حساس

## 📝 یادداشت‌ها - Notes

- تمامی کامنت‌های کد به زبان فارسی نوشته شده است
- پروژه بدون Docker و CI/CD طراحی شده است
- تست‌های واحد در این نسخه شامل نشده است
- از Bootstrap 5 RTL برای پشتیبانی از زبان فارسی استفاده شده است

## 📄 مجوز - License

این پروژه تحت مجوز MIT منتشر شده است. برای جزئیات بیشتر فایل [LICENSE](LICENSE) را مشاهده کنید.

## 🤝 مشارکت - Contributing

مشارکت‌ها خوشامدید است! لطفاً:

1. Fork کنید
2. یک branch ویژگی جدید ایجاد کنید
3. تغییرات خود را commit کنید
4. به branch خود push کنید
5. یک Pull Request باز کنید

## 📧 تماس - Contact

برای سوالات و پشتیبانی، لطفاً یک issue در GitHub ایجاد کنید.

---

**توسعه داده شده با ❤️ در ایران**

**Developed with ❤️ in Iran**