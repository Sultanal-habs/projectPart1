-- ============================================
-- Script: Create Database for Art Gallery Management System
-- Author: Sultan & Abdulla
-- Date: 2024
-- Description: Creates the main database for projectPart1
-- ============================================

USE master;
GO

-- Drop database if exists (for development purposes)
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'ArtGalleryDB')
BEGIN
    ALTER DATABASE ArtGalleryDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ArtGalleryDB;
END
GO

-- Create new database
CREATE DATABASE ArtGalleryDB;
GO

-- Use the database
USE ArtGalleryDB;
GO

PRINT 'Database ArtGalleryDB created successfully!';
GO
