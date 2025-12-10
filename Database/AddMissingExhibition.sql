-- ============================================
-- Add Missing Exhibition: Heritage Crafts Fair
-- ============================================
USE ArtGalleryDB;
GO

PRINT 'Adding missing exhibition...';

-- Check if already exists
IF NOT EXISTS (SELECT 1 FROM Exhibitions WHERE Name = 'Heritage Crafts Fair')
BEGIN
    INSERT INTO Exhibitions (Name, Description, StartDate, EndDate, Location, MaxArtworks, Status, BannerImageUrl, CreatedDate, CreatedByUserId)
    VALUES (
        'Heritage Crafts Fair',
        'Exhibition showcasing traditional Omani craftsmanship including silverwork, pottery, and textiles',
        DATEADD(DAY, 30, GETDATE()),  -- Starts in 30 days
        DATEADD(DAY, 45, GETDATE()),  -- Ends in 45 days
        'Nizwa Fort',
        60,
        0,  -- Status: Upcoming
        '/images/HeritageCraftsFair.jpg',
        GETDATE(),
        1  -- Created by admin user
    );
    
    PRINT 'Heritage Crafts Fair exhibition added successfully!';
END
ELSE
BEGIN
    PRINT 'Heritage Crafts Fair already exists, skipping...';
END

-- Show all exhibitions
PRINT '';
PRINT 'Current Exhibitions:';
SELECT ExhibitionId, Name, Status, StartDate, EndDate, Location, BannerImageUrl
FROM Exhibitions
ORDER BY ExhibitionId;

-- Show count
DECLARE @Count INT;
SELECT @Count = COUNT(*) FROM Exhibitions;
PRINT '';
PRINT 'Total Exhibitions: ' + CAST(@Count AS VARCHAR);

GO
