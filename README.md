# projectPart1 - Art Gallery Management System
## منصة الفن والمعارض العمانية

ASP.NET Core Razor Pages application for managing artworks, artists, and exhibitions in an Omani cultural context.

---

## 🎯 Project Overview

This is a comprehensive art gallery management system designed for Omani artists and art enthusiasts. The project is divided into multiple parts:

- **Part 1:** In-memory data storage with Razor Pages
- **Part 2:** SQL Server database implementation ✅ COMPLETE
- **Part 3:** Entity Framework Core integration (upcoming)

---

## ✨ Features

### Core Functionality
- 🎨 **Artwork Management** - Browse, search, and filter artworks
- 👨‍🎨 **Artist Profiles** - Complete artist portfolios with statistics
- 🏛️ **Exhibition System** - Create and manage art exhibitions
- 📸 **Image Upload** - Secure image handling and storage
- ❤️ **Engagement Tracking** - Like system and user interactions
- 🔐 **User Authentication** - Role-based access control (Admin/Artist/User/Moderator)
- 🌐 **CORS Support** - Ready for React frontend integration

### New in Part 2
- 💾 **SQL Server Database** - Persistent data storage
- 🔗 **Relational Integrity** - Foreign key constraints
- 📊 **Database Views** - Optimized complex queries
- 📈 **Analytics Ready** - Comprehensive reporting views
- 🔍 **Advanced Indexing** - Performance optimization

---

## 🛠️ Technology Stack

### Backend
- **Framework:** ASP.NET Core 8.0
- **UI:** Razor Pages
- **Database:** SQL Server (Part 2)
- **ORM:** Entity Framework Core (Part 3 - upcoming)
- **Architecture:** Dependency Injection

### Database (Part 2)
- **DBMS:** Microsoft SQL Server
- **Tables:** 6 (Users, Artists, Artworks, Exhibitions, ExhibitionArtworks, ArtworkLikes)
- **Views:** 6 comprehensive views
- **Constraints:** 47+ (PK, FK, CHECK, DEFAULT, UNIQUE, NOT NULL)
- **Sample Data:** 93 realistic records

---

## 👥 Team Contributions

| Team Member | Contribution | Responsibilities |
|-------------|--------------|------------------|
| **Sultan** | 50% | Project setup, ArtworkService, DataStore, Database design, GitHub management |
| **Abdulla** | 50% | Data modeling, ArtistService, Razor Pages, Documentation, Database views |

---

## 📁 Project Structure

```
projectPart1/
├── Database/                    # 🆕 Part 2: SQL Server database
│   ├── 01_CreateDatabase.sql   # Database creation
│   ├── 02_CreateTables.sql     # Tables with constraints
│   ├── 03_CreateViews.sql      # Database views
│   ├── 04_InsertData.sql       # Sample data
│   ├── COMPLETE_SETUP.sql      # All-in-one installation
│   ├── README.md               # Full documentation
│   ├── QUICKSTART.md           # Quick start guide
│   ├── ER_DIAGRAM.md           # Entity-Relationship diagram
│   └── SUMMARY.md              # Implementation summary
├── Models/                      # Data models
│   ├── Artist.cs
│   ├── Artwork.cs
│   └── Exhibition.cs
├── Services/                    # Business logic
│   ├── DataStore.cs            # In-memory storage (Part 1)
│   ├── ArtworkService.cs
│   ├── ArtistService.cs
│   ├── ExhibitionService.cs
│   ├── FileUploadService.cs
│   └── SortingHelper.cs
├── Pages/                       # Razor Pages
│   ├── Index.cshtml            # Artwork gallery
│   ├── ArtworkDetails.cshtml
│   ├── AddArtwork.cshtml
│   ├── Artists.cshtml
│   └── Exhibitions.cshtml
├── wwwroot/                     # Static files
│   ├── css/
│   ├── js/
│   ├── images/
│   └── lib/
├── Program.cs                   # App configuration
├── appsettings.json            # Settings
└── README.md                   # This file
```

---

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server (for Part 2)
- Visual Studio 2022 or VS Code (optional)
- Git

### Part 1: Run with In-Memory Storage

1. **Clone the repository:**
```bash
git clone https://github.com/Sultanal-habs/projectPart1.git
cd projectPart1
```

2. **Restore packages:**
```bash
dotnet restore
```

3. **Run the application:**
```bash
dotnet run
```

4. **Open browser:**
```
https://localhost:5001
```

### Part 2: Setup SQL Server Database

1. **Install SQL Server** (if not already installed)
   - Download from: https://www.microsoft.com/sql-server

2. **Run database setup:**
   - Open SQL Server Management Studio (SSMS)
   - Open file: `Database/COMPLETE_SETUP.sql`
   - Press F5 to execute

3. **Verify installation:**
```sql
-- Check all tables have data
SELECT 'Users' AS [Table], COUNT(*) FROM Users
UNION ALL SELECT 'Artists', COUNT(*) FROM Artists
UNION ALL SELECT 'Artworks', COUNT(*) FROM Artworks;
```

4. **Update connection string** (for Part 3):
   - Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ArtGalleryDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**📖 Detailed Instructions:** See `/Database/QUICKSTART.md`

---

## 🗄️ Database Schema (Part 2)

### Tables Overview

| Table | Purpose | Records |
|-------|---------|---------|
| **Users** | Authentication & user profiles | 10 |
| **Artists** | Artist information & portfolios | 10 |
| **Artworks** | Artwork catalog | 20 |
| **Exhibitions** | Exhibition management | 8 |
| **ExhibitionArtworks** | Many-to-many relationships | 15 |
| **ArtworkLikes** | User engagement tracking | 30 |

### Database Views

1. `vw_ArtistPortfolio` - Artist statistics and performance
2. `vw_ExhibitionDetails` - Exhibition metrics
3. `vw_ArtworkGallery` - Complete artwork catalog
4. `vw_ActiveExhibitions` - Available exhibitions
5. `vw_PopularArtworks` - Trending content
6. `vw_UserDashboard` - User activity summary

**📊 Full Schema:** See `/Database/ER_DIAGRAM.md`

---

## 📚 Documentation

### Database (Part 2)
- **[README.md](Database/README.md)** - Complete database documentation
- **[QUICKSTART.md](Database/QUICKSTART.md)** - Quick installation guide
- **[ER_DIAGRAM.md](Database/ER_DIAGRAM.md)** - Entity-Relationship diagram
- **[SUMMARY.md](Database/SUMMARY.md)** - Implementation summary

### Scripts
- **COMPLETE_SETUP.sql** - All-in-one installation (recommended)
- **01-04 Scripts** - Modular installation files

---

## 🧪 Sample Data

The database includes realistic Omani context data:

### Users
- 10 users with different roles (Admin, Artist, User, Moderator)
- Authentic Omani names

### Artists
- 10 professional artist profiles
- Specializations: Painting, Photography, Crafts, Sculpture, Digital Art

### Artworks
- 20 diverse artworks
- Prices in OMR (Omani Rials)
- Various types and statuses

### Exhibitions
- 8 exhibitions (past, current, upcoming)
- Omani cultural venues
- Different themes and capacities

---

## 🔍 Sample Queries

### Browse Active Artists
```sql
SELECT * FROM vw_ArtistPortfolio 
WHERE StatusName = 'Active' 
ORDER BY TotalLikes DESC;
```

### Find Available Exhibitions
```sql
SELECT * FROM vw_ActiveExhibitions 
WHERE AvailableSlots > 0;
```

### Popular Artworks
```sql
SELECT TOP 10 * FROM vw_PopularArtworks;
```

---

## 📊 Project Milestones

### ✅ Part 1 - In-Memory Implementation
- [x] C# Models (Artist, Artwork, Exhibition)
- [x] Services layer (DataStore, Business logic)
- [x] Razor Pages UI
- [x] Image upload functionality
- [x] Search and filtering
- [x] Like system

### ✅ Part 2 - Database Implementation (CURRENT)
- [x] SQL Server database creation
- [x] 6 normalized tables
- [x] Primary and foreign keys
- [x] Check, default, unique constraints
- [x] 6 database views
- [x] 93 sample records
- [x] Complete documentation

### 🔄 Part 3 - Entity Framework Integration (UPCOMING)
- [ ] Install EF Core packages
- [ ] Create DbContext
- [ ] Replace DataStore with EF Core
- [ ] Implement migrations
- [ ] LINQ query optimization
- [ ] Repository pattern

---

## 🎓 Learning Outcomes

This project demonstrates:
- ✅ ASP.NET Core Razor Pages development
- ✅ Dependency Injection pattern
- ✅ SQL Server database design
- ✅ Database normalization (3NF)
- ✅ Referential integrity with foreign keys
- ✅ Complex SQL views and queries
- ✅ Data validation and constraints
- ✅ File upload handling
- ✅ Git version control
- ✅ Technical documentation

---

## 🤝 Contributing

This is a university project, but suggestions are welcome:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/NewFeature`)
3. Commit changes (`git commit -m 'Add NewFeature'`)
4. Push to branch (`git push origin feature/NewFeature`)
5. Open a Pull Request

---

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## 📞 Contact & Support

- **GitHub:** [Sultanal-habs/projectPart1](https://github.com/Sultanal-habs/projectPart1)
- **Authors:** Sultan & Abdulla
- **Course:** Web Application Development
- **Institution:** [Your University Name]

---

## 🙏 Acknowledgments

- Omani cultural context and themes
- ASP.NET Core documentation
- SQL Server best practices
- Bootstrap UI framework
- Open-source community

---

**Last Updated:** January 2024  
**Status:** Part 2 Complete ✅ | Part 3 In Progress 🔄

