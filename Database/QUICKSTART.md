# Quick Start Guide - Database Setup

## 🚀 How to Install the Database

### Option 1: Single File Installation (RECOMMENDED)
Open SQL Server Management Studio (SSMS) and run:
```sql
-- File: Database/COMPLETE_SETUP.sql
```
This single file creates everything: database, tables, views, and sample data.

### Option 2: Step-by-Step Installation
Run scripts in this order:
1. `01_CreateDatabase.sql` - Creates ArtGalleryDB
2. `02_CreateTables.sql` - Creates all 6 tables with constraints
3. `03_CreateViews.sql` - Creates 6 database views
4. `04_InsertData.sql` - Populates tables with sample data

---

## 📊 What Gets Created

### Tables (6)
1. **Users** - Login and authentication (10 records)
2. **Artists** - Artist profiles (10 records)
3. **Exhibitions** - Exhibition events (8 records)
4. **Artworks** - Artwork catalog (20 records)
5. **ExhibitionArtworks** - Many-to-many junction (15 records)
6. **ArtworkLikes** - Like tracking (30 records)

### Views (6)
1. `vw_ArtistPortfolio` - Artist statistics
2. `vw_ExhibitionDetails` - Exhibition metrics
3. `vw_ArtworkGallery` - Complete artwork catalog
4. `vw_ActiveExhibitions` - Available exhibitions
5. `vw_PopularArtworks` - Top 100 trending artworks
6. `vw_UserDashboard` - User activity summary

### Constraints (40+)
- 6 Primary Keys
- 8 Foreign Keys
- 4 Unique Constraints
- 21 Check Constraints
- 14 Default Constraints

---

## ✅ Verification Queries

After installation, run these to verify:

```sql
-- Check record counts
SELECT 'Users' AS [Table], COUNT(*) AS [Records] FROM Users
UNION ALL SELECT 'Artists', COUNT(*) FROM Artists
UNION ALL SELECT 'Exhibitions', COUNT(*) FROM Exhibitions
UNION ALL SELECT 'Artworks', COUNT(*) FROM Artworks;

-- Test views
SELECT TOP 5 * FROM vw_ArtistPortfolio;
SELECT TOP 5 * FROM vw_ExhibitionDetails;
SELECT TOP 10 * FROM vw_PopularArtworks;
```

Expected results:
- Users: 10 records
- Artists: 10 records
- Exhibitions: 8 records
- Artworks: 20 records
- ExhibitionArtworks: 15 records
- ArtworkLikes: 30 records

---

## 🔗 Connection String for ASP.NET Core

Add to `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ArtGalleryDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Or with username/password:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ArtGalleryDB;User Id=your_username;Password=your_password;TrustServerCertificate=True;"
  }
}
```

---

## 📋 Requirements Checklist

✅ **[6 Points]** Three database tables (Artists, Artworks, Exhibitions)  
✅ **[2 Points]** User login table (Users with username, password, email, phone)  
✅ **[4 Points]** Primary keys (6) and Foreign keys (8) defined  
✅ **[3 Points]** CHECK, DEFAULT, NOT NULL constraints applied  
✅ **[3 Points]** Database views created (6 views)  
✅ **[2 Points]** Realistic sample data populated (93 total records)

**Total: 20/20 Points**

---

## 🎯 Sample Test Users

| Username | Password (Hash) | Role | Email |
|----------|-----------------|------|-------|
| admin | hash_admin | Admin | admin@artgallery.om |
| sultan_artist | hash_sultan | Artist | sultan@example.com |
| abdulla_artist | hash_abdulla | Artist | abdulla@example.com |
| fatima_user | hash_fatima | User | fatima.user@example.com |

*Note: Password hashes are placeholders. Implement real hashing in your application.*

---

## 📖 Documentation

For detailed documentation, see:
- **Database/README.md** - Complete database documentation
- **ER Diagram** - Included in README.md
- **Table Schemas** - Detailed column descriptions
- **Constraint Details** - All constraints explained

---

## 🔍 Useful Queries

### Get Active Artists with Artwork Count
```sql
SELECT * FROM vw_ArtistPortfolio 
WHERE StatusName = 'Active' 
ORDER BY TotalArtworks DESC;
```

### Find Exhibitions with Available Slots
```sql
SELECT * FROM vw_ActiveExhibitions 
WHERE AvailableSlots > 0 
ORDER BY StartDate;
```

### Browse Artworks by Type
```sql
SELECT * FROM vw_ArtworkGallery 
WHERE ArtworkTypeName = 'Painting' 
AND StatusName = 'Active' 
ORDER BY Likes DESC;
```

### Get Popular Artworks
```sql
SELECT TOP 10 * FROM vw_PopularArtworks;
```

---

## ⚠️ Troubleshooting

**Error: "Database already exists"**
- The script automatically drops and recreates the database
- Or manually drop: `DROP DATABASE ArtGalleryDB;`

**Error: "Foreign key constraint failed"**
- Make sure to run scripts in order
- Tables must exist before creating foreign keys

**Error: "Cannot insert NULL"**
- Check NOT NULL constraints in table definitions
- Required fields: Title, Name, Email, etc.

---

## 📞 Support

Created by: Sultan & Abdulla  
Project: projectPart1 - Art Gallery Management System  
GitHub: https://github.com/Sultanal-habs/projectPart1

For issues or questions, check the README.md or create an issue on GitHub.
