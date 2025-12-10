-- ============================================
-- Update User Passwords to Hashed Format
-- Run this to fix login issue
-- ============================================

USE ArtGalleryDB;
GO

-- Update passwords to SHA256 hashes
-- Password hashes calculated using SHA256 algorithm

-- admin password = "admin"
UPDATE Users
SET PasswordHash = 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg='
WHERE Username = 'admin';

-- sultan password = "password123"
UPDATE Users  
SET PasswordHash = '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8='
WHERE Username = 'sultan';

-- abdulla password = "password123"
UPDATE Users
SET PasswordHash = '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8='  
WHERE Username = 'abdulla';

GO

PRINT '============================================';
PRINT 'Passwords updated successfully!';
PRINT 'You can now login with:';
PRINT '  Username: admin     | Password: admin';
PRINT '  Username: sultan    | Password: password123';
PRINT '  Username: abdulla   | Password: password123';
PRINT '============================================';
GO
