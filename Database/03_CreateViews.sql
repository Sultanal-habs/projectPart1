-- ============================================
-- Script: Create Database Views
-- Author: Sultan & Abdulla
-- Date: 2024
-- Description: Creates views for complex queries and reporting
-- ============================================

USE ArtGalleryDB;
GO

-- ============================================
-- View 1: vw_ArtistPortfolio
-- Purpose: Comprehensive artist information with artwork statistics
-- Usage: Display artist profiles with their performance metrics
-- ============================================
CREATE OR ALTER VIEW vw_ArtistPortfolio
AS
SELECT 
    a.ArtistId,
    a.Name AS ArtistName,
    a.Email,
    a.Phone,
    a.Bio,
    a.ProfileImageUrl,
    CASE a.Status
        WHEN 0 THEN 'Active'
        WHEN 1 THEN 'Inactive'
        WHEN 2 THEN 'Suspended'
        WHEN 3 THEN 'Pending Approval'
    END AS StatusName,
    a.JoinedDate,
    u.Username AS LinkedUsername,
    u.UserRole,
    -- Artwork Statistics
    COUNT(DISTINCT aw.ArtworkId) AS TotalArtworks,
    ISNULL(SUM(aw.Likes), 0) AS TotalLikes,
    ISNULL(AVG(CAST(aw.Likes AS DECIMAL(10,2))), 0) AS AverageLikesPerArtwork,
    SUM(CASE WHEN aw.IsForSale = 1 THEN 1 ELSE 0 END) AS ArtworksForSale,
    SUM(CASE WHEN aw.Status = 2 THEN 1 ELSE 0 END) AS SoldArtworks, -- Status=2 is Sold
    ISNULL(SUM(CASE WHEN aw.Status = 2 THEN aw.Price ELSE 0 END), 0) AS TotalRevenue,
    -- Latest artwork date
    MAX(aw.CreatedDate) AS LatestArtworkDate,
    -- Exhibition participation
    COUNT(DISTINCT ea.ExhibitionId) AS ExhibitionsParticipated
FROM 
    Artists a
    LEFT JOIN Users u ON a.UserId = u.UserId
    LEFT JOIN Artworks aw ON a.ArtistId = aw.ArtistId
    LEFT JOIN ExhibitionArtworks ea ON aw.ArtworkId = ea.ArtworkId
GROUP BY 
    a.ArtistId, a.Name, a.Email, a.Phone, a.Bio, 
    a.ProfileImageUrl, a.Status, a.JoinedDate,
    u.Username, u.UserRole;
GO

PRINT 'View vw_ArtistPortfolio created successfully!';
GO

-- ============================================
-- View 2: vw_ExhibitionDetails
-- Purpose: Exhibition information with artwork count and featured pieces
-- Usage: Display exhibition listings and details
-- ============================================
CREATE OR ALTER VIEW vw_ExhibitionDetails
AS
SELECT 
    e.ExhibitionId,
    e.Name AS ExhibitionName,
    e.Description,
    e.StartDate,
    e.EndDate,
    e.Location,
    e.MaxArtworks,
    CASE e.Status
        WHEN 0 THEN 'Upcoming'
        WHEN 1 THEN 'Active'
        WHEN 2 THEN 'Ended'
        WHEN 3 THEN 'Cancelled'
    END AS StatusName,
    e.BannerImageUrl,
    e.CreatedDate,
    u.Username AS CreatedBy,
    -- Exhibition metrics
    COUNT(DISTINCT ea.ArtworkId) AS CurrentArtworkCount,
    e.MaxArtworks - COUNT(DISTINCT ea.ArtworkId) AS RemainingSlots,
    CAST(ROUND(CAST(COUNT(DISTINCT ea.ArtworkId) AS FLOAT) / e.MaxArtworks * 100, 2) AS DECIMAL(5,2)) AS FillPercentage,
    -- Date calculations
    CASE 
        WHEN GETDATE() < e.StartDate THEN DATEDIFF(DAY, GETDATE(), e.StartDate)
        WHEN GETDATE() BETWEEN e.StartDate AND e.EndDate THEN DATEDIFF(DAY, GETDATE(), e.EndDate)
        ELSE 0
    END AS DaysRemaining,
    DATEDIFF(DAY, e.StartDate, e.EndDate) AS TotalDuration,
    -- Featured artworks
    SUM(CASE WHEN ea.IsFeatured = 1 THEN 1 ELSE 0 END) AS FeaturedArtworkCount,
    -- Artist participation
    COUNT(DISTINCT aw.ArtistId) AS ParticipatingArtists,
    -- Total likes across all artworks
    ISNULL(SUM(aw.Likes), 0) AS TotalExhibitionLikes
FROM 
    Exhibitions e
    LEFT JOIN Users u ON e.CreatedByUserId = u.UserId
    LEFT JOIN ExhibitionArtworks ea ON e.ExhibitionId = ea.ExhibitionId
    LEFT JOIN Artworks aw ON ea.ArtworkId = aw.ArtworkId
GROUP BY 
    e.ExhibitionId, e.Name, e.Description, e.StartDate, e.EndDate,
    e.Location, e.MaxArtworks, e.Status, e.BannerImageUrl, 
    e.CreatedDate, u.Username;
GO

PRINT 'View vw_ExhibitionDetails created successfully!';
GO

-- ============================================
-- View 3: vw_ArtworkGallery
-- Purpose: Complete artwork information with artist and exhibition details
-- Usage: Main gallery view for browsing and searching artworks
-- ============================================
CREATE OR ALTER VIEW vw_ArtworkGallery
AS
SELECT 
    aw.ArtworkId,
    aw.Title,
    aw.Description,
    aw.ImageUrl,
    CASE aw.ArtworkType
        WHEN 0 THEN 'Painting'
        WHEN 1 THEN 'Photography'
        WHEN 2 THEN 'Handmade Craft'
        WHEN 3 THEN 'Sculpture'
        WHEN 4 THEN 'Digital Art'
    END AS ArtworkTypeName,
    aw.Likes,
    aw.CreatedDate,
    aw.Price,
    aw.IsForSale,
    CASE aw.Status
        WHEN 0 THEN 'Active'
        WHEN 1 THEN 'Pending'
        WHEN 2 THEN 'Sold'
        WHEN 3 THEN 'Archived'
    END AS StatusName,
    -- Artist information
    a.ArtistId,
    a.Name AS ArtistName,
    a.Email AS ArtistEmail,
    a.Phone AS ArtistPhone,
    a.ProfileImageUrl AS ArtistProfileImage,
    -- Exhibition information (current/latest)
    e.ExhibitionId,
    e.Name AS ExhibitionName,
    e.Location AS ExhibitionLocation,
    e.StartDate AS ExhibitionStartDate,
    e.EndDate AS ExhibitionEndDate,
    ea.IsFeatured,
    ea.DisplayOrder,
    -- Calculated fields
    CASE 
        WHEN DATEDIFF(DAY, aw.CreatedDate, GETDATE()) <= 7 THEN 1
        ELSE 0
    END AS IsNew,
    CASE 
        WHEN aw.IsForSale = 1 AND aw.Price IS NOT NULL THEN CONCAT(FORMAT(aw.Price, 'N2'), ' OMR')
        ELSE 'Not for sale'
    END AS DisplayPrice,
    -- Like count category
    CASE 
        WHEN aw.Likes >= 100 THEN 'Very Popular'
        WHEN aw.Likes >= 50 THEN 'Popular'
        WHEN aw.Likes >= 20 THEN 'Trending'
        ELSE 'New'
    END AS PopularityLevel
FROM 
    Artworks aw
    INNER JOIN Artists a ON aw.ArtistId = a.ArtistId
    LEFT JOIN ExhibitionArtworks ea ON aw.ArtworkId = ea.ArtworkId
    LEFT JOIN Exhibitions e ON ea.ExhibitionId = e.ExhibitionId;
GO

PRINT 'View vw_ArtworkGallery created successfully!';
GO

-- ============================================
-- View 4: vw_ActiveExhibitions
-- Purpose: Shows only active and upcoming exhibitions with availability
-- Usage: Display current exhibition opportunities for artists
-- ============================================
CREATE OR ALTER VIEW vw_ActiveExhibitions
AS
SELECT 
    e.ExhibitionId,
    e.Name,
    e.Description,
    e.StartDate,
    e.EndDate,
    e.Location,
    e.MaxArtworks,
    e.BannerImageUrl,
    COUNT(ea.ArtworkId) AS CurrentArtworks,
    e.MaxArtworks - COUNT(ea.ArtworkId) AS AvailableSlots,
    CASE 
        WHEN GETDATE() < e.StartDate THEN 'Upcoming'
        WHEN GETDATE() BETWEEN e.StartDate AND e.EndDate THEN 'Active'
    END AS CurrentStatus,
    CASE 
        WHEN GETDATE() < e.StartDate THEN DATEDIFF(DAY, GETDATE(), e.StartDate)
        ELSE DATEDIFF(DAY, GETDATE(), e.EndDate)
    END AS DaysUntilChange
FROM 
    Exhibitions e
    LEFT JOIN ExhibitionArtworks ea ON e.ExhibitionId = ea.ExhibitionId
WHERE 
    e.Status IN (0, 1) -- Upcoming or Active
    AND e.EndDate >= GETDATE()
GROUP BY 
    e.ExhibitionId, e.Name, e.Description, e.StartDate, 
    e.EndDate, e.Location, e.MaxArtworks, e.BannerImageUrl, e.Status
HAVING 
    COUNT(ea.ArtworkId) < e.MaxArtworks; -- Only show if slots available
GO

PRINT 'View vw_ActiveExhibitions created successfully!';
GO

-- ============================================
-- View 5: vw_PopularArtworks
-- Purpose: Top artworks by likes and engagement
-- Usage: Homepage featured content, trending section
-- ============================================
CREATE OR ALTER VIEW vw_PopularArtworks
AS
SELECT TOP 100
    aw.ArtworkId,
    aw.Title,
    aw.ImageUrl,
    aw.Likes,
    aw.CreatedDate,
    a.Name AS ArtistName,
    a.ProfileImageUrl AS ArtistImage,
    CASE aw.ArtworkType
        WHEN 0 THEN 'Painting'
        WHEN 1 THEN 'Photography'
        WHEN 2 THEN 'Handmade Craft'
        WHEN 3 THEN 'Sculpture'
        WHEN 4 THEN 'Digital Art'
    END AS ArtworkType,
    -- Engagement metrics
    DATEDIFF(DAY, aw.CreatedDate, GETDATE()) AS DaysSinceCreated,
    CASE 
        WHEN DATEDIFF(DAY, aw.CreatedDate, GETDATE()) > 0 
        THEN CAST(aw.Likes AS FLOAT) / DATEDIFF(DAY, aw.CreatedDate, GETDATE())
        ELSE aw.Likes
    END AS LikesPerDay,
    -- Exhibition status
    CASE 
        WHEN EXISTS (
            SELECT 1 FROM ExhibitionArtworks ea 
            INNER JOIN Exhibitions e ON ea.ExhibitionId = e.ExhibitionId
            WHERE ea.ArtworkId = aw.ArtworkId 
            AND e.Status = 1 -- Active
            AND GETDATE() BETWEEN e.StartDate AND e.EndDate
        ) THEN 1 ELSE 0
    END AS IsInActiveExhibition
FROM 
    Artworks aw
    INNER JOIN Artists a ON aw.ArtistId = a.ArtistId
WHERE 
    aw.Status = 0 -- Active only
ORDER BY 
    aw.Likes DESC, aw.CreatedDate DESC;
GO

PRINT 'View vw_PopularArtworks created successfully!';
GO

-- ============================================
-- View 6: vw_UserDashboard
-- Purpose: User-specific dashboard with personalized information
-- Usage: User profile and activity overview
-- ============================================
CREATE OR ALTER VIEW vw_UserDashboard
AS
SELECT 
    u.UserId,
    u.Username,
    u.Email,
    u.FullName,
    u.UserRole,
    u.CreatedDate AS MemberSince,
    u.LastLoginDate,
    -- If user is an artist
    a.ArtistId,
    a.Name AS ArtistName,
    a.Status AS ArtistStatus,
    -- Statistics
    (SELECT COUNT(*) FROM ArtworkLikes al WHERE al.UserId = u.UserId) AS TotalArtworksLiked,
    CASE 
        WHEN a.ArtistId IS NOT NULL THEN (SELECT COUNT(*) FROM Artworks aw WHERE aw.ArtistId = a.ArtistId)
        ELSE 0
    END AS MyArtworks,
    CASE 
        WHEN a.ArtistId IS NOT NULL THEN ISNULL((SELECT SUM(aw.Likes) FROM Artworks aw WHERE aw.ArtistId = a.ArtistId), 0)
        ELSE 0
    END AS TotalLikesReceived
FROM 
    Users u
    LEFT JOIN Artists a ON u.UserId = a.UserId;
GO

PRINT 'View vw_UserDashboard created successfully!';
GO

-- ============================================
-- Summary
-- ============================================
PRINT '============================================';
PRINT 'All views created successfully!';
PRINT 'Total views: 6';
PRINT '  1. vw_ArtistPortfolio - Artist statistics';
PRINT '  2. vw_ExhibitionDetails - Exhibition metrics';
PRINT '  3. vw_ArtworkGallery - Complete artwork catalog';
PRINT '  4. vw_ActiveExhibitions - Available exhibitions';
PRINT '  5. vw_PopularArtworks - Trending content';
PRINT '  6. vw_UserDashboard - User activity';
PRINT '============================================';
GO
