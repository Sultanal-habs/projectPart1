-- ============================================
-- Fix Exhibition Banner Image URLs
-- ============================================
USE ArtGalleryDB;
GO

PRINT 'Starting exhibition image URL fix...';

-- First, let's see what we have
SELECT ExhibitionId, Name, BannerImageUrl 
FROM Exhibitions 
ORDER BY ExhibitionId;

PRINT '';
PRINT 'Updating exhibition image URLs...';

-- Update exhibitions with correct image paths
UPDATE Exhibitions
SET BannerImageUrl = CASE 
    WHEN Name = 'Contemporary Omani Art' THEN '/images/ContemporaryOmaniArt.jpg'
    WHEN Name = 'Photography Week' THEN '/images/PhotographyWeek.jpg'
    WHEN Name = 'Heritage Crafts Fair' THEN '/images/HeritageCraftsFair.jpg'
    ELSE '/images/exhibitions/default-banner.jpg'
END;

PRINT '';
PRINT 'Exhibition image URLs updated!';

-- Verify the changes
PRINT '';
PRINT 'Updated Exhibition Images:';
SELECT ExhibitionId, Name, BannerImageUrl 
FROM Exhibitions 
ORDER BY ExhibitionId;

PRINT '';
PRINT 'Fix completed successfully!';
GO
