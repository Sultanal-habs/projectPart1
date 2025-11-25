-- ============================================
-- Script: Create Tables with Constraints
-- Author: Sultan & Abdulla
-- Date: 2024
-- Description: Creates all tables with PRIMARY KEY, FOREIGN KEY, CHECK, DEFAULT, and NOT NULL constraints
-- ============================================

USE ArtGalleryDB;
GO

-- ============================================
-- Table 1: Users (Login Information)
-- Purpose: Store user authentication and profile data
-- ============================================
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    FullName NVARCHAR(200) NOT NULL,
    UserRole NVARCHAR(20) NOT NULL DEFAULT 'User',
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    LastLoginDate DATETIME2 NULL,
    
    -- Primary Key
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId ASC),
    
    -- Unique Constraints
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    
    -- Check Constraints
    CONSTRAINT CK_Users_UserRole CHECK (UserRole IN ('Admin', 'Artist', 'User', 'Moderator')),
    CONSTRAINT CK_Users_Email CHECK (Email LIKE '%_@__%.__%'),
    CONSTRAINT CK_Users_PhoneNumber CHECK (PhoneNumber IS NULL OR PhoneNumber LIKE '+968[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
);
GO

-- Index for faster login queries
CREATE NONCLUSTERED INDEX IX_Users_Username ON Users(Username);
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
GO

PRINT 'Table Users created successfully!';
GO

-- ============================================
-- Table 2: Artists
-- Purpose: Store artist profile information
-- ============================================
CREATE TABLE Artists (
    ArtistId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Phone NVARCHAR(20) NULL,
    Bio NVARCHAR(500) NULL,
    ProfileImageUrl NVARCHAR(512) NULL,
    Status TINYINT NOT NULL DEFAULT 0, -- 0=Active, 1=Inactive, 2=Suspended, 3=PendingApproval
    JoinedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    UserId INT NULL, -- Foreign key to Users table
    
    -- Primary Key
    CONSTRAINT PK_Artists PRIMARY KEY CLUSTERED (ArtistId ASC),
    
    -- Unique Constraint
    CONSTRAINT UQ_Artists_Email UNIQUE (Email),
    
    -- Foreign Key
    CONSTRAINT FK_Artists_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL,
    
    -- Check Constraints
    CONSTRAINT CK_Artists_Status CHECK (Status IN (0, 1, 2, 3)),
    CONSTRAINT CK_Artists_Email CHECK (Email LIKE '%_@__%.__%'),
    CONSTRAINT CK_Artists_Phone CHECK (Phone IS NULL OR Phone LIKE '+968%'),
    CONSTRAINT CK_Artists_NameLength CHECK (LEN(Name) >= 2 AND LEN(Name) <= 100),
    CONSTRAINT CK_Artists_BioLength CHECK (Bio IS NULL OR LEN(Bio) <= 500)
);
GO

-- Indexes
CREATE NONCLUSTERED INDEX IX_Artists_Status ON Artists(Status);
CREATE NONCLUSTERED INDEX IX_Artists_JoinedDate ON Artists(JoinedDate DESC);
CREATE NONCLUSTERED INDEX IX_Artists_UserId ON Artists(UserId);
GO

PRINT 'Table Artists created successfully!';
GO

-- ============================================
-- Table 3: Exhibitions
-- Purpose: Store exhibition information
-- ============================================
CREATE TABLE Exhibitions (
    ExhibitionId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Location NVARCHAR(200) NULL,
    MaxArtworks INT NOT NULL DEFAULT 50,
    Status TINYINT NOT NULL DEFAULT 0, -- 0=Upcoming, 1=Active, 2=Ended, 3=Cancelled
    BannerImageUrl NVARCHAR(512) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CreatedByUserId INT NULL,
    
    -- Primary Key
    CONSTRAINT PK_Exhibitions PRIMARY KEY CLUSTERED (ExhibitionId ASC),
    
    -- Foreign Key
    CONSTRAINT FK_Exhibitions_Users FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId) ON DELETE SET NULL,
    
    -- Check Constraints
    CONSTRAINT CK_Exhibitions_Status CHECK (Status IN (0, 1, 2, 3)),
    CONSTRAINT CK_Exhibitions_Dates CHECK (EndDate > StartDate),
    CONSTRAINT CK_Exhibitions_MaxArtworks CHECK (MaxArtworks > 0 AND MaxArtworks <= 1000),
    CONSTRAINT CK_Exhibitions_NameLength CHECK (LEN(Name) >= 3 AND LEN(Name) <= 200),
    CONSTRAINT CK_Exhibitions_DescriptionLength CHECK (LEN(Description) >= 10 AND LEN(Description) <= 1000)
);
GO

-- Indexes
CREATE NONCLUSTERED INDEX IX_Exhibitions_StartDate ON Exhibitions(StartDate);
CREATE NONCLUSTERED INDEX IX_Exhibitions_Status ON Exhibitions(Status);
CREATE NONCLUSTERED INDEX IX_Exhibitions_DateRange ON Exhibitions(StartDate, EndDate);
GO

PRINT 'Table Exhibitions created successfully!';
GO

-- ============================================
-- Table 4: Artworks
-- Purpose: Store artwork information
-- ============================================
CREATE TABLE Artworks (
    ArtworkId INT IDENTITY(1,1) NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    ImageUrl NVARCHAR(512) NULL,
    ArtworkType TINYINT NOT NULL, -- 0=Painting, 1=Photography, 2=HandmadeCraft, 3=Sculpture, 4=DigitalArt
    Likes INT NOT NULL DEFAULT 0,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    Price DECIMAL(10,2) NULL,
    IsForSale BIT NOT NULL DEFAULT 0,
    Status TINYINT NOT NULL DEFAULT 0, -- 0=Active, 1=Pending, 2=Sold, 3=Archived
    ArtistId INT NOT NULL,
    ExhibitionId INT NULL,
    
    -- Primary Key
    CONSTRAINT PK_Artworks PRIMARY KEY CLUSTERED (ArtworkId ASC),
    
    -- Foreign Keys
    CONSTRAINT FK_Artworks_Artists FOREIGN KEY (ArtistId) REFERENCES Artists(ArtistId) ON DELETE CASCADE,
    CONSTRAINT FK_Artworks_Exhibitions FOREIGN KEY (ExhibitionId) REFERENCES Exhibitions(ExhibitionId) ON DELETE SET NULL,
    
    -- Check Constraints
    CONSTRAINT CK_Artworks_ArtworkType CHECK (ArtworkType IN (0, 1, 2, 3, 4)),
    CONSTRAINT CK_Artworks_Status CHECK (Status IN (0, 1, 2, 3)),
    CONSTRAINT CK_Artworks_Likes CHECK (Likes >= 0),
    CONSTRAINT CK_Artworks_Price CHECK (Price IS NULL OR (Price >= 0 AND Price <= 999999)),
    CONSTRAINT CK_Artworks_TitleLength CHECK (LEN(Title) >= 3 AND LEN(Title) <= 150),
    CONSTRAINT CK_Artworks_DescriptionLength CHECK (LEN(Description) >= 10 AND LEN(Description) <= 1000)
);
GO

-- Indexes
CREATE NONCLUSTERED INDEX IX_Artworks_ArtistId ON Artworks(ArtistId);
CREATE NONCLUSTERED INDEX IX_Artworks_ExhibitionId ON Artworks(ExhibitionId);
CREATE NONCLUSTERED INDEX IX_Artworks_Status_Type ON Artworks(Status, ArtworkType);
CREATE NONCLUSTERED INDEX IX_Artworks_CreatedDate ON Artworks(CreatedDate DESC);
CREATE NONCLUSTERED INDEX IX_Artworks_IsForSale ON Artworks(IsForSale) WHERE IsForSale = 1;
GO

PRINT 'Table Artworks created successfully!';
GO

-- ============================================
-- Table 5: ExhibitionArtworks (Junction Table)
-- Purpose: Many-to-many relationship between Exhibitions and Artworks
-- ============================================
CREATE TABLE ExhibitionArtworks (
    ExhibitionId INT NOT NULL,
    ArtworkId INT NOT NULL,
    AddedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    DisplayOrder INT NULL,
    IsFeatured BIT NOT NULL DEFAULT 0,
    
    -- Primary Key (Composite)
    CONSTRAINT PK_ExhibitionArtworks PRIMARY KEY CLUSTERED (ExhibitionId ASC, ArtworkId ASC),
    
    -- Foreign Keys
    CONSTRAINT FK_ExhibitionArtworks_Exhibitions FOREIGN KEY (ExhibitionId) REFERENCES Exhibitions(ExhibitionId) ON DELETE CASCADE,
    CONSTRAINT FK_ExhibitionArtworks_Artworks FOREIGN KEY (ArtworkId) REFERENCES Artworks(ArtworkId) ON DELETE CASCADE,
    
    -- Check Constraints
    CONSTRAINT CK_ExhibitionArtworks_DisplayOrder CHECK (DisplayOrder IS NULL OR DisplayOrder >= 0)
);
GO

CREATE NONCLUSTERED INDEX IX_ExhibitionArtworks_ArtworkId ON ExhibitionArtworks(ArtworkId);
GO

PRINT 'Table ExhibitionArtworks created successfully!';
GO

-- ============================================
-- Table 6: ArtworkLikes (Track individual likes)
-- Purpose: Track which users liked which artworks
-- ============================================
CREATE TABLE ArtworkLikes (
    LikeId INT IDENTITY(1,1) NOT NULL,
    ArtworkId INT NOT NULL,
    UserId INT NULL, -- NULL for anonymous likes
    LikedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    IpAddress NVARCHAR(45) NULL,
    
    -- Primary Key
    CONSTRAINT PK_ArtworkLikes PRIMARY KEY CLUSTERED (LikeId ASC),
    
    -- Foreign Keys
    CONSTRAINT FK_ArtworkLikes_Artworks FOREIGN KEY (ArtworkId) REFERENCES Artworks(ArtworkId) ON DELETE CASCADE,
    CONSTRAINT FK_ArtworkLikes_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);
GO

-- Prevent duplicate likes from same user
CREATE UNIQUE NONCLUSTERED INDEX UQ_ArtworkLikes_UserArtwork 
    ON ArtworkLikes(UserId, ArtworkId) 
    WHERE UserId IS NOT NULL;
GO

CREATE NONCLUSTERED INDEX IX_ArtworkLikes_ArtworkId ON ArtworkLikes(ArtworkId);
GO

PRINT 'Table ArtworkLikes created successfully!';
GO

-- ============================================
-- Summary
-- ============================================
PRINT '============================================';
PRINT 'All tables created successfully!';
PRINT 'Total tables: 6';
PRINT '  1. Users (Login & Authentication)';
PRINT '  2. Artists (Artist Profiles)';
PRINT '  3. Exhibitions (Exhibition Management)';
PRINT '  4. Artworks (Artwork Catalog)';
PRINT '  5. ExhibitionArtworks (Junction Table)';
PRINT '  6. ArtworkLikes (Like Tracking)';
PRINT '============================================';
GO
