
# 💾 اصول ACID در تراکنش‌های پایگاه داده با C# و Entity Framework Core

در پایگاه داده‌ها، ACID مجموعه‌ای از چهار ویژگی است که برای اطمینان از **صحت و امنیت اجرای تراکنش‌ها** تعریف شده‌اند. این اصول به برنامه‌نویسان کمک می‌کنند تراکنش‌هایی بنویسند که قابل اطمینان، ایمن و بدون تداخل باشند.

---

## 🔹 1. Atomicity (اتمی بودن)

**تعریف:** یک تراکنش باید یا **کامل انجام شود** یا اگر خطایی رخ دهد، **هیچ‌کدام از تغییرات آن اعمال نشود**. این اصل از بروز تغییرات ناقص جلوگیری می‌کند.

### 📌 مثال: انتقال وجه بین دو حساب

```csharp
using var context = new AppDbContext();
using var transaction = context.Database.BeginTransaction();

try
{
    var from = context.Accounts.Find(1);
    var to = context.Accounts.Find(2);

    from.Balance -= 100000;
    to.Balance += 100000;

    context.SaveChanges();
    transaction.Commit();
}
catch
{
    transaction.Rollback(); // بازگشت همه تغییرات
}
```

---

## 🔹 2. Consistency (سازگاری)

**تعریف:** داده‌ها باید همیشه **قوانین تجاری و محدودیت‌های پایگاه داده** را رعایت کنند. هیچ تراکنشی نباید باعث شود پایگاه داده در وضعیت ناسازگار قرار گیرد.

### 📌 مثال: جلوگیری از منفی شدن موجودی

```csharp
var account = context.Accounts.Find(1);

if (account.Balance >= 200000)
{
    account.Balance -= 200000;
    context.SaveChanges(); // سازگاری حفظ شده
}
else
{
    Console.WriteLine("موجودی کافی نیست!"); // نقض جلوگیری شد
}
```

---

## 🔹 3. Isolation (ایزوله بودن)

**تعریف:** تراکنش‌ها باید **مستقل از یکدیگر** اجرا شوند. پایگاه داده باید طوری عمل کند که هم‌زمانی چند تراکنش باعث تداخل در داده‌ها نشود.

### 📌 مثال: سطح ایزولیشن SERIALIZABLE برای جلوگیری از هم‌زمانی ناخواسته

```csharp
using var connection = context.Database.GetDbConnection();
connection.Open();

using var command = connection.CreateCommand();
command.CommandText = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE";
command.ExecuteNonQuery();

using var transaction = context.Database.BeginTransaction();

var product = context.Products.Find(5); // فقط یک کالا در انبار موجود است
if (product.Stock > 0)
{
    product.Stock -= 1;
    context.SaveChanges();
    transaction.Commit();
}
else
{
    transaction.Rollback();
}
```

> این سطح ایزولیشن، بالاترین سطح ایمنی را فراهم می‌کند ولی ممکن است باعث کاهش سرعت در تراکنش‌های زیاد شود.

---

## 🔹 4. Durability (ماندگاری)

**تعریف:** پس از اینکه یک تراکنش با موفقیت انجام و `Commit` شد، داده‌های آن حتی در صورت **قطع برق یا کرش سیستم** نیز باقی می‌مانند.

### 📌 مثال: کاهش موجودی که بعد از Commit باقی می‌ماند

```csharp
using var context = new AppDbContext();
using var transaction = context.Database.BeginTransaction();

try
{
    var product = context.Products.Find(10);
    product.Stock -= 1;

    context.SaveChanges();
    transaction.Commit(); // داده‌ها ماندگار می‌شوند
}
catch
{
    transaction.Rollback();
}
```

---

## 🎯 جمع‌بندی

| اصل | کاربرد | تضمین می‌کند که... |
|-----|--------|--------------------|
| ✅ **Atomicity** | اجرای کامل یا لغو کامل | تراکنش نصفه اجرا نشود |
| ✅ **Consistency** | حفظ قوانین و قیود | داده‌ها در وضعیت معتبر بمانند |
| ✅ **Isolation** | جلوگیری از تداخل | تراکنش‌ها مستقل از هم اجرا شوند |
| ✅ **Durability** | ثبت دائمی تغییرات | داده‌ها پس از Commit باقی بمانند |

---

## 📚 پیش‌نیازها برای اجرای کدها

- [.NET 6/7](https://dotnet.microsoft.com/)
- Entity Framework Core
- SQL Server (لوکال یا Azure)
- Visual Studio / Rider / VS Code

---

## 🛠️ راه‌اندازی سریع

1. ایجاد دیتابیس:
   ```bash
   dotnet ef database update
   ```

2. اجرای پروژه:
   ```bash
   dotnet run
   ```

---

> این پروژه برای آشنایی عملی با اصول ACID طراحی شده و برای استفاده در آموزش، مصاحبه‌ها و پروژه‌های حرفه‌ای توصیه می‌شود.
