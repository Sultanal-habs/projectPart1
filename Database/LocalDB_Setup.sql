-- ============================================
-- LocalDB Setup Script for Art Gallery
-- Run this in Visual Studio or SSMS connected to (localdb)\mssqllocaldb
-- ============================================

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'ArtGalleryDB')
BEGIN
    ALTER DATABASE ArtGalleryDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ArtGalleryDB;
END
GO

-- Create database
CREATE DATABASE ArtGalleryDB;
GO

USE ArtGalleryDB;
GO

PRINT 'Database ArtGalleryDB created successfully!';
GO

-- ============================================
-- Create Tables
-- ============================================

-- 1. Users Table
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
    CONSTRAINT CK_Users_Email CHECK (Email LIKE '%_@__%.__%')
);
GO

-- 2. Artists Table
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
    CONSTRAINT CK_Artists_Status CHECK (Status IN (0, 1, 2, 3))
);
GO

-- 3. Exhibitions Table
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
    CONSTRAINT CK_Exhibitions_Dates CHECK (EndDate > StartDate)
);
GO

-- 4. Artworks Table
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
    CONSTRAINT CK_Artworks_Likes CHECK (Likes >= 0)
);
GO

-- 5. ExhibitionArtworks Table
CREATE TABLE ExhibitionArtworks (
    ExhibitionId INT NOT NULL,
    ArtworkId INT NOT NULL,
    AddedDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    DisplayOrder INT NULL,
    IsFeatured BIT NOT NULL DEFAULT 0,
    
    CONSTRAINT PK_ExhibitionArtworks PRIMARY KEY CLUSTERED (ExhibitionId ASC, ArtworkId ASC),
    CONSTRAINT FK_ExhibitionArtworks_Exhibitions FOREIGN KEY (ExhibitionId) REFERENCES Exhibitions(ExhibitionId) ON DELETE CASCADE,
    CONSTRAINT FK_ExhibitionArtworks_Artworks FOREIGN KEY (ArtworkId) REFERENCES Artworks(ArtworkId) ON DELETE CASCADE
);
GO

-- 6. ArtworkLikes Table
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
GO

PRINT 'All tables created successfully!';
GO

-- ============================================
-- Insert Sample Data
-- ============================================

-- Insert Users with simple passwords for testing
INSERT INTO Users (Username, PasswordHash, Email, PhoneNumber, FullName, UserRole, IsActive, CreatedDate)
VALUES
    ('admin', 'admin', 'admin@artgallery.om', '+96892345678', 'Admin User', 'Admin', 1, GETDATE()),
    ('sultan', 'password123', 'sultan@example.com', '+96891234567', 'Sultan Al-Habsi', 'Artist', 1, GETDATE()),
    ('abdulla', 'password123', 'abdulla@example.com', '+96898765432', 'Abdulla Al-Saidi', 'Artist', 1, GETDATE());
GO

-- Insert Artists
INSERT INTO Artists (Name, Email, Phone, Bio, ProfileImageUrl, Status, JoinedDate, UserId)
VALUES
    ('Mohammed Al Harthi', 'mohammed@example.com', '+96891234567', 'Traditional Omani artist specializing in desert landscapes', '/images/artists/mohammed.jpg', 0, GETDATE(), 2),
    ('Fatima Al Saidi', 'fatima@example.com', '+96892345678', 'Contemporary photographer', '/images/artists/fatima.jpg', 0, GETDATE(), 3),
    ('Ahmed Al Balushi', 'ahmed@example.com', '+96893456789', 'Master craftsman', '/images/artists/ahmed.jpg', 0, GETDATE(), NULL);
GO

-- Insert Exhibitions
INSERT INTO Exhibitions (Name, Description, StartDate, EndDate, Location, MaxArtworks, Status, BannerImageUrl, CreatedDate, CreatedByUserId)
VALUES
    ('Contemporary Omani Art', 'Showcase of modern and traditional Omani art', DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, 20, GETDATE()), 'Muscat Art Gallery', 50, 1, '/images/exhibitions/contemporary.jpg', GETDATE(), 1),
    ('Photography Week', 'Annual photography exhibition', DATEADD(DAY, 15, GETDATE()), DATEADD(DAY, 22, GETDATE()), 'Royal Opera House', 75, 0, '/images/exhibitions/photo.jpg', GETDATE(), 1);
GO

-- Insert Artworks
INSERT INTO Artworks (Title, Description, ImageUrl, ArtworkType, Likes, CreatedDate, Price, IsForSale, Status, ArtistId, ExhibitionId)
VALUES
    ('Desert Dawn', 'Oil painting of Wahiba Sands at sunrise', '/images/painting1.jpg', 0, 45, DATEADD(DAY, -30, GETDATE()), 250.00, 1, 0, 1, 1),
    ('Ghost Pier', 'Monochrome photograph of abandoned pier', '/images/photograph1.jpg', 1, 67, DATEADD(DAY, -15, GETDATE()), 180.00, 1, 0, 2, 1),
    ('Woven Warmth', 'Handcrafted rattan baskets', '/images/handmadecrafts1.jpg', 2, 89, DATEADD(DAY, -7, GETDATE()), 500.00, 1, 0, 3, NULL),
    ('Chromatic Storm', 'Abstract painting with vibrant colors', '/images/painting2.jpg', 0, 73, DATEADD(DAY, -5, GETDATE()), 350.00, 1, 0, 1, NULL),
    ('Whisker Whisper', 'Embroidery of cat face', '/images/handmadecrafts2.jpg', 2, 41, DATEADD(DAY, -12, GETDATE()), 120.00, 1, 0, 3, NULL);
GO

PRINT '============================================';
PRINT 'Database setup completed successfully!';
PRINT 'You can now login with:';
PRINT '  Username: admin';
PRINT '  Password: admin';
PRINT '============================================';
GO
