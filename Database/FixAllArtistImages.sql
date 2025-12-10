-- ============================================
-- Complete Fix for All Artist Image URLs
-- ============================================
USE ArtGalleryDB;
GO

PRINT 'Starting complete image URL fix...';

-- Update all artists with correct image paths
UPDATE Artists
SET ProfileImageUrl = CASE 
    WHEN Name = 'Mohammed Al Harthi' THEN '/images/MohammedAlHarthi.jpg'
    WHEN Name = 'Fatima Al Saidi' THEN '/images/FatimaAlSaidi.jpg'
    WHEN Name = 'Ahmed Al Balushi' THEN '/images/AhmedAlBalushi.jpg'
    ELSE '/images/artists/default.svg'
END;

PRINT 'Artist image URLs updated!';

-- Verify the changes
PRINT '';
PRINT 'Current Artist Images:';
SELECT ArtistId, Name, ProfileImageUrl 
FROM Artists 
ORDER BY ArtistId;

PRINT '';
PRINT 'Fix completed successfully!';
GO
