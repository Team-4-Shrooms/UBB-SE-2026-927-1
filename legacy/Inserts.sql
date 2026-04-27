--DELETE FROM Events;
--DELETE FROM Reviews;
--DELETE FROM Transactions;
--DELETE FROM Equipment;
--DELETE FROM ActiveSales;
--DELETE FROM Users;
--DELETE FROM Movies;
--DELETE FROM OwnedMovies;

--DBCC CHECKIDENT('Events', RESEED, 0) WITH NO_INFOMSGS;
--DBCC CHECKIDENT('Movies', RESEED, 0) WITH NO_INFOMSGS;
--DBCC CHECKIDENT('Equipment', RESEED, 0) WITH NO_INFOMSGS;
--DBCC CHECKIDENT('ActiveSales', RESEED, 0) WITH NO_INFOMSGS;
--DBCC CHECKIDENT('Users', RESEED, 0) WITH NO_INFOMSGS;
--DBCC CHECKIDENT('Reviews', RESEED, 0) WITH NO_INFOMSGS;
--DBCC CHECKIDENT('Transactions', RESEED, 0) WITH NO_INFOMSGS;
--DBCC CHECKIDENT('OwnedMovies', RESEED, 0) WITH NO_INFOMSGS;

-- Make sure User 1 exists
IF NOT EXISTS (SELECT * FROM Users WHERE ID = 1)
    INSERT INTO Users (Username, Email, PasswordHash, Balance) 
    VALUES ('Test', 'test@test.com', 'pwd', 100.00);

-- Make sure Movie 1 exists
IF NOT EXISTS (SELECT * FROM Movies WHERE ID = 1)
    INSERT INTO Movies (Title, Description, Price, ImageUrl) 
    VALUES ('Inception', 'A dream within a dream', 45.00, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS1ylQxJCDkRmlhPcTzBMXenct8rScWPHqvPA&s');

INSERT INTO ActiveSales (MovieID, DiscountPercentage, StartTime, EndTime)
VALUES (1, 20.00, DATEADD(day, -1, GETDATE()), DATEADD(day, 5, GETDATE()));

--  happened last week and ended 2 days ago
INSERT INTO ActiveSales (MovieID, DiscountPercentage, StartTime, EndTime)
VALUES (1, 50.00, DATEADD(day, -10, GETDATE()), DATEADD(day, -2, GETDATE()));

--to see if it expires in one minute when i refresh
INSERT INTO ActiveSales (MovieID, DiscountPercentage, StartTime, EndTime)
VALUES (1, 99.00, GETDATE(), DATEADD(minute, 1, GETDATE()));

-- Insert 5 more movies
IF NOT EXISTS (SELECT * FROM Movies WHERE Title = 'The Matrix')
    INSERT INTO Movies (Title, Description, Price, ImageUrl)
    VALUES ('The Matrix', 'A computer hacker learns about the true nature of reality.', 39.99,
            'https://static0.colliderimages.com/wordpress/wp-content/uploads/2023/05/the-matrix-code-keanu-reeves.jpeg');

IF NOT EXISTS (SELECT * FROM Movies WHERE Title = 'Interstellar')
    INSERT INTO Movies (Title, Description, Price, ImageUrl)
    VALUES ('Interstellar', 'A team travels through a wormhole in search of humanity''s survival.', 49.99,
            'https://m.media-amazon.com/images/I/91vIHsL-zjL._AC_UF894,1000_QL80_.jpg');

IF NOT EXISTS (SELECT * FROM Movies WHERE Title = 'Parasite')
    INSERT INTO Movies (Title, Description, Price, ImageUrl)
    VALUES ('Parasite', 'Greed and class discrimination threaten the newly formed symbiotic relationship.', 29.99,
            'https://m.media-amazon.com/images/M/MV5BYjk1Y2U4MjQtY2ZiNS00OWQyLWI3MmYtZWUwNmRjYWRiNWNhXkEyXkFqcGc@._V1_QL75_UX190_CR0,0,190,281_.jpg');

IF NOT EXISTS (SELECT * FROM Movies WHERE Title = 'John Wick')
    INSERT INTO Movies (Title, Description, Price, ImageUrl)
    VALUES ('John Wick', 'An ex-hitman comes out of retirement to track down the gangsters responsible for his wife''s death.', 34.99,
            'https://m.media-amazon.com/images/M/MV5BMTU2NjA1ODgzMF5BMl5BanBnXkFtZTgwMTM2MTI4MjE@._V1_FMjpg_UX1000_.jpg');

IF NOT EXISTS (SELECT * FROM Movies WHERE Title = 'Whiplash')
    INSERT INTO Movies (Title, Description, Price, ImageUrl)
    VALUES ('Whiplash', 'A young drummer''s obsession with greatness leads to a brutal rivalry with his instructor.', 24.99,
            'https://miro.medium.com/v2/resize:fit:1200/1*HygtAUSg3MqQjimu0MQy3Q.jpeg');

DECLARE @MatrixID INT = (SELECT ID FROM Movies WHERE Title = 'The Matrix');
DECLARE @InterstellarID INT = (SELECT ID FROM Movies WHERE Title = 'Interstellar');
DECLARE @ParasiteID INT = (SELECT ID FROM Movies WHERE Title = 'Parasite');
DECLARE @JohnWickID INT = (SELECT ID FROM Movies WHERE Title = 'John Wick');
DECLARE @WhiplashID INT = (SELECT ID FROM Movies WHERE Title = 'Whiplash');

-- Active discounts for some movies only
IF NOT EXISTS (
    SELECT 1
    FROM ActiveSales
    WHERE MovieID = @MatrixID
      AND StartTime <= GETDATE()
      AND EndTime > GETDATE()
)
INSERT INTO ActiveSales (MovieID, DiscountPercentage, StartTime, EndTime)
VALUES (@MatrixID, 20.00, DATEADD(day, -1, GETDATE()), DATEADD(day, 5, GETDATE()));

IF NOT EXISTS (
    SELECT 1
    FROM ActiveSales
    WHERE MovieID = @InterstellarID
      AND StartTime <= GETDATE()
      AND EndTime > GETDATE()
)
INSERT INTO ActiveSales (MovieID, DiscountPercentage, StartTime, EndTime)
VALUES (@InterstellarID, 35.00, DATEADD(day, -1, GETDATE()), DATEADD(day, 5, GETDATE()));

IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'dummy1')
BEGIN
	INSERT INTO Users(Username, Email, PasswordHash, Balance)
	VALUES ('dummy1', 'dummy1@gmail.com', 'pass1', 0.00);
END

IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'dummy2')
BEGIN
	INSERT INTO Users(Username, Email, PasswordHash, Balance)
	VALUES ('dummy2', 'dummy2@gmail.com', 'pass2', 50.00);
END

DECLARE @Seller1 INT = (SELECT ID FROM Users WHERE Username = 'dummy1');
DECLARE @Seller2 INT = (SELECT ID FROM Users WHERE Username = 'dummy2');

-- Dummy events for a few movies
IF NOT EXISTS (SELECT 1 FROM Events WHERE Title = 'Inception - Midnight Screening')
    INSERT INTO Events (MovieID, Title, Description, Date, Location, TicketPrice, PosterUrl)
    VALUES (1, 'Inception - Midnight Screening',
            'One-night-only midnight screening with a short pre-show talk.',
            DATEADD(day, 7, GETDATE()),
            'Cinema Hall A',
            12.50,
            'https://m.media-amazon.com/images/I/71DwIcSgFcS._AC_UF894,1000_QL80_.jpg');

IF NOT EXISTS (SELECT 1 FROM Events WHERE Title = 'The Matrix - Fan Marathon')
    INSERT INTO Events (MovieID, Title, Description, Date, Location, TicketPrice, PosterUrl)
    VALUES (@MatrixID, 'The Matrix - Fan Marathon',
            'Back-to-back screening + trivia. Doors open 18:00.',
            DATEADD(day, 14, GETDATE()),
            'Retro Theater',
            18.00,
            'https://m.media-amazon.com/images/I/51EG732BV3L.jpg');

IF NOT EXISTS (SELECT 1 FROM Events WHERE Title = 'Interstellar - Space Night')
    INSERT INTO Events (MovieID, Title, Description, Date, Location, TicketPrice, PosterUrl)
    VALUES (@InterstellarID, 'Interstellar - Space Night',
            'Screening followed by a small astronomy Q&A.',
            DATEADD(day, 21, GETDATE()),
            'Science Center Auditorium',
            15.00,
            'https://m.media-amazon.com/images/I/91vIHsL-zjL._AC_UF894,1000_QL80_.jpg');

IF NOT EXISTS (SELECT 1 FROM Events WHERE Title = 'Whiplash - Live Jazz Intro')
    INSERT INTO Events (MovieID, Title, Description, Date, Location, TicketPrice, PosterUrl)
    VALUES (@WhiplashID, 'Whiplash - Live Jazz Intro',
            'Short live jazz set before the movie.',
            DATEADD(day, 10, GETDATE()),
            'Downtown Arts Cinema',
            14.00,
            'https://m.media-amazon.com/images/I/81hKZ6oTqUL._AC_UF894,1000_QL80_.jpg');

-- Reviews for the 5 inserted movies (rating is later averaged into Movies.Rating)
IF NOT EXISTS (SELECT 1 FROM Reviews WHERE MovieID = @MatrixID AND UserID = @Seller1)
    INSERT INTO Reviews (MovieID, UserID, StarRating, Comment)
    VALUES (@MatrixID, @Seller1, 9, 'A mind-bending classic with unforgettable world-building.');

IF NOT EXISTS (SELECT 1 FROM Reviews WHERE MovieID = @MatrixID AND UserID = @Seller2)
    INSERT INTO Reviews (MovieID, UserID, StarRating, Comment)
    VALUES (@MatrixID, @Seller2, 7, 'Great action and ideas, but definitely not for everyone.');

IF NOT EXISTS (SELECT 1 FROM Reviews WHERE MovieID = @InterstellarID AND UserID = @Seller1)
    INSERT INTO Reviews (MovieID, UserID, StarRating, Comment)
    VALUES (@InterstellarID, @Seller1, 10, 'Epic, emotional, and incredibly thought-provoking.');

IF NOT EXISTS (SELECT 1 FROM Reviews WHERE MovieID = @InterstellarID AND UserID = @Seller2)
    INSERT INTO Reviews (MovieID, UserID, StarRating, Comment)
    VALUES (@InterstellarID, @Seller2, 8, 'Beautiful visuals and a satisfying emotional payoff.');

IF NOT EXISTS (SELECT 1 FROM Reviews WHERE MovieID = @ParasiteID AND UserID = @Seller1)
    INSERT INTO Reviews (MovieID, UserID, StarRating, Comment)
    VALUES (@ParasiteID, @Seller1, 9, 'Smart, tense, and darkly funny all the way through.');

IF NOT EXISTS (SELECT 1 FROM Reviews WHERE MovieID = @ParasiteID AND UserID = @Seller2)
    INSERT INTO Reviews (MovieID, UserID, StarRating, Comment)
    VALUES (@ParasiteID, @Seller2, 6, 'Surprisingly entertaining, but the pacing felt uneven.');

IF NOT EXISTS (SELECT 1 FROM Reviews WHERE MovieID = @JohnWickID AND UserID = @Seller1)
    INSERT INTO Reviews (MovieID, UserID, StarRating, Comment)
    VALUES (@JohnWickID, @Seller1, 8, 'Non-stop style and killer action choreography.');

IF NOT EXISTS (SELECT 1 FROM Reviews WHERE MovieID = @JohnWickID AND UserID = @Seller2)
    INSERT INTO Reviews (MovieID, UserID, StarRating, Comment)
    VALUES (@JohnWickID, @Seller2, 7, 'Solid thrills and great atmosphere; easy to binge.');

IF NOT EXISTS (SELECT 1 FROM Reviews WHERE MovieID = @WhiplashID AND UserID = @Seller1)
    INSERT INTO Reviews (MovieID, UserID, StarRating, Comment)
    VALUES (@WhiplashID, @Seller1, 9, 'A brutal, addictive rivalry that stays with you.');

IF NOT EXISTS (SELECT 1 FROM Reviews WHERE MovieID = @WhiplashID AND UserID = @Seller2)
    INSERT INTO Reviews (MovieID, UserID, StarRating, Comment)
    VALUES (@WhiplashID, @Seller2, 8, 'Fantastic performances and a soundtrack that demands attention.');

--insert 5 equipments
INSERT INTO Equipment(SellerID, Title, Category, Description, Condition, Price, ImageUrl, Status)
VALUES
	(@Seller1, 'Canon EOS 2000D Kit', 'Cameras', '24.1 MP APS-C CMOS sensor. Perfect entry-level DSLR for student films, includes 18-55mm IS II Lens and 1080p cinematic video mode.', 'Good', 1200.00, 'https://static0.pocketlintimages.com/wordpress/wp-content/uploads/wm/143700-cameras-review-hands-on-canon-eos-2000d-review-image1-xploy5pbva.jpg', 'Available'),
    
    (@Seller1, 'Rode NTG Shotgun Mic', 'Audio', 'Professional directional condenser microphone. Super-cardioid polar pattern, ideal for isolating dialogue on noisy film sets.', 'New', 1200.00, 'https://fstudio.vtexassets.com/arquivos/ids/750303-1200-1200', 'Sold'),
    
    (@Seller2, 'Blackmagic Pocket Cinema 6K', 'Cameras', 'EF Mount, Super 35 HDR sensor, 13 stops of dynamic range and dual native ISO up to 25,600 for incredible low light performance.', 'Like New', 9500.00, 'https://images.blackmagicdesign.com/images/products/blackmagicpocketcinemacamera/main/pocket-6k-g2-xl.jpg', 'Available'),
    
    (@Seller1, 'DJI RS 3 Pro Gimbal', 'Stabilization', 'Carbon fiber construction, 4.5kg (10lbs) tested payload. Automated axis locks and LiDAR focusing for professional solo cinematographers.', 'New', 4200.00, 'https://m.media-amazon.com/images/I/61S6h1S-z3L._AC_SL1500_.jpg', 'Available'),
    
    (@Seller2, 'Atomos Ninja V+ Monitor', 'Monitoring', '5-inch 4K HDMI Recording Monitor. 1000 nits brightness for outdoor use, supports ProRes RAW recording directly from camera sensor.', 'Used', 2800.00, 'https://m.media-amazon.com/images/I/71N-W-vV6NL._AC_SL1500_.jpg', 'Available');