-- ============================================
-- COMPLETE DATABASE SETUP - SINGLE FILE
-- Art Gallery Management System
-- Author: Sultan & Abdulla
-- ============================================
-- Run this entire script to create the complete database
-- Estimated execution time: 10-30 seconds
-- ============================================

SET NOCOUNT ON;
GO

PRINT '============================================';
PRINT 'Art Gallery Database - Complete Installation';
PRINT 'Started at: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '============================================';
PRINT '';
GO

-- ============================================
-- PART 1: CREATE DATABASE
-- ============================================
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'ArtGalleryDB')
BEGIN
    PRINT 'Dropping existing database...';
    ALTER DATABASE ArtGalleryDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ArtGalleryDB;
END
GO

PRINT 'Creating database ArtGalleryDB...';
CREATE DATABASE ArtGalleryDB;
GO

PRINT 'Database created successfully!';
PRINT '';
GO

USE ArtGalleryDB;
GO

-- ============================================
-- PART 2: CREATE TABLES
-- ============================================
PRINT 'Creating tables...';
GO

-- Table 1: Users
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
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId ASC),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT CK_Users_UserRole CHECK (UserRole IN ('Admin', 'Artist', 'User', 'Moderator')),
    CONSTRAINT CK_Users_Email CHECK (Email LIKE '%_@__%.__%'),
    CONSTRAINT CK_Users_PhoneNumber CHECK (PhoneNumber IS NULL OR PhoneNumber LIKE '+968[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
);
CREATE NONCLUSTERED INDEX IX_Users_Username ON Users(Username);
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
PRINT '  ✓ Users';
GO

-- Table 2: Artists
CREATE TABLE Artists (
    ArtistId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Phone NVARCHAR(20) NULL,
    Bio NVARCHAR(500) NULL,
    ProfileImageUrl NVARCHAR(512) NULL,
    Status TINYINT NOT NULL DEFAULT 0,
    JoinedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    UserId INT NULL,
    CONSTRAINT PK_Artists PRIMARY KEY CLUSTERED (ArtistId ASC),
    CONSTRAINT UQ_Artists_Email UNIQUE (Email),
    CONSTRAINT FK_Artists_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL,
    CONSTRAINT CK_Artists_Status CHECK (Status IN (0, 1, 2, 3)),
    CONSTRAINT CK_Artists_Email CHECK (Email LIKE '%_@__%.__%'),
    CONSTRAINT CK_Artists_Phone CHECK (Phone IS NULL OR Phone LIKE '+968%'),
    CONSTRAINT CK_Artists_NameLength CHECK (LEN(Name) >= 2 AND LEN(Name) <= 100),
    CONSTRAINT CK_Artists_BioLength CHECK (Bio IS NULL OR LEN(Bio) <= 500)
);
CREATE NONCLUSTERED INDEX IX_Artists_Status ON Artists(Status);
CREATE NONCLUSTERED INDEX IX_Artists_JoinedDate ON Artists(JoinedDate DESC);
CREATE NONCLUSTERED INDEX IX_Artists_UserId ON Artists(UserId);
PRINT '  ✓ Artists';
GO

-- Table 3: Exhibitions
CREATE TABLE Exhibitions (
    ExhibitionId INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Location NVARCHAR(200) NULL,
    MaxArtworks INT NOT NULL DEFAULT 50,
    Status TINYINT NOT NULL DEFAULT 0,
    BannerImageUrl NVARCHAR(512) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CreatedByUserId INT NULL,
    CONSTRAINT PK_Exhibitions PRIMARY KEY CLUSTERED (ExhibitionId ASC),
    CONSTRAINT FK_Exhibitions_Users FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId) ON DELETE SET NULL,
    CONSTRAINT CK_Exhibitions_Status CHECK (Status IN (0, 1, 2, 3)),
    CONSTRAINT CK_Exhibitions_Dates CHECK (EndDate > StartDate),
    CONSTRAINT CK_Exhibitions_MaxArtworks CHECK (MaxArtworks > 0 AND MaxArtworks <= 1000),
    CONSTRAINT CK_Exhibitions_NameLength CHECK (LEN(Name) >= 3 AND LEN(Name) <= 200),
    CONSTRAINT CK_Exhibitions_DescriptionLength CHECK (LEN(Description) >= 10 AND LEN(Description) <= 1000)
);
CREATE NONCLUSTERED INDEX IX_Exhibitions_StartDate ON Exhibitions(StartDate);
CREATE NONCLUSTERED INDEX IX_Exhibitions_Status ON Exhibitions(Status);
CREATE NONCLUSTERED INDEX IX_Exhibitions_DateRange ON Exhibitions(StartDate, EndDate);
PRINT '  ✓ Exhibitions';
GO

-- Table 4: Artworks
CREATE TABLE Artworks (
    ArtworkId INT IDENTITY(1,1) NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    ImageUrl NVARCHAR(512) NULL,
    ArtworkType TINYINT NOT NULL,
    Likes INT NOT NULL DEFAULT 0,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    Price DECIMAL(10,2) NULL,
    IsForSale BIT NOT NULL DEFAULT 0,
    Status TINYINT NOT NULL DEFAULT 0,
    ArtistId INT NOT NULL,
    ExhibitionId INT NULL,
    CONSTRAINT PK_Artworks PRIMARY KEY CLUSTERED (ArtworkId ASC),
    CONSTRAINT FK_Artworks_Artists FOREIGN KEY (ArtistId) REFERENCES Artists(ArtistId) ON DELETE CASCADE,
    CONSTRAINT FK_Artworks_Exhibitions FOREIGN KEY (ExhibitionId) REFERENCES Exhibitions(ExhibitionId) ON DELETE SET NULL,
    CONSTRAINT CK_Artworks_ArtworkType CHECK (ArtworkType IN (0, 1, 2, 3, 4)),
    CONSTRAINT CK_Artworks_Status CHECK (Status IN (0, 1, 2, 3)),
    CONSTRAINT CK_Artworks_Likes CHECK (Likes >= 0),
    CONSTRAINT CK_Artworks_Price CHECK (Price IS NULL OR (Price >= 0 AND Price <= 999999)),
    CONSTRAINT CK_Artworks_TitleLength CHECK (LEN(Title) >= 3 AND LEN(Title) <= 150),
    CONSTRAINT CK_Artworks_DescriptionLength CHECK (LEN(Description) >= 10 AND LEN(Description) <= 1000)
);
CREATE NONCLUSTERED INDEX IX_Artworks_ArtistId ON Artworks(ArtistId);
CREATE NONCLUSTERED INDEX IX_Artworks_ExhibitionId ON Artworks(ExhibitionId);
CREATE NONCLUSTERED INDEX IX_Artworks_Status_Type ON Artworks(Status, ArtworkType);
CREATE NONCLUSTERED INDEX IX_Artworks_CreatedDate ON Artworks(CreatedDate DESC);
CREATE NONCLUSTERED INDEX IX_Artworks_IsForSale ON Artworks(IsForSale) WHERE IsForSale = 1;
PRINT '  ✓ Artworks';
GO

-- Table 5: ExhibitionArtworks
CREATE TABLE ExhibitionArtworks (
    ExhibitionId INT NOT NULL,
    ArtworkId INT NOT NULL,
    AddedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    DisplayOrder INT NULL,
    IsFeatured BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ExhibitionArtworks PRIMARY KEY CLUSTERED (ExhibitionId ASC, ArtworkId ASC),
    CONSTRAINT FK_ExhibitionArtworks_Exhibitions FOREIGN KEY (ExhibitionId) REFERENCES Exhibitions(ExhibitionId) ON DELETE CASCADE,
    CONSTRAINT FK_ExhibitionArtworks_Artworks FOREIGN KEY (ArtworkId) REFERENCES Artworks(ArtworkId) ON DELETE CASCADE,
    CONSTRAINT CK_ExhibitionArtworks_DisplayOrder CHECK (DisplayOrder IS NULL OR DisplayOrder >= 0)
);
CREATE NONCLUSTERED INDEX IX_ExhibitionArtworks_ArtworkId ON ExhibitionArtworks(ArtworkId);
PRINT '  ✓ ExhibitionArtworks';
GO

-- Table 6: ArtworkLikes
CREATE TABLE ArtworkLikes (
    LikeId INT IDENTITY(1,1) NOT NULL,
    ArtworkId INT NOT NULL,
    UserId INT NULL,
    LikedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    IpAddress NVARCHAR(45) NULL,
    CONSTRAINT PK_ArtworkLikes PRIMARY KEY CLUSTERED (LikeId ASC),
    CONSTRAINT FK_ArtworkLikes_Artworks FOREIGN KEY (ArtworkId) REFERENCES Artworks(ArtworkId) ON DELETE CASCADE,
    CONSTRAINT FK_ArtworkLikes_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);
CREATE UNIQUE NONCLUSTERED INDEX UQ_ArtworkLikes_UserArtwork ON ArtworkLikes(UserId, ArtworkId) WHERE UserId IS NOT NULL;
CREATE NONCLUSTERED INDEX IX_ArtworkLikes_ArtworkId ON ArtworkLikes(ArtworkId);
PRINT '  ✓ ArtworkLikes';
GO

PRINT 'All tables created successfully!';
PRINT '';
GO

-- ============================================
-- PART 3: CREATE VIEWS
-- ============================================
PRINT 'Creating views...';
GO

-- View 1: vw_ArtistPortfolio
CREATE OR ALTER VIEW vw_ArtistPortfolio
AS
SELECT 
    a.ArtistId, a.Name AS ArtistName, a.Email, a.Phone, a.Bio, a.ProfileImageUrl,
    CASE a.Status WHEN 0 THEN 'Active' WHEN 1 THEN 'Inactive' WHEN 2 THEN 'Suspended' WHEN 3 THEN 'Pending Approval' END AS StatusName,
    a.JoinedDate, u.Username AS LinkedUsername, u.UserRole,
    COUNT(DISTINCT aw.ArtworkId) AS TotalArtworks,
    ISNULL(SUM(aw.Likes), 0) AS TotalLikes,
    ISNULL(AVG(CAST(aw.Likes AS DECIMAL(10,2))), 0) AS AverageLikesPerArtwork,
    SUM(CASE WHEN aw.IsForSale = 1 THEN 1 ELSE 0 END) AS ArtworksForSale,
    SUM(CASE WHEN aw.Status = 2 THEN 1 ELSE 0 END) AS SoldArtworks,
    ISNULL(SUM(CASE WHEN aw.Status = 2 THEN aw.Price ELSE 0 END), 0) AS TotalRevenue,
    MAX(aw.CreatedDate) AS LatestArtworkDate,
    COUNT(DISTINCT ea.ExhibitionId) AS ExhibitionsParticipated
FROM Artists a
LEFT JOIN Users u ON a.UserId = u.UserId
LEFT JOIN Artworks aw ON a.ArtistId = aw.ArtistId
LEFT JOIN ExhibitionArtworks ea ON aw.ArtworkId = ea.ArtworkId
GROUP BY a.ArtistId, a.Name, a.Email, a.Phone, a.Bio, a.ProfileImageUrl, a.Status, a.JoinedDate, u.Username, u.UserRole;
GO
PRINT '  ✓ vw_ArtistPortfolio';

-- View 2: vw_ExhibitionDetails (shorter version for space)
CREATE OR ALTER VIEW vw_ExhibitionDetails
AS
SELECT 
    e.ExhibitionId, e.Name AS ExhibitionName, e.Description, e.StartDate, e.EndDate, e.Location, e.MaxArtworks,
    CASE e.Status WHEN 0 THEN 'Upcoming' WHEN 1 THEN 'Active' WHEN 2 THEN 'Ended' WHEN 3 THEN 'Cancelled' END AS StatusName,
    e.BannerImageUrl, e.CreatedDate, u.Username AS CreatedBy,
    COUNT(DISTINCT ea.ArtworkId) AS CurrentArtworkCount,
    e.MaxArtworks - COUNT(DISTINCT ea.ArtworkId) AS RemainingSlots,
    CAST(ROUND(CAST(COUNT(DISTINCT ea.ArtworkId) AS FLOAT) / e.MaxArtworks * 100, 2) AS DECIMAL(5,2)) AS FillPercentage,
    SUM(CASE WHEN ea.IsFeatured = 1 THEN 1 ELSE 0 END) AS FeaturedArtworkCount,
    COUNT(DISTINCT aw.ArtistId) AS ParticipatingArtists,
    ISNULL(SUM(aw.Likes), 0) AS TotalExhibitionLikes
FROM Exhibitions e
LEFT JOIN Users u ON e.CreatedByUserId = u.UserId
LEFT JOIN ExhibitionArtworks ea ON e.ExhibitionId = ea.ExhibitionId
LEFT JOIN Artworks aw ON ea.ArtworkId = aw.ArtworkId
GROUP BY e.ExhibitionId, e.Name, e.Description, e.StartDate, e.EndDate, e.Location, e.MaxArtworks, e.Status, e.BannerImageUrl, e.CreatedDate, u.Username;
GO
PRINT '  ✓ vw_ExhibitionDetails';

PRINT 'All views created successfully!';
PRINT '';
GO

-- ============================================
-- PART 4: INSERT SAMPLE DATA
-- ============================================
PRINT 'Inserting sample data...';
GO

-- Insert Users (10 records)
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (UserId, Username, PasswordHash, Email, PhoneNumber, FullName, UserRole, IsActive, CreatedDate, LastLoginDate)
VALUES
(1, 'admin', 'hash_admin', 'admin@artgallery.om', '+96892345678', 'System Administrator', 'Admin', 1, '2023-01-15', '2024-01-20'),
(2, 'sultan_artist', 'hash_sultan', 'sultan@example.com', '+96891234567', 'Sultan Al-Habsi', 'Artist', 1, '2023-06-10', '2024-01-19'),
(3, 'abdulla_artist', 'hash_abdulla', 'abdulla@example.com', '+96898765432', 'Abdulla Al-Saidi', 'Artist', 1, '2023-07-20', '2024-01-18'),
(4, 'fatima_user', 'hash_fatima', 'fatima.user@example.com', '+96893456789', 'Fatima Al-Harthi', 'User', 1, '2023-08-05', '2024-01-17'),
(5, 'mohammed_mod', 'hash_mohammed', 'mohammed@artgallery.om', '+96894567890', 'Mohammed Al-Balushi', 'Moderator', 1, '2023-03-12', '2024-01-19'),
(6, 'sara_artist', 'hash_sara', 'sara@example.com', '+96895678901', 'Sara Al-Rashdi', 'Artist', 1, '2023-09-01', '2024-01-16'),
(7, 'khalid_user', 'hash_khalid', 'khalid@example.com', '+96896789012', 'Khalid Al-Kharusi', 'User', 1, '2023-10-10', '2024-01-15'),
(8, 'aisha_artist', 'hash_aisha', 'aisha@example.com', '+96897890123', 'Aisha Al-Lawati', 'Artist', 1, '2023-11-15', '2024-01-14'),
(9, 'omar_user', 'hash_omar', 'omar@example.com', '+96898901234', 'Omar Al-Hinai', 'User', 1, '2023-12-01', '2024-01-13'),
(10, 'layla_artist', 'hash_layla', 'layla@example.com', '+96899012345', 'Layla Al-Mahrouqi', 'Artist', 1, '2024-01-05', '2024-01-12');
SET IDENTITY_INSERT Users OFF;
PRINT '  ✓ 10 Users inserted';
GO

-- Insert Artists (10 records)
SET IDENTITY_INSERT Artists ON;
INSERT INTO Artists (ArtistId, Name, Email, Phone, Bio, ProfileImageUrl, Status, JoinedDate, UserId)
VALUES
(1, 'Mohammed Al Harthi', 'mohammed@example.com', '+96891234567', 'Traditional Omani artist specializing in desert landscapes.', '/images/artists/mohammed.jpg', 0, '2023-06-01', 2),
(2, 'Fatima Al Saidi', 'fatima@example.com', '+96892345678', 'Contemporary photographer capturing modern Omani life.', '/images/artists/fatima.jpg', 0, '2023-07-15', 3),
(3, 'Ahmed Al Balushi', 'ahmed@example.com', '+96893456789', 'Master craftsman known for traditional silverwork.', '/images/artists/ahmed.jpg', 0, '2022-01-10', NULL),
(4, 'Sara Al Rashdi', 'sara@example.com', '+96895678901', 'Digital artist blending Islamic patterns with technology.', '/images/artists/sara.jpg', 0, '2023-09-01', 6),
(5, 'Aisha Al Lawati', 'aisha@example.com', '+96897890123', 'Sculpture artist working with local materials.', '/images/artists/aisha.jpg', 0, '2023-11-15', 8),
(6, 'Khalid Al Habsi', 'khalid.artist@example.com', '+96894567890', 'Oil painter focusing on portraits.', '/images/artists/khalid.jpg', 0, '2023-05-20', NULL),
(7, 'Layla Al Mahrouqi', 'layla@example.com', '+96899012345', 'Mixed media artist exploring identity themes.', '/images/artists/layla.jpg', 0, '2024-01-05', 10),
(8, 'Youssef Al Amri', 'youssef@example.com', '+96890123456', 'Calligraphy expert in Arabic art.', '/images/artists/youssef.jpg', 0, '2022-08-12', NULL),
(9, 'Mariam Al Wahaibi', 'mariam@example.com', '+96891012345', 'Watercolor artist depicting Omani nature.', '/images/artists/mariam.jpg', 3, '2024-01-18', NULL),
(10, 'Hassan Al Kharusi', 'hassan@example.com', '+96892013456', 'Photography documenting Omani traditions.', '/images/artists/hassan.jpg', 1, '2021-03-15', NULL);
SET IDENTITY_INSERT Artists OFF;
PRINT '  ✓ 10 Artists inserted';
GO

-- Insert Exhibitions (8 records)
SET IDENTITY_INSERT Exhibitions ON;
INSERT INTO Exhibitions (ExhibitionId, Name, Description, StartDate, EndDate, Location, MaxArtworks, Status, BannerImageUrl, CreatedDate, CreatedByUserId)
VALUES
(1, 'Heritage & Modernity', 'Celebration of Omani culture blending tradition with contemporary art.', '2024-02-01', '2024-03-31', 'Royal Opera House Muscat', 50, 1, '/images/exhibitions/heritage.jpg', '2023-12-01', 1),
(2, 'Desert Dreams', 'Landscapes inspired by Omani deserts.', '2024-03-15', '2024-05-15', 'Bait Al Zubair Museum', 30, 0, '/images/exhibitions/desert.jpg', '2024-01-10', 5),
(3, 'Urban Lens', 'Photographic journey of Muscat transformation.', '2024-01-10', '2024-02-28', 'National Museum', 40, 1, '/images/exhibitions/urban.jpg', '2023-11-15', 1),
(4, 'Crafted Traditions', 'Traditional Omani handicrafts showcase.', '2024-04-01', '2024-06-30', 'Mutrah Souq Gallery', 35, 0, '/images/exhibitions/crafted.jpg', '2024-01-05', 5),
(5, 'Digital Horizons', 'Contemporary digital art and NFT showcase.', '2023-11-01', '2024-01-15', 'Cultural Complex', 25, 2, '/images/exhibitions/digital.jpg', '2023-09-20', 1),
(6, 'Women in Art', 'Celebrating female Omani artists.', '2024-05-01', '2024-07-31', 'Al Amerat Center', 45, 0, '/images/exhibitions/women.jpg', '2024-01-12', 5),
(7, 'Calligraphy Masters', 'Classical and modern Arabic calligraphy.', '2023-12-01', '2024-02-15', 'Grand Mosque Gallery', 20, 1, '/images/exhibitions/calligraphy.jpg', '2023-10-10', 1),
(8, 'Marine Wonders', 'Underwater photography celebrating marine life.', '2024-06-01', '2024-08-31', 'Marine Science Center', 30, 0, '/images/exhibitions/marine.jpg', '2024-01-15', 1);
SET IDENTITY_INSERT Exhibitions OFF;
PRINT '  ✓ 8 Exhibitions inserted';
GO

-- Insert Artworks (20 records - abbreviated for space)
SET IDENTITY_INSERT Artworks ON;
INSERT INTO Artworks (ArtworkId, Title, Description, ImageUrl, ArtworkType, Likes, CreatedDate, Price, IsForSale, Status, ArtistId, ExhibitionId)
VALUES
(1, 'Desert at Dawn', 'Oil painting of Wahiba Sands at golden hour.', '/images/artworks/desert_dawn.jpg', 0, 145, '2023-10-15', 2500.00, 1, 0, 1, 1),
(2, 'Frankincense Trail', 'Historical trade routes through Dhofar.', '/images/artworks/frankincense.jpg', 0, 98, '2023-11-20', 3200.00, 1, 0, 1, 1),
(3, 'Omani Fort Sunset', 'Traditional fort against sunset sky.', '/images/artworks/fort_sunset.jpg', 0, 210, '2024-01-05', 2800.00, 1, 0, 1, 3),
(4, 'Muttrah Corniche Nights', 'Long exposure of illuminated corniche.', '/images/artworks/muttrah.jpg', 1, 187, '2023-09-10', 1500.00, 1, 0, 2, 3),
(5, 'Souq Stories', 'Black and white portrait series.', '/images/artworks/souq.jpg', 1, 156, '2023-12-01', 1800.00, 1, 0, 2, 3),
(6, 'Royal Khanjar Set', 'Exquisitely crafted silver khanjar.', '/images/artworks/khanjar.jpg', 2, 223, '2023-08-20', 5500.00, 1, 0, 3, 4),
(7, 'Omani Jewelry', 'Traditional silver jewelry collection.', '/images/artworks/jewelry.jpg', 2, 178, '2023-10-05', 3200.00, 1, 0, 3, 4),
(8, 'Geometric Dreams', 'Digital art with Islamic patterns.', '/images/artworks/geometric.jpg', 4, 267, '2023-11-01', 1200.00, 1, 0, 4, 5),
(9, 'NFT: Digital Khanjar', 'Limited edition NFT artwork.', '/images/artworks/nft_khanjar.jpg', 4, 312, '2023-10-20', 2500.00, 0, 0, 4, NULL),
(10, 'Stone Whispers', 'Marble sculpture inspired by desert.', '/images/artworks/stone.jpg', 3, 156, '2023-11-25', 8500.00, 1, 0, 5, 1),
(11, 'Portrait of Tradition', 'Oil portrait of elderly Omani man.', '/images/artworks/portrait.jpg', 0, 167, '2023-09-15', 4200.00, 1, 0, 6, 1),
(12, 'Identity Layers', 'Mixed media exploring Omani identity.', '/images/artworks/identity.jpg', 0, 98, '2024-01-08', 2200.00, 1, 0, 7, 6),
(13, 'Quranic Verses', 'Masterful Thuluth calligraphy.', '/images/artworks/quran.jpg', 4, 234, '2023-12-05', 3500.00, 1, 0, 8, 7),
(14, 'Poetry in Motion', 'Contemporary Arabic calligraphy.', '/images/artworks/poetry.jpg', 4, 201, '2023-11-10', 2900.00, 1, 0, 8, 7),
(15, 'Wadi Paradise', 'Watercolor of lush Omani wadi.', '/images/artworks/wadi.jpg', 0, 112, '2024-01-12', 1800.00, 1, 1, 9, NULL),
(16, 'Arabian Oryx', 'Watercolor of national animal.', '/images/artworks/oryx.jpg', 0, 145, '2024-01-18', 1500.00, 1, 1, 9, NULL),
(17, 'Wedding Traditions', 'Documentary photo series.', '/images/artworks/wedding.jpg', 1, 178, '2023-07-20', NULL, 0, 3, 10, NULL),
(18, 'Coastal Morning', 'Beach scene with dhow boats.', '/images/artworks/coastal.jpg', 0, 89, '2024-01-10', 2100.00, 1, 0, 1, NULL),
(19, 'Mountain Majesty', 'Panoramic view of Jebel Shams.', '/images/artworks/mountain.jpg', 1, 123, '2023-12-20', 2200.00, 1, 0, 2, 3),
(20, 'Digital Souk', 'VR experience of traditional marketplace.', '/images/artworks/digital_souk.jpg', 4, 245, '2023-11-15', 3500.00, 1, 2, 4, 5);
SET IDENTITY_INSERT Artworks OFF;
PRINT '  ✓ 20 Artworks inserted';
GO

-- Insert ExhibitionArtworks (15 records)
INSERT INTO ExhibitionArtworks (ExhibitionId, ArtworkId, AddedDate, DisplayOrder, IsFeatured)
VALUES
(1, 1, '2024-01-15', 1, 1), (1, 2, '2024-01-15', 2, 0), (1, 3, '2024-01-16', 3, 1), (1, 10, '2024-01-17', 5, 1), (1, 11, '2024-01-17', 6, 0),
(3, 4, '2024-01-05', 1, 1), (3, 5, '2024-01-05', 2, 1), (3, 19, '2024-01-07', 4, 0),
(4, 6, '2024-01-20', 1, 1), (4, 7, '2024-01-20', 2, 1),
(5, 8, '2023-10-25', 1, 1), (5, 20, '2023-10-26', 3, 0),
(6, 12, '2024-01-18', 1, 1),
(7, 13, '2023-11-20', 1, 1), (7, 14, '2023-11-20', 2, 1);
PRINT '  ✓ 15 ExhibitionArtworks relationships created';
GO

-- Insert ArtworkLikes (30 sample records)
SET IDENTITY_INSERT ArtworkLikes ON;
INSERT INTO ArtworkLikes (LikeId, ArtworkId, UserId, LikedDate, IpAddress)
VALUES
(1, 9, 1, '2023-10-21', '192.168.1.1'), (2, 9, 2, '2023-10-21', '192.168.1.2'), (3, 9, 3, '2023-10-22', '192.168.1.3'),
(4, 8, 1, '2023-11-02', '192.168.1.1'), (5, 8, 3, '2023-11-02', '192.168.1.3'), (6, 8, 4, '2023-11-03', '192.168.1.4'),
(7, 20, 2, '2023-11-16', '192.168.1.2'), (8, 20, 3, '2023-11-16', '192.168.1.3'), (9, 20, 4, '2023-11-17', '192.168.1.4'),
(10, 13, 1, '2023-12-06', '192.168.1.1'), (11, 13, 2, '2023-12-06', '192.168.1.2'), (12, 13, 4, '2023-12-07', '192.168.1.4'),
(13, 6, 1, '2023-08-21', '192.168.1.1'), (14, 6, 3, '2023-08-21', '192.168.1.3'), (15, 6, 4, '2023-08-22', '192.168.1.4'),
(16, 3, 2, '2024-01-06', '192.168.1.2'), (17, 3, 3, '2024-01-06', '192.168.1.3'), (18, 3, 4, '2024-01-07', '192.168.1.4'),
(19, 1, 3, '2023-10-16', '192.168.1.3'), (20, 1, 4, '2023-10-17', '192.168.1.4'),
(21, 4, 1, '2023-09-11', '192.168.1.1'), (22, 4, 6, '2023-09-12', '192.168.1.6'),
(23, 5, 2, '2023-12-02', '192.168.1.2'), (24, 5, 7, '2023-12-03', '192.168.1.7'),
(25, 10, 4, '2023-11-26', '192.168.1.4'), (26, 10, 9, '2023-11-27', '192.168.1.9'),
(27, 11, 2, '2023-09-16', '192.168.1.2'), (28, 11, 6, '2023-09-17', '192.168.1.6'),
(29, 14, 4, '2023-11-11', '192.168.1.4'), (30, 14, 9, '2023-11-12', '192.168.1.9');
SET IDENTITY_INSERT ArtworkLikes OFF;
PRINT '  ✓ 30 ArtworkLikes inserted';
GO

PRINT 'Sample data population completed!';
PRINT '';
GO

-- ============================================
-- VERIFICATION & SUMMARY
-- ============================================
PRINT '============================================';
PRINT 'INSTALLATION COMPLETED SUCCESSFULLY!';
PRINT '============================================';
PRINT '';
PRINT 'Database: ArtGalleryDB';
PRINT 'Status: Ready for use';
PRINT '';
PRINT 'Record Summary:';
PRINT '---------------';

SELECT 'Users' AS [Table], COUNT(*) AS [Records] FROM Users
UNION ALL SELECT 'Artists', COUNT(*) FROM Artists
UNION ALL SELECT 'Exhibitions', COUNT(*) FROM Exhibitions
UNION ALL SELECT 'Artworks', COUNT(*) FROM Artworks
UNION ALL SELECT 'ExhibitionArtworks', COUNT(*) FROM ExhibitionArtworks
UNION ALL SELECT 'ArtworkLikes', COUNT(*) FROM ArtworkLikes;

PRINT '';
PRINT 'Sample Queries:';
PRINT '---------------';
PRINT 'SELECT * FROM vw_ArtistPortfolio;';
PRINT 'SELECT * FROM vw_ExhibitionDetails;';
PRINT '';
PRINT 'Completed at: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '============================================';
GO
