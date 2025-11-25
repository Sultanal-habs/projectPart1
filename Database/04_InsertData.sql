-- ============================================
-- Script: Insert Sample Data
-- Author: Sultan & Abdulla
-- Date: 2024
-- Description: Populates all tables with realistic, comprehensive test data
-- ============================================

USE ArtGalleryDB;
GO

PRINT 'Starting data population...';
GO

-- ============================================
-- 1. Insert Users (Login Information)
-- ============================================
SET IDENTITY_INSERT Users ON;
GO

INSERT INTO Users (UserId, Username, PasswordHash, Email, PhoneNumber, FullName, UserRole, IsActive, CreatedDate, LastLoginDate)
VALUES
    (1, 'admin', 'hashed_password_admin_123', 'admin@artgallery.om', '+96892345678', 'System Administrator', 'Admin', 1, '2023-01-15', '2024-01-20'),
    (2, 'sultan_artist', 'hashed_password_sultan_456', 'sultan@example.com', '+96891234567', 'Sultan Al-Habsi', 'Artist', 1, '2023-06-10', '2024-01-19'),
    (3, 'abdulla_artist', 'hashed_password_abdulla_789', 'abdulla@example.com', '+96898765432', 'Abdulla Al-Saidi', 'Artist', 1, '2023-07-20', '2024-01-18'),
    (4, 'fatima_user', 'hashed_password_fatima_321', 'fatima.user@example.com', '+96893456789', 'Fatima Al-Harthi', 'User', 1, '2023-08-05', '2024-01-17'),
    (5, 'mohammed_mod', 'hashed_password_mohammed_654', 'mohammed@artgallery.om', '+96894567890', 'Mohammed Al-Balushi', 'Moderator', 1, '2023-03-12', '2024-01-19'),
    (6, 'sara_artist', 'hashed_password_sara_987', 'sara@example.com', '+96895678901', 'Sara Al-Rashdi', 'Artist', 1, '2023-09-01', '2024-01-16'),
    (7, 'khalid_user', 'hashed_password_khalid_147', 'khalid@example.com', '+96896789012', 'Khalid Al-Kharusi', 'User', 1, '2023-10-10', '2024-01-15'),
    (8, 'aisha_artist', 'hashed_password_aisha_258', 'aisha@example.com', '+96897890123', 'Aisha Al-Lawati', 'Artist', 1, '2023-11-15', '2024-01-14'),
    (9, 'omar_user', 'hashed_password_omar_369', 'omar@example.com', '+96898901234', 'Omar Al-Hinai', 'User', 1, '2023-12-01', '2024-01-13'),
    (10, 'layla_artist', 'hashed_password_layla_741', 'layla@example.com', '+96899012345', 'Layla Al-Mahrouqi', 'Artist', 1, '2024-01-05', '2024-01-12');
GO

SET IDENTITY_INSERT Users OFF;
GO

PRINT '10 Users inserted successfully!';
GO

-- ============================================
-- 2. Insert Artists
-- ============================================
SET IDENTITY_INSERT Artists ON;
GO

INSERT INTO Artists (ArtistId, Name, Email, Phone, Bio, ProfileImageUrl, Status, JoinedDate, UserId)
VALUES
    (1, 'Mohammed Al Harthi', 'mohammed@example.com', '+96891234567', 'Traditional Omani artist specializing in desert landscapes and cultural heritage. Winner of the Sultan Qaboos Art Award 2022.', '/images/artists/mohammed_al_harthi.jpg', 0, '2023-06-01', 2),
    (2, 'Fatima Al Saidi', 'fatima@example.com', '+96892345678', 'Contemporary photographer capturing modern Omani life and architecture. Featured in National Geographic.', '/images/artists/fatima_al_saidi.jpg', 0, '2023-07-15', 3),
    (3, 'Ahmed Al Balushi', 'ahmed@example.com', '+96893456789', 'Master craftsman known for traditional Omani silverwork and khanjars. 30 years of experience.', '/images/artists/ahmed_al_balushi.jpg', 0, '2022-01-10', NULL),
    (4, 'Sara Al Rashdi', 'sara@example.com', '+96895678901', 'Digital artist blending Islamic geometric patterns with modern technology. TEDx speaker.', '/images/artists/sara_al_rashdi.jpg', 0, '2023-09-01', 6),
    (5, 'Aisha Al Lawati', 'aisha@example.com', '+96897890123', 'Sculpture artist working with local materials to create contemporary pieces inspired by Omani heritage.', '/images/artists/aisha_al_lawati.jpg', 0, '2023-11-15', 8),
    (6, 'Khalid Al Habsi', 'khalid.artist@example.com', '+96894567890', 'Oil painter focusing on portraits and figurative art. Graduated from the Royal College of Art.', '/images/artists/khalid_al_habsi.jpg', 0, '2023-05-20', NULL),
    (7, 'Layla Al Mahrouqi', 'layla@example.com', '+96899012345', 'Mixed media artist exploring themes of identity and tradition in the modern world.', '/images/artists/layla_al_mahrouqi.jpg', 0, '2024-01-05', 10),
    (8, 'Youssef Al Amri', 'youssef@example.com', '+96890123456', 'Calligraphy expert specializing in Arabic calligraphy and Islamic art.', '/images/artists/youssef_al_amri.jpg', 0, '2022-08-12', NULL),
    (9, 'Mariam Al Wahaibi', 'mariam@example.com', '+96891012345', 'Watercolor artist known for vibrant depictions of Omani nature and wildlife.', '/images/artists/mariam_al_wahaibi.jpg', 3, '2024-01-18', NULL),
    (10, 'Hassan Al Kharusi', 'hassan@example.com', '+96892013456', 'Photography artist documenting Omani traditions and ceremonies.', '/images/artists/hassan_al_kharusi.jpg', 1, '2021-03-15', NULL);
GO

SET IDENTITY_INSERT Artists OFF;
GO

PRINT '10 Artists inserted successfully!';
GO

-- ============================================
-- 3. Insert Exhibitions
-- ============================================
SET IDENTITY_INSERT Exhibitions ON;
GO

INSERT INTO Exhibitions (ExhibitionId, Name, Description, StartDate, EndDate, Location, MaxArtworks, Status, BannerImageUrl, CreatedDate, CreatedByUserId)
VALUES
    (1, 'Heritage & Modernity', 'A celebration of Omani culture blending traditional art forms with contemporary expressions. This exhibition showcases the evolution of Omani art.', '2024-02-01', '2024-03-31', 'Royal Opera House Muscat', 50, 1, '/images/exhibitions/heritage_modernity.jpg', '2023-12-01', 1),
    (2, 'Desert Dreams', 'Landscapes and abstract works inspired by the beauty of Omani deserts. Experience the golden dunes through the eyes of local artists.', '2024-03-15', '2024-05-15', 'Bait Al Zubair Museum', 30, 0, '/images/exhibitions/desert_dreams.jpg', '2024-01-10', 5),
    (3, 'Urban Lens: Muscat Through Time', 'Photographic journey documenting the transformation of Muscat from traditional port to modern capital.', '2024-01-10', '2024-02-28', 'National Museum of Oman', 40, 1, '/images/exhibitions/urban_lens.jpg', '2023-11-15', 1),
    (4, 'Crafted Traditions', 'Traditional Omani handicrafts including silverwork, pottery, and textile art. Meet the master artisans.', '2024-04-01', '2024-06-30', 'Mutrah Souq Gallery', 35, 0, '/images/exhibitions/crafted_traditions.jpg', '2024-01-05', 5),
    (5, 'Digital Horizons', 'Contemporary digital art and NFT showcase featuring emerging Omani digital artists.', '2023-11-01', '2024-01-15', 'Cultural Complex', 25, 2, '/images/exhibitions/digital_horizons.jpg', '2023-09-20', 1),
    (6, 'Women in Art: Omani Voices', 'Celebrating female Omani artists and their unique perspectives on society, culture, and identity.', '2024-05-01', '2024-07-31', 'Al Amerat Cultural Center', 45, 0, '/images/exhibitions/women_voices.jpg', '2024-01-12', 5),
    (7, 'Calligraphy Masters', 'Exhibition of classical and modern Arabic calligraphy by renowned regional artists.', '2023-12-01', '2024-02-15', 'Sultan Qaboos Grand Mosque Gallery', 20, 1, '/images/exhibitions/calligraphy.jpg', '2023-10-10', 1),
    (8, 'Marine Wonders', 'Underwater photography and marine life paintings celebrating Oman''s rich coastal biodiversity.', '2024-06-01', '2024-08-31', 'Marine Science Center', 30, 0, '/images/exhibitions/marine_wonders.jpg', '2024-01-15', 1);
GO

SET IDENTITY_INSERT Exhibitions OFF;
GO

PRINT '8 Exhibitions inserted successfully!';
GO

-- ============================================
-- 4. Insert Artworks
-- ============================================
SET IDENTITY_INSERT Artworks ON;
GO

INSERT INTO Artworks (ArtworkId, Title, Description, ImageUrl, ArtworkType, Likes, CreatedDate, Price, IsForSale, Status, ArtistId, ExhibitionId)
VALUES
    -- Mohammed Al Harthi artworks (Artist 1)
    (1, 'Desert at Dawn', 'Breathtaking oil painting capturing the golden hour over Wahiba Sands with intricate details of sand patterns.', '/images/artworks/desert_dawn.jpg', 0, 145, '2023-10-15', 2500.00, 1, 0, 1, 1),
    (2, 'Frankincense Trail', 'Historical depiction of ancient trade routes through Dhofar region in warm earth tones.', '/images/artworks/frankincense_trail.jpg', 0, 98, '2023-11-20', 3200.00, 1, 0, 1, 1),
    (3, 'Omani Fort Sunset', 'Traditional fort silhouette against vibrant sunset sky, symbolizing strength and heritage.', '/images/artworks/fort_sunset.jpg', 0, 210, '2024-01-05', 2800.00, 1, 0, 1, 3),
    
    -- Fatima Al Saidi artworks (Artist 2)
    (4, 'Muttrah Corniche Nights', 'Long exposure photograph of the illuminated corniche reflecting on calm waters.', '/images/artworks/muttrah_nights.jpg', 1, 187, '2023-09-10', 1500.00, 1, 0, 2, 3),
    (5, 'Souq Stories', 'Candid black and white portrait series of souq vendors and their daily lives.', '/images/artworks/souq_stories.jpg', 1, 156, '2023-12-01', 1800.00, 1, 0, 2, 3),
    (6, 'Architectural Harmony', 'Modern Muscat architecture juxtaposed with traditional design elements.', '/images/artworks/arch_harmony.jpg', 1, 134, '2024-01-10', 2000.00, 1, 0, 2, NULL),
    
    -- Ahmed Al Balushi artworks (Artist 3)
    (7, 'Royal Khanjar Set', 'Exquisitely crafted silver khanjar with ornate details and precious stones.', '/images/artworks/royal_khanjar.jpg', 2, 223, '2023-08-20', 5500.00, 1, 0, 3, 4),
    (8, 'Traditional Omani Jewelry', 'Handmade silver jewelry collection featuring traditional Omani motifs and techniques.', '/images/artworks/omani_jewelry.jpg', 2, 178, '2023-10-05', 3200.00, 1, 0, 3, 4),
    (9, 'Incense Burner Art', 'Functional art piece - ornate silver incense burner with geometric patterns.', '/images/artworks/incense_burner.jpg', 2, 145, '2023-11-15', 2800.00, 1, 0, 3, 1),
    
    -- Sara Al Rashdi artworks (Artist 4)
    (10, 'Geometric Dreams', 'Digital art piece exploring Islamic geometric patterns in vibrant neon colors.', '/images/artworks/geometric_dreams.jpg', 4, 267, '2023-11-01', 1200.00, 1, 0, 4, 5),
    (11, 'Data Dunes', 'Algorithmic art visualizing desert landscapes through data and code.', '/images/artworks/data_dunes.jpg', 4, 198, '2023-12-10', 1500.00, 1, 2, 4, 5),
    (12, 'NFT: Digital Khanjar', 'Limited edition NFT reimagining the traditional khanjar in cyberpunk aesthetic.', '/images/artworks/digital_khanjar.jpg', 4, 312, '2023-10-20', 2500.00, 0, 0, 4, NULL),
    
    -- Aisha Al Lawati artworks (Artist 5)
    (13, 'Stone Whispers', 'Contemporary sculpture carved from Omani marble representing wind-eroded desert rocks.', '/images/artworks/stone_whispers.jpg', 3, 156, '2023-11-25', 8500.00, 1, 0, 5, 1),
    (14, 'Bronze Heritage', 'Life-size bronze sculpture of Omani fisherman casting traditional net.', '/images/artworks/bronze_heritage.jpg', 3, 189, '2023-12-15', 12000.00, 1, 0, 5, NULL),
    
    -- Khalid Al Habsi artworks (Artist 6)
    (15, 'Portrait of Tradition', 'Oil portrait of elderly Omani man in traditional dress with incredible detail.', '/images/artworks/portrait_tradition.jpg', 0, 167, '2023-09-15', 4200.00, 1, 0, 6, 1),
    (16, 'Market Day', 'Vibrant painting capturing the energy and colors of a traditional fish market.', '/images/artworks/market_day.jpg', 0, 134, '2023-10-30', 3800.00, 1, 0, 6, NULL),
    
    -- Layla Al Mahrouqi artworks (Artist 7)
    (17, 'Identity Layers', 'Mixed media collage exploring modern Omani identity through textiles and photography.', '/images/artworks/identity_layers.jpg', 0, 98, '2024-01-08', 2200.00, 1, 0, 7, 6),
    (18, 'Heritage Reimagined', 'Contemporary interpretation of traditional Omani patterns using modern materials.', '/images/artworks/heritage_reimagined.jpg', 0, 76, '2024-01-15', 2800.00, 1, 0, 7, NULL),
    
    -- Youssef Al Amri artworks (Artist 8)
    (19, 'Quranic Verses in Thuluth', 'Masterful calligraphy piece in traditional Thuluth script with gold leaf accents.', '/images/artworks/quran_thuluth.jpg', 4, 234, '2023-12-05', 3500.00, 1, 0, 8, 7),
    (20, 'Poetry in Motion', 'Contemporary calligraphy blending classical Arabic poetry with modern artistic expression.', '/images/artworks/poetry_motion.jpg', 4, 201, '2023-11-10', 2900.00, 1, 0, 8, 7),
    
    -- Mariam Al Wahaibi artworks (Artist 9)
    (21, 'Wadi Paradise', 'Watercolor painting of lush Omani wadi with crystal clear pools and palm trees.', '/images/artworks/wadi_paradise.jpg', 0, 112, '2024-01-12', 1800.00, 1, 1, 9, NULL),
    (22, 'Arabian Oryx', 'Delicate watercolor study of Oman''s national animal in its natural habitat.', '/images/artworks/arabian_oryx.jpg', 0, 145, '2024-01-18', 1500.00, 1, 1, 9, NULL),
    
    -- Hassan Al Kharusi artworks (Artist 10)
    (23, 'Wedding Traditions', 'Documentary series photographing traditional Omani wedding ceremonies.', '/images/artworks/wedding_traditions.jpg', 1, 178, '2023-07-20', NULL, 0, 3, 10, NULL),
    (24, 'Razha Dance', 'Action photography capturing the energy of traditional Omani sword dance.', '/images/artworks/razha_dance.jpg', 1, 156, '2023-08-15', NULL, 0, 3, 10, NULL),
    
    -- Additional artworks for variety
    (25, 'Coastal Morning', 'Serene beach scene at sunrise with traditional Omani dhow boats.', '/images/artworks/coastal_morning.jpg', 0, 89, '2024-01-10', 2100.00, 1, 0, 1, NULL),
    (26, 'Mountain Majesty', 'Panoramic view of Jebel Shams with dramatic lighting and clouds.', '/images/artworks/mountain_majesty.jpg', 1, 123, '2023-12-20', 2200.00, 1, 0, 2, 3),
    (27, 'Silver Tea Set', 'Complete traditional Omani tea service in ornate silver.', '/images/artworks/tea_set.jpg', 2, 167, '2023-09-30', 4500.00, 1, 0, 3, 4),
    (28, 'Digital Souk', 'Virtual reality experience recreating traditional Omani marketplace.', '/images/artworks/digital_souk.jpg', 4, 245, '2023-11-15', 3500.00, 1, 2, 4, 5),
    (29, 'Clay Traditions', 'Series of ceramic vessels using ancient Omani pottery techniques.', '/images/artworks/clay_traditions.jpg', 2, 101, '2023-10-12', 1200.00, 1, 0, 5, 4),
    (30, 'Urban Portraits', 'Contemporary portrait series of modern Omani youth.', '/images/artworks/urban_portraits.jpg', 0, 134, '2023-11-28', 3200.00, 1, 0, 6, NULL);
GO

SET IDENTITY_INSERT Artworks OFF;
GO

PRINT '30 Artworks inserted successfully!';
GO

-- ============================================
-- 5. Insert ExhibitionArtworks (Junction Table)
-- ============================================
INSERT INTO ExhibitionArtworks (ExhibitionId, ArtworkId, AddedDate, DisplayOrder, IsFeatured)
VALUES
    -- Heritage & Modernity (Exhibition 1)
    (1, 1, '2024-01-15', 1, 1),
    (1, 2, '2024-01-15', 2, 0),
    (1, 3, '2024-01-16', 3, 1),
    (1, 9, '2024-01-16', 4, 0),
    (1, 13, '2024-01-17', 5, 1),
    (1, 15, '2024-01-17', 6, 0),
    
    -- Urban Lens (Exhibition 3)
    (3, 4, '2024-01-05', 1, 1),
    (3, 5, '2024-01-05', 2, 1),
    (3, 6, '2024-01-06', 3, 0),
    (3, 26, '2024-01-07', 4, 0),
    
    -- Crafted Traditions (Exhibition 4)
    (4, 7, '2024-01-20', 1, 1),
    (4, 8, '2024-01-20', 2, 1),
    (4, 27, '2024-01-21', 3, 0),
    (4, 29, '2024-01-21', 4, 0),
    
    -- Digital Horizons (Exhibition 5)
    (5, 10, '2023-10-25', 1, 1),
    (5, 11, '2023-10-25', 2, 1),
    (5, 28, '2023-10-26', 3, 0),
    
    -- Women in Art (Exhibition 6)
    (6, 17, '2024-01-18', 1, 1),
    
    -- Calligraphy Masters (Exhibition 7)
    (7, 19, '2023-11-20', 1, 1),
    (7, 20, '2023-11-20', 2, 1);
GO

PRINT 'ExhibitionArtworks relationships created successfully!';
GO

-- ============================================
-- 6. Insert ArtworkLikes
-- ============================================
SET IDENTITY_INSERT ArtworkLikes ON;
GO

-- Generate realistic like patterns
DECLARE @LikeId INT = 1;
DECLARE @ArtworkId INT;
DECLARE @UserId INT;
DECLARE @LikeCount INT;

-- Popular artworks get more likes
INSERT INTO ArtworkLikes (LikeId, ArtworkId, UserId, LikedDate, IpAddress)
VALUES
    -- Artwork 12 (NFT Digital Khanjar - 312 likes)
    (@LikeId, 12, 1, '2023-10-21', '192.168.1.1'), (@LikeId + 1, 12, 2, '2023-10-21', '192.168.1.2'),
    (@LikeId + 2, 12, 3, '2023-10-22', '192.168.1.3'), (@LikeId + 3, 12, 4, '2023-10-22', '192.168.1.4'),
    (@LikeId + 4, 12, 6, '2023-10-23', '192.168.1.6'), (@LikeId + 5, 12, 7, '2023-10-23', '192.168.1.7'),
    (@LikeId + 6, 12, 9, '2023-10-24', '192.168.1.9'), (@LikeId + 7, 12, 10, '2023-10-24', '192.168.1.10'),
    
    -- Artwork 10 (Geometric Dreams - 267 likes)
    (@LikeId + 8, 10, 1, '2023-11-02', '192.168.1.1'), (@LikeId + 9, 10, 3, '2023-11-02', '192.168.1.3'),
    (@LikeId + 10, 10, 4, '2023-11-03', '192.168.1.4'), (@LikeId + 11, 10, 7, '2023-11-03', '192.168.1.7'),
    (@LikeId + 12, 10, 9, '2023-11-04', '192.168.1.9'), (@LikeId + 13, 10, 10, '2023-11-04', '192.168.1.10'),
    
    -- Artwork 28 (Digital Souk - 245 likes)
    (@LikeId + 14, 28, 2, '2023-11-16', '192.168.1.2'), (@LikeId + 15, 28, 3, '2023-11-16', '192.168.1.3'),
    (@LikeId + 16, 28, 4, '2023-11-17', '192.168.1.4'), (@LikeId + 17, 28, 6, '2023-11-17', '192.168.1.6'),
    
    -- Artwork 19 (Quranic Verses - 234 likes)
    (@LikeId + 18, 19, 1, '2023-12-06', '192.168.1.1'), (@LikeId + 19, 19, 2, '2023-12-06', '192.168.1.2'),
    (@LikeId + 20, 19, 4, '2023-12-07', '192.168.1.4'), (@LikeId + 21, 19, 7, '2023-12-07', '192.168.1.7'),
    (@LikeId + 22, 19, 9, '2023-12-08', '192.168.1.9'), (@LikeId + 23, 19, 10, '2023-12-08', '192.168.1.10'),
    
    -- Artwork 7 (Royal Khanjar - 223 likes)
    (@LikeId + 24, 7, 1, '2023-08-21', '192.168.1.1'), (@LikeId + 25, 7, 3, '2023-08-21', '192.168.1.3'),
    (@LikeId + 26, 7, 4, '2023-08-22', '192.168.1.4'), (@LikeId + 27, 7, 6, '2023-08-22', '192.168.1.6'),
    (@LikeId + 28, 7, 7, '2023-08-23', '192.168.1.7'), (@LikeId + 29, 7, 9, '2023-08-23', '192.168.1.9'),
    
    -- Artwork 3 (Fort Sunset - 210 likes)
    (@LikeId + 30, 3, 2, '2024-01-06', '192.168.1.2'), (@LikeId + 31, 3, 3, '2024-01-06', '192.168.1.3'),
    (@LikeId + 32, 3, 4, '2024-01-07', '192.168.1.4'), (@LikeId + 33, 3, 6, '2024-01-07', '192.168.1.6'),
    (@LikeId + 34, 3, 7, '2024-01-08', '192.168.1.7'), (@LikeId + 35, 3, 9, '2024-01-08', '192.168.1.9'),
    
    -- More likes for other popular artworks
    (@LikeId + 36, 1, 3, '2023-10-16', '192.168.1.3'), (@LikeId + 37, 1, 4, '2023-10-17', '192.168.1.4'),
    (@LikeId + 38, 4, 1, '2023-09-11', '192.168.1.1'), (@LikeId + 39, 4, 6, '2023-09-12', '192.168.1.6'),
    (@LikeId + 40, 5, 2, '2023-12-02', '192.168.1.2'), (@LikeId + 41, 5, 7, '2023-12-03', '192.168.1.7'),
    (@LikeId + 42, 13, 4, '2023-11-26', '192.168.1.4'), (@LikeId + 43, 13, 9, '2023-11-27', '192.168.1.9'),
    (@LikeId + 44, 14, 3, '2023-12-16', '192.168.1.3'), (@LikeId + 45, 14, 10, '2023-12-17', '192.168.1.10'),
    (@LikeId + 46, 8, 1, '2023-10-06', '192.168.1.1'), (@LikeId + 47, 8, 7, '2023-10-07', '192.168.1.7'),
    (@LikeId + 48, 15, 2, '2023-09-16', '192.168.1.2'), (@LikeId + 49, 15, 6, '2023-09-17', '192.168.1.6'),
    (@LikeId + 50, 20, 4, '2023-11-11', '192.168.1.4'), (@LikeId + 51, 20, 9, '2023-11-12', '192.168.1.9');
GO

SET IDENTITY_INSERT ArtworkLikes OFF;
GO

PRINT '52+ ArtworkLikes inserted successfully!';
GO

-- ============================================
-- Summary and Verification
-- ============================================
PRINT '============================================';
PRINT 'DATA POPULATION COMPLETED SUCCESSFULLY!';
PRINT '============================================';
PRINT '';
PRINT 'Summary of inserted records:';
PRINT '----------------------------';

SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL
SELECT 'Artists', COUNT(*) FROM Artists
UNION ALL
SELECT 'Exhibitions', COUNT(*) FROM Exhibitions
UNION ALL
SELECT 'Artworks', COUNT(*) FROM Artworks
UNION ALL
SELECT 'ExhibitionArtworks', COUNT(*) FROM ExhibitionArtworks
UNION ALL
SELECT 'ArtworkLikes', COUNT(*) FROM ArtworkLikes;

PRINT '';
PRINT 'Sample Queries to Verify Data:';
PRINT '-------------------------------';
PRINT '-- View active artists with artwork counts:';
PRINT 'SELECT * FROM vw_ArtistPortfolio WHERE StatusName = ''Active'';';
PRINT '';
PRINT '-- View current exhibitions:';
PRINT 'SELECT * FROM vw_ExhibitionDetails WHERE StatusName IN (''Active'', ''Upcoming'');';
PRINT '';
PRINT '-- View popular artworks:';
PRINT 'SELECT TOP 10 * FROM vw_PopularArtworks;';
PRINT '';
PRINT '============================================';
GO
