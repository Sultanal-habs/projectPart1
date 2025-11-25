# Database Documentation - Art Gallery Management System
**Project:** projectPart1  
**Authors:** Sultan & Abdulla  
**Database Name:** ArtGalleryDB  
**DBMS:** Microsoft SQL Server

---

## Table of Contents
1. [Database Overview](#database-overview)
2. [Entity Relationship Diagram](#entity-relationship-diagram)
3. [Database Tables](#database-tables)
4. [Constraints](#constraints)
5. [Database Views](#database-views)
6. [Sample Queries](#sample-queries)
7. [Installation Instructions](#installation-instructions)

---

## Database Overview

### Purpose
This database supports a comprehensive art gallery management system for managing artists, artworks, exhibitions, and user interactions in an Omani cultural context.

### Key Features
- **User Authentication**: Secure login with role-based access
- **Artist Management**: Complete artist profiles and portfolios
- **Artwork Catalog**: Comprehensive artwork information with media support
- **Exhibition System**: Event management with artwork associations
- **Engagement Tracking**: Like system and user activity monitoring

### Statistics
- **Total Tables**: 6
- **Total Views**: 6
- **Total Constraints**: 40+
- **Sample Records**: 100+

---

## Entity Relationship Diagram

```
┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│   Users     │         │   Artists    │         │  Artworks   │
│─────────────│         │──────────────│         │─────────────│
│ UserId (PK) │◄───────┤│ ArtistId (PK)│◄───────┤│ArtworkId(PK)│
│ Username    │   1:1   │ UserId (FK)  │   1:M   │ ArtistId(FK)│
│ PasswordHash│         │ Name         │         │ Title       │
│ Email       │         │ Email        │         │ Description │
│ UserRole    │         │ Status       │         │ Type        │
└─────────────┘         └──────────────┘         │ Likes       │
       │                                          │ Price       │
       │                                          └─────────────┘
       │  1:M                                            │
       │                                                 │ M:M
       ▼                                                 ▼
┌─────────────┐                              ┌──────────────────┐
│ Exhibitions │                              │ExhibitionArtworks│
│─────────────│                              │──────────────────│
│ExhibitionId │◄────────────────────────────┤│ExhibitionId (PK) │
│    (PK)     │              1:M             │ ArtworkId (PK)   │
│ Name        │                              │ AddedDate        │
│ StartDate   │                              │ IsFeatured       │
│ EndDate     │                              └──────────────────┘
│ MaxArtworks │
└─────────────┘
       ▲
       │ 1:M
       │
┌─────────────┐
│ArtworkLikes │
│─────────────│
│ LikeId (PK) │
│ ArtworkId   │──────────────────────────────────────┐
│ UserId (FK) │                                      │
│ LikedDate   │                                      │
└─────────────┘                                      │
```

---

## Database Tables

### 1. Users Table
**Purpose:** Store user authentication and profile information

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| UserId | INT | PK, IDENTITY | Unique user identifier |
| Username | NVARCHAR(50) | NOT NULL, UNIQUE | Login username |
| PasswordHash | NVARCHAR(256) | NOT NULL | Hashed password |
| Email | NVARCHAR(256) | NOT NULL, UNIQUE | User email address |
| PhoneNumber | NVARCHAR(20) | NULL | Omani phone number |
| FullName | NVARCHAR(200) | NOT NULL | User's full name |
| UserRole | NVARCHAR(20) | NOT NULL, DEFAULT 'User' | Admin/Artist/User/Moderator |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Account status |
| CreatedDate | DATETIME2 | NOT NULL, DEFAULT SYSDATETIME() | Registration date |
| LastLoginDate | DATETIME2 | NULL | Last login timestamp |

**Constraints:**
- `CK_Users_UserRole`: Role must be 'Admin', 'Artist', 'User', or 'Moderator'
- `CK_Users_Email`: Email format validation
- `CK_Users_PhoneNumber`: Omani phone format (+968XXXXXXXX)

**Indexes:**
- `IX_Users_Username` (NONCLUSTERED)
- `IX_Users_Email` (NONCLUSTERED)

---

### 2. Artists Table
**Purpose:** Store artist profile and portfolio information

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ArtistId | INT | PK, IDENTITY | Unique artist identifier |
| Name | NVARCHAR(100) | NOT NULL | Artist name (2-100 chars) |
| Email | NVARCHAR(256) | NOT NULL, UNIQUE | Contact email |
| Phone | NVARCHAR(20) | NULL | Phone number |
| Bio | NVARCHAR(500) | NULL | Artist biography (max 500) |
| ProfileImageUrl | NVARCHAR(512) | NULL | Profile picture path |
| Status | TINYINT | NOT NULL, DEFAULT 0 | 0=Active, 1=Inactive, 2=Suspended, 3=Pending |
| JoinedDate | DATETIME2 | NOT NULL, DEFAULT SYSDATETIME() | Registration date |
| UserId | INT | FK → Users(UserId), NULL | Linked user account |

**Constraints:**
- `FK_Artists_Users`: References Users.UserId ON DELETE SET NULL
- `CK_Artists_Status`: Status must be 0, 1, 2, or 3
- `CK_Artists_Email`: Email format validation
- `CK_Artists_Phone`: Starts with '+968'
- `CK_Artists_NameLength`: 2-100 characters
- `CK_Artists_BioLength`: Max 500 characters

**Indexes:**
- `IX_Artists_Status` (NONCLUSTERED)
- `IX_Artists_JoinedDate` (NONCLUSTERED, DESC)
- `IX_Artists_UserId` (NONCLUSTERED)

---

### 3. Exhibitions Table
**Purpose:** Manage art exhibitions and events

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ExhibitionId | INT | PK, IDENTITY | Unique exhibition identifier |
| Name | NVARCHAR(200) | NOT NULL | Exhibition name (3-200 chars) |
| Description | NVARCHAR(1000) | NOT NULL | Description (10-1000 chars) |
| StartDate | DATE | NOT NULL | Exhibition start date |
| EndDate | DATE | NOT NULL | Exhibition end date |
| Location | NVARCHAR(200) | NULL | Venue location |
| MaxArtworks | INT | NOT NULL, DEFAULT 50 | Maximum artwork capacity (1-1000) |
| Status | TINYINT | NOT NULL, DEFAULT 0 | 0=Upcoming, 1=Active, 2=Ended, 3=Cancelled |
| BannerImageUrl | NVARCHAR(512) | NULL | Exhibition banner image |
| CreatedDate | DATETIME2 | NOT NULL, DEFAULT SYSDATETIME() | Creation timestamp |
| CreatedByUserId | INT | FK → Users(UserId), NULL | Creator user ID |

**Constraints:**
- `FK_Exhibitions_Users`: References Users.UserId ON DELETE SET NULL
- `CK_Exhibitions_Status`: Status must be 0, 1, 2, or 3
- `CK_Exhibitions_Dates`: EndDate > StartDate
- `CK_Exhibitions_MaxArtworks`: Between 1 and 1000
- `CK_Exhibitions_NameLength`: 3-200 characters
- `CK_Exhibitions_DescriptionLength`: 10-1000 characters

**Indexes:**
- `IX_Exhibitions_StartDate` (NONCLUSTERED)
- `IX_Exhibitions_Status` (NONCLUSTERED)
- `IX_Exhibitions_DateRange` (NONCLUSTERED, composite)

---

### 4. Artworks Table
**Purpose:** Catalog of all artworks in the system

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ArtworkId | INT | PK, IDENTITY | Unique artwork identifier |
| Title | NVARCHAR(150) | NOT NULL | Artwork title (3-150 chars) |
| Description | NVARCHAR(1000) | NOT NULL | Description (10-1000 chars) |
| ImageUrl | NVARCHAR(512) | NULL | Artwork image path |
| ArtworkType | TINYINT | NOT NULL | 0=Painting, 1=Photography, 2=Craft, 3=Sculpture, 4=Digital |
| Likes | INT | NOT NULL, DEFAULT 0 | Like count (>= 0) |
| CreatedDate | DATETIME2 | NOT NULL, DEFAULT SYSDATETIME() | Creation date |
| Price | DECIMAL(10,2) | NULL | Price in OMR (0-999999) |
| IsForSale | BIT | NOT NULL, DEFAULT 0 | Sale availability flag |
| Status | TINYINT | NOT NULL, DEFAULT 0 | 0=Active, 1=Pending, 2=Sold, 3=Archived |
| ArtistId | INT | FK → Artists(ArtistId), NOT NULL | Artist reference |
| ExhibitionId | INT | FK → Exhibitions(ExhibitionId), NULL | Current exhibition |

**Constraints:**
- `FK_Artworks_Artists`: References Artists.ArtistId ON DELETE CASCADE
- `FK_Artworks_Exhibitions`: References Exhibitions.ExhibitionId ON DELETE SET NULL
- `CK_Artworks_ArtworkType`: Type must be 0, 1, 2, 3, or 4
- `CK_Artworks_Status`: Status must be 0, 1, 2, or 3
- `CK_Artworks_Likes`: Likes >= 0
- `CK_Artworks_Price`: NULL or 0-999999
- `CK_Artworks_TitleLength`: 3-150 characters
- `CK_Artworks_DescriptionLength`: 10-1000 characters

**Indexes:**
- `IX_Artworks_ArtistId` (NONCLUSTERED)
- `IX_Artworks_ExhibitionId` (NONCLUSTERED)
- `IX_Artworks_Status_Type` (NONCLUSTERED, composite)
- `IX_Artworks_CreatedDate` (NONCLUSTERED, DESC)
- `IX_Artworks_IsForSale` (NONCLUSTERED, filtered WHERE IsForSale = 1)

---

### 5. ExhibitionArtworks Table (Junction)
**Purpose:** Many-to-many relationship between Exhibitions and Artworks

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| ExhibitionId | INT | PK, FK → Exhibitions | Exhibition reference |
| ArtworkId | INT | PK, FK → Artworks | Artwork reference |
| AddedDate | DATETIME2 | NOT NULL, DEFAULT SYSDATETIME() | Date added to exhibition |
| DisplayOrder | INT | NULL | Display sequence number |
| IsFeatured | BIT | NOT NULL, DEFAULT 0 | Featured artwork flag |

**Constraints:**
- `PK_ExhibitionArtworks`: Composite primary key (ExhibitionId, ArtworkId)
- `FK_ExhibitionArtworks_Exhibitions`: ON DELETE CASCADE
- `FK_ExhibitionArtworks_Artworks`: ON DELETE CASCADE
- `CK_ExhibitionArtworks_DisplayOrder`: >= 0 if not NULL

**Indexes:**
- `IX_ExhibitionArtworks_ArtworkId` (NONCLUSTERED)

---

### 6. ArtworkLikes Table
**Purpose:** Track individual user likes on artworks

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| LikeId | INT | PK, IDENTITY | Unique like identifier |
| ArtworkId | INT | FK → Artworks, NOT NULL | Liked artwork |
| UserId | INT | FK → Users, NULL | User who liked (NULL = anonymous) |
| LikedDate | DATETIME2 | NOT NULL, DEFAULT SYSDATETIME() | Timestamp of like |
| IpAddress | NVARCHAR(45) | NULL | IP address for tracking |

**Constraints:**
- `FK_ArtworkLikes_Artworks`: ON DELETE CASCADE
- `FK_ArtworkLikes_Users`: ON DELETE CASCADE
- `UQ_ArtworkLikes_UserArtwork`: Unique (UserId, ArtworkId) WHERE UserId IS NOT NULL

**Indexes:**
- `IX_ArtworkLikes_ArtworkId` (NONCLUSTERED)

---

## Constraints Summary

### Primary Keys (6)
| Table | Primary Key |
|-------|-------------|
| Users | UserId |
| Artists | ArtistId |
| Exhibitions | ExhibitionId |
| Artworks | ArtworkId |
| ExhibitionArtworks | (ExhibitionId, ArtworkId) |
| ArtworkLikes | LikeId |

### Foreign Keys (8)
| Child Table | Foreign Key | Parent Table | Parent Column | On Delete |
|-------------|-------------|--------------|---------------|-----------|
| Artists | UserId | Users | UserId | SET NULL |
| Exhibitions | CreatedByUserId | Users | UserId | SET NULL |
| Artworks | ArtistId | Artists | ArtistId | CASCADE |
| Artworks | ExhibitionId | Exhibitions | ExhibitionId | SET NULL |
| ExhibitionArtworks | ExhibitionId | Exhibitions | ExhibitionId | CASCADE |
| ExhibitionArtworks | ArtworkId | Artworks | ArtworkId | CASCADE |
| ArtworkLikes | ArtworkId | Artworks | ArtworkId | CASCADE |
| ArtworkLikes | UserId | Users | UserId | CASCADE |

### Unique Constraints (4)
- Users.Username
- Users.Email
- Artists.Email
- ArtworkLikes.(UserId, ArtworkId) [filtered]

### Check Constraints (21)
- **Users**: UserRole, Email format, Phone format
- **Artists**: Status, Email format, Phone format, Name length, Bio length
- **Exhibitions**: Status, Dates order, MaxArtworks range, Name length, Description length
- **Artworks**: ArtworkType, Status, Likes >= 0, Price range, Title length, Description length
- **ExhibitionArtworks**: DisplayOrder >= 0

### Default Constraints (14)
- Users: UserRole='User', IsActive=1, CreatedDate=SYSDATETIME()
- Artists: Status=0, JoinedDate=SYSDATETIME()
- Exhibitions: MaxArtworks=50, Status=0, CreatedDate=SYSDATETIME()
- Artworks: Likes=0, IsForSale=0, Status=0, CreatedDate=SYSDATETIME()
- ExhibitionArtworks: AddedDate=SYSDATETIME(), IsFeatured=0
- ArtworkLikes: LikedDate=SYSDATETIME()

---

## Database Views

### 1. vw_ArtistPortfolio
**Purpose:** Comprehensive artist statistics and performance metrics

**Columns:**
- Artist basic info (ID, Name, Email, Phone, Bio, Profile Image)
- Status and dates
- Linked user information
- **Statistics:**
  - Total artworks count
  - Total likes received
  - Average likes per artwork
  - Artworks for sale count
  - Sold artworks count
  - Total revenue from sales
  - Latest artwork date
  - Exhibitions participated count

**Usage Example:**
```sql
SELECT * FROM vw_ArtistPortfolio 
WHERE StatusName = 'Active' 
ORDER BY TotalLikes DESC;
```

---

### 2. vw_ExhibitionDetails
**Purpose:** Exhibition information with real-time metrics

**Columns:**
- Exhibition basic info
- Status name (Upcoming/Active/Ended/Cancelled)
- Creator username
- **Metrics:**
  - Current artwork count
  - Remaining slots
  - Fill percentage
  - Days remaining
  - Total duration
  - Featured artwork count
  - Participating artists count
  - Total exhibition likes

**Usage Example:**
```sql
SELECT * FROM vw_ExhibitionDetails 
WHERE StatusName IN ('Active', 'Upcoming') 
AND RemainingSlots > 0;
```

---

### 3. vw_ArtworkGallery
**Purpose:** Complete artwork catalog with related information

**Columns:**
- Artwork details (Title, Description, Image, Type, Likes, Price)
- Artist information (Name, Email, Phone, Profile Image)
- Exhibition information (Name, Location, Dates)
- **Calculated Fields:**
  - IsNew (created in last 7 days)
  - DisplayPrice (formatted with "OMR" or "Not for sale")
  - PopularityLevel (Very Popular/Popular/Trending/New)

**Usage Example:**
```sql
SELECT * FROM vw_ArtworkGallery 
WHERE StatusName = 'Active' 
AND IsNew = 1 
ORDER BY Likes DESC;
```

---

### 4. vw_ActiveExhibitions
**Purpose:** Current and upcoming exhibitions with availability

**Columns:**
- Exhibition details
- Current artworks count
- Available slots
- Current status (Upcoming/Active)
- Days until status change

**Filter:** Only shows exhibitions that are active/upcoming AND have available slots

**Usage Example:**
```sql
SELECT * FROM vw_ActiveExhibitions 
WHERE AvailableSlots >= 5 
ORDER BY StartDate;
```

---

### 5. vw_PopularArtworks
**Purpose:** Top 100 artworks by engagement

**Columns:**
- Artwork basic info
- Artist name and image
- Artwork type name
- **Engagement Metrics:**
  - Days since created
  - Likes per day ratio
  - Is in active exhibition flag

**Limit:** TOP 100 ordered by Likes DESC

**Usage Example:**
```sql
SELECT TOP 10 * FROM vw_PopularArtworks;
```

---

### 6. vw_UserDashboard
**Purpose:** Personalized user activity and statistics

**Columns:**
- User profile information
- Artist profile (if user is an artist)
- **Activity Metrics:**
  - Total artworks liked
  - My artworks count (if artist)
  - Total likes received (if artist)

**Usage Example:**
```sql
SELECT * FROM vw_UserDashboard 
WHERE UserId = @CurrentUserId;
```

---

## Sample Queries

### Get Active Artists with Top Artworks
```sql
SELECT 
    a.Name AS ArtistName,
    COUNT(aw.ArtworkId) AS TotalArtworks,
    SUM(aw.Likes) AS TotalLikes,
    MAX(aw.Price) AS HighestPrice
FROM Artists a
INNER JOIN Artworks aw ON a.ArtistId = aw.ArtistId
WHERE a.Status = 0 AND aw.Status = 0
GROUP BY a.ArtistId, a.Name
HAVING COUNT(aw.ArtworkId) > 0
ORDER BY TotalLikes DESC;
```

### Find Exhibitions with Most Engagement
```sql
SELECT 
    e.Name AS ExhibitionName,
    e.StartDate,
    e.EndDate,
    COUNT(DISTINCT ea.ArtworkId) AS ArtworkCount,
    SUM(aw.Likes) AS TotalLikes,
    COUNT(DISTINCT aw.ArtistId) AS ArtistCount
FROM Exhibitions e
INNER JOIN ExhibitionArtworks ea ON e.ExhibitionId = ea.ExhibitionId
INNER JOIN Artworks aw ON ea.ArtworkId = aw.ArtworkId
WHERE e.Status = 1 -- Active
GROUP BY e.ExhibitionId, e.Name, e.StartDate, e.EndDate
ORDER BY TotalLikes DESC;
```

### Get User's Liked Artworks
```sql
SELECT 
    aw.Title,
    aw.ImageUrl,
    a.Name AS ArtistName,
    aw.Likes,
    al.LikedDate
FROM ArtworkLikes al
INNER JOIN Artworks aw ON al.ArtworkId = aw.ArtworkId
INNER JOIN Artists a ON aw.ArtistId = a.ArtistId
WHERE al.UserId = @UserId
ORDER BY al.LikedDate DESC;
```

### Search Artworks by Type and Price Range
```sql
SELECT * FROM vw_ArtworkGallery
WHERE ArtworkTypeName = 'Painting'
AND IsForSale = 1
AND Price BETWEEN 1000 AND 5000
AND StatusName = 'Active'
ORDER BY Likes DESC;
```

---

## Installation Instructions

### Step 1: Create Database
Run the script in order:
```sql
-- File: 01_CreateDatabase.sql
```

### Step 2: Create Tables
```sql
-- File: 02_CreateTables.sql
```

### Step 3: Create Views
```sql
-- File: 03_CreateViews.sql
```

### Step 4: Insert Sample Data
```sql
-- File: 04_InsertData.sql
```

### Step 5: Verify Installation
```sql
-- Check table counts
SELECT 
    'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL
SELECT 'Artists', COUNT(*) FROM Artists
UNION ALL
SELECT 'Exhibitions', COUNT(*) FROM Exhibitions
UNION ALL
SELECT 'Artworks', COUNT(*) FROM Artworks
UNION ALL
SELECT 'ExhibitionArtworks', COUNT(*) FROM ExhibitionArtworks
UNION ALL
SELECT 'ArtworkLikes', COUNT(*) FROM ArtworkLikes;

-- Test a view
SELECT TOP 5 * FROM vw_ArtistPortfolio;
```

---

## Connection String Example

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ArtGalleryDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

## Notes
- All tables use `IDENTITY` for auto-incrementing primary keys
- Dates are stored as `DATETIME2` for better precision
- Enum values are stored as `TINYINT` (0-255 range)
- Cascading deletes are used where appropriate to maintain referential integrity
- SET NULL is used for non-critical relationships to preserve historical data
- All text fields use `NVARCHAR` for Unicode support (Arabic text)
- Phone numbers follow Omani format: `+968XXXXXXXX`

---

**End of Documentation**
