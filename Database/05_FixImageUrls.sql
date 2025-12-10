-- ============================================
-- Fix Image URLs for Artists
-- ============================================
USE ArtGalleryDB;
GO

PRINT 'Fixing Artist Image URLs...';

UPDATE Artists
SET ProfileImageUrl = CASE ArtistId
    WHEN 1 THEN '/images/MohammedAlHarthi.jpg'
    WHEN 2 THEN '/images/FatimaAlSaidi.jpg'
    WHEN 3 THEN '/images/AhmedAlBalushi.jpg'
    ELSE '/images/artists/default.svg'
END
WHERE ArtistId IN (1, 2, 3);

PRINT 'Artist image URLs updated successfully!';

-- Verify the changes
SELECT ArtistId, Name, ProfileImageUrl 
FROM Artists 
WHERE ArtistId IN (1, 2, 3);

GO
