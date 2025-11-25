-- ============================================
-- MASTER INSTALLATION SCRIPT
-- Art Gallery Management System Database
-- Author: Sultan & Abdulla
-- Date: 2024
-- ============================================
-- Description: This script runs all database setup scripts in order
-- Execute this file to create the complete database from scratch
-- ============================================

PRINT '============================================';
PRINT 'Starting Art Gallery Database Installation';
PRINT 'Please wait...';
PRINT '============================================';
PRINT '';
GO

-- ============================================
-- STEP 1: CREATE DATABASE
-- ============================================
PRINT 'STEP 1/4: Creating database...';
GO

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'ArtGalleryDB')
BEGIN
    ALTER DATABASE ArtGalleryDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ArtGalleryDB;
    PRINT '  - Existing database dropped';
END
GO

CREATE DATABASE ArtGalleryDB;
GO

PRINT '  - Database ArtGalleryDB created successfully!';
PRINT '';
GO

USE ArtGalleryDB;
GO

-- ============================================
-- STEP 2: CREATE TABLES
-- ============================================
PRINT 'STEP 2/4: Creating tables with constraints...';
GO

-- Execute table creation script from 02_CreateTables.sql
-- (Content embedded here for convenience)

:r .\02_CreateTables.sql

PRINT '';
GO

-- ============================================
-- STEP 3: CREATE VIEWS
-- ============================================
PRINT 'STEP 3/4: Creating database views...';
GO

-- Execute view creation script from 03_CreateViews.sql
:r .\03_CreateViews.sql

PRINT '';
GO

-- ============================================
-- STEP 4: INSERT SAMPLE DATA
-- ============================================
PRINT 'STEP 4/4: Populating tables with sample data...';
GO

-- Execute data insertion script from 04_InsertData.sql
:r .\04_InsertData.sql

PRINT '';
GO

-- ============================================
-- FINAL VERIFICATION
-- ============================================
PRINT '============================================';
PRINT 'INSTALLATION COMPLETED SUCCESSFULLY!';
PRINT '============================================';
PRINT '';
PRINT 'Database Summary:';
PRINT '----------------';

SELECT 
    t.name AS TableName,
    SUM(p.rows) AS RowCount,
    COUNT(c.column_id) AS ColumnCount
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.object_id
INNER JOIN sys.columns c ON t.object_id = c.object_id
WHERE p.index_id IN (0,1)
GROUP BY t.name
ORDER BY t.name;

PRINT '';
PRINT 'Views Created:';
PRINT '-------------';

SELECT name AS ViewName
FROM sys.views
ORDER BY name;

PRINT '';
PRINT '============================================';
PRINT 'Next Steps:';
PRINT '1. Update appsettings.json with connection string';
PRINT '2. Install Entity Framework Core packages';
PRINT '3. Create DbContext class';
PRINT '4. Test database connectivity';
PRINT '============================================';
GO
