# projectPart1 - منصة الفن والمعارض

تطبيق ASP.NET Core Razor Pages لإدارة الأعمال الفنية والفنانين والمعارض (بيئة عمانية).

## الميزات الرئيسية

- إدارة الأعمال الفنية (Artworks) مع البحث والتصفية
- إدارة بيانات الفنانين (Artists)
- إدارة المعارض الفنية (Exhibitions)
- تحميل الصور وحفظها
- تتبع عدد الإعجابات للأعمال الفنية
- CORS مفعل لدعم React

## التكنولوجيا المستخدمة

- **Framework**: ASP.NET Core 8.0
- **Views**: Razor Pages
- **Storage**: In-memory DataStore
- **Services**: Dependency Injection

## التوزيع الوظيفي

| الفريق | المساهمة | المهام |
|---|---|---|
| Sultan | 50% | إدارة المشاريع، ArtworkService، DataStore |
| Abdulla | 50% | نمذجة البيانات، ArtistService، الصفحات |

## المشروع في المشروع

```
projectPart1/
├── Models/           # كائنات البيانات
├── Services/         # الخدمات (DataStore, ArtworkService, etc.)
├── Pages/            # صفحات Razor
├── wwwroot/          # الملفات الثابتة (صور، CSS، JS)
├── Program.cs        # إعدادات التطبيق
└── appsettings.json  # الإعدادات
```

## كيفية التشغيل

1. استنساخ المستودع:
```bash
git clone https://github.com/<your-username>/projectPart1.git
cd projectPart1
```

2. استعادة الحزم:
```bash
dotnet restore
```

3. تشغيل التطبيق:
```bash
dotnet run
```

4. افتح المتصفح على: `https://localhost:5001`

## الهيكل قاعدة البيانات (في الذاكرة)

### جداول البيانات الرئيسية

- **Artists** - بيانات الفنانين
- **Artworks** - الأعمال الفنية
- **Exhibitions** - المعارض

