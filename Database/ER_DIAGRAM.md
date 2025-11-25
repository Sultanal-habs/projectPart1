# Entity Relationship Diagram
## Art Gallery Management System Database

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         ArtGalleryDB Schema                                  │
└─────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────┐
│      Users           │
│──────────────────────│
│ PK UserId            │────┐
│    Username (UQ)     │    │
│    PasswordHash      │    │
│    Email (UQ)        │    │
│    PhoneNumber       │    │
│    FullName          │    │
│    UserRole          │    │
│    IsActive          │    │
│    CreatedDate       │    │
│    LastLoginDate     │    │
└──────────────────────┘    │
        │                   │
        │ 1:1               │ 1:M
        │ (Optional)        │
        ▼                   ▼
┌──────────────────────┐  ┌──────────────────────┐
│     Artists          │  │    Exhibitions       │
│──────────────────────│  │──────────────────────│
│ PK ArtistId          │  │ PK ExhibitionId      │
│ FK UserId            │  │ FK CreatedByUserId   │
│    Name              │  │    Name              │
│    Email (UQ)        │  │    Description       │
│    Phone             │  │    StartDate         │
│    Bio               │  │    EndDate           │
│    ProfileImageUrl   │  │    Location          │
│    Status            │  │    MaxArtworks       │
│    JoinedDate        │  │    Status            │
└──────────────────────┘  │    BannerImageUrl    │
        │                  │    CreatedDate       │
        │ 1:M              └──────────────────────┘
        │                           │
        ▼                           │ 1:M
┌──────────────────────┐            │
│     Artworks         │            │
│──────────────────────│            │
│ PK ArtworkId         │            │
│ FK ArtistId          │◄───────────┘
│ FK ExhibitionId (N)  │
│    Title             │
│    Description       │
│    ImageUrl          │
│    ArtworkType       │
│    Likes             │
│    CreatedDate       │
│    Price             │
│    IsForSale         │
│    Status            │
└──────────────────────┘
        │  ▲
        │  │
        │  │ M:M (through ExhibitionArtworks)
        │  │
        ▼  │
┌──────────────────────────────────┐
│    ExhibitionArtworks            │
│  (Junction Table)                │
│──────────────────────────────────│
│ PK,FK ExhibitionId               │
│ PK,FK ArtworkId                  │
│       AddedDate                  │
│       DisplayOrder               │
│       IsFeatured                 │
└──────────────────────────────────┘

        ┌──────────────────────┐
        │   ArtworkLikes       │
        │──────────────────────│
        │ PK LikeId            │
        │ FK ArtworkId         │───┐
        │ FK UserId (Nullable) │   │
        │    LikedDate         │   │
        │    IpAddress         │   │
        └──────────────────────┘   │
                │                  │
                │ M:1              │ M:1
                └──────────────────┴──────────► (Points to Artworks & Users)


═══════════════════════════════════════════════════════════════════════════════
LEGEND:
═══════════════════════════════════════════════════════════════════════════════

PK  = Primary Key
FK  = Foreign Key
UQ  = Unique Constraint
(N) = Nullable Foreign Key

1:1  = One-to-One relationship
1:M  = One-to-Many relationship
M:M  = Many-to-Many relationship


═══════════════════════════════════════════════════════════════════════════════
RELATIONSHIPS SUMMARY:
═══════════════════════════════════════════════════════════════════════════════

1. Users → Artists (1:1 Optional)
   - A user can optionally have an artist profile
   - FK: Artists.UserId → Users.UserId
   - ON DELETE: SET NULL

2. Users → Exhibitions (1:M)
   - A user can create multiple exhibitions
   - FK: Exhibitions.CreatedByUserId → Users.UserId
   - ON DELETE: SET NULL

3. Artists → Artworks (1:M)
   - An artist can create multiple artworks
   - FK: Artworks.ArtistId → Artists.ArtistId
   - ON DELETE: CASCADE

4. Exhibitions → Artworks (1:M via ExhibitionId)
   - An exhibition can feature multiple artworks (direct FK)
   - FK: Artworks.ExhibitionId → Exhibitions.ExhibitionId
   - ON DELETE: SET NULL

5. Exhibitions ←→ Artworks (M:M via ExhibitionArtworks)
   - Many artworks can be in many exhibitions
   - Junction: ExhibitionArtworks
   - ON DELETE: CASCADE (both sides)

6. Users → ArtworkLikes (1:M)
   - A user can like multiple artworks
   - FK: ArtworkLikes.UserId → Users.UserId
   - ON DELETE: CASCADE

7. Artworks → ArtworkLikes (1:M)
   - An artwork can have multiple likes
   - FK: ArtworkLikes.ArtworkId → Artworks.ArtworkId
   - ON DELETE: CASCADE


═══════════════════════════════════════════════════════════════════════════════
ENUM VALUES:
═══════════════════════════════════════════════════════════════════════════════

UserRole (NVARCHAR):
  - Admin
  - Artist
  - User
  - Moderator

ArtistStatus (TINYINT):
  - 0: Active
  - 1: Inactive
  - 2: Suspended
  - 3: PendingApproval

ExhibitionStatus (TINYINT):
  - 0: Upcoming
  - 1: Active
  - 2: Ended
  - 3: Cancelled

ArtworkType (TINYINT):
  - 0: Painting
  - 1: Photography
  - 2: HandmadeCraft
  - 3: Sculpture
  - 4: DigitalArt

ArtworkStatus (TINYINT):
  - 0: Active
  - 1: Pending
  - 2: Sold
  - 3: Archived


═══════════════════════════════════════════════════════════════════════════════
INDEXES SUMMARY:
═══════════════════════════════════════════════════════════════════════════════

Users:
  - IX_Users_Username (NONCLUSTERED)
  - IX_Users_Email (NONCLUSTERED)

Artists:
  - IX_Artists_Status (NONCLUSTERED)
  - IX_Artists_JoinedDate (NONCLUSTERED, DESC)
  - IX_Artists_UserId (NONCLUSTERED)

Exhibitions:
  - IX_Exhibitions_StartDate (NONCLUSTERED)
  - IX_Exhibitions_Status (NONCLUSTERED)
  - IX_Exhibitions_DateRange (NONCLUSTERED, COMPOSITE)

Artworks:
  - IX_Artworks_ArtistId (NONCLUSTERED)
  - IX_Artworks_ExhibitionId (NONCLUSTERED)
  - IX_Artworks_Status_Type (NONCLUSTERED, COMPOSITE)
  - IX_Artworks_CreatedDate (NONCLUSTERED, DESC)
  - IX_Artworks_IsForSale (NONCLUSTERED, FILTERED)

ExhibitionArtworks:
  - IX_ExhibitionArtworks_ArtworkId (NONCLUSTERED)

ArtworkLikes:
  - UQ_ArtworkLikes_UserArtwork (UNIQUE, FILTERED)
  - IX_ArtworkLikes_ArtworkId (NONCLUSTERED)
```

---

## Visual Relationship Flow

```
User Registration
    ↓
User Login
    ↓
    ├─→ Browse Artworks (vw_ArtworkGallery)
    │       ↓
    │   Like Artwork (ArtworkLikes)
    │       ↓
    │   View Artist Profile (vw_ArtistPortfolio)
    │
    ├─→ Create Artist Profile (if role = Artist)
    │       ↓
    │   Upload Artwork
    │       ↓
    │   Submit to Exhibition (ExhibitionArtworks)
    │       ↓
    │   Track Likes & Sales
    │
    └─→ Manage Exhibitions (if role = Admin/Moderator)
            ↓
        Create Exhibition
            ↓
        Approve Artworks
            ↓
        Monitor Engagement (vw_ExhibitionDetails)
```

---

## Database Normalization

This database follows **Third Normal Form (3NF)**:

1. **First Normal Form (1NF)**: All attributes contain atomic values
2. **Second Normal Form (2NF)**: No partial dependencies (all non-key attributes depend on the entire primary key)
3. **Third Normal Form (3NF)**: No transitive dependencies

**Note**: Some denormalization exists in Part 1 models (e.g., ArtistName in Artworks) but this database design is fully normalized. When migrating from Part 1, these redundant fields should be removed and replaced with proper joins.

---

## File Structure

```
projectPart1/
└── Database/
    ├── 00_MasterInstall.sql       # Master installation script
    ├── 01_CreateDatabase.sql      # Database creation
    ├── 02_CreateTables.sql        # Table definitions
    ├── 03_CreateViews.sql         # View definitions
    ├── 04_InsertData.sql          # Sample data
    ├── COMPLETE_SETUP.sql         # All-in-one script
    ├── README.md                  # Full documentation
    ├── QUICKSTART.md              # Quick start guide
    └── ER_DIAGRAM.md              # This file
```
