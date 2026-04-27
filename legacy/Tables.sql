DROP TABLE IF EXISTS OwnedMovies;
DROP TABLE IF EXISTS OwnedTickets;
DROP TABLE IF EXISTS Transactions;
DROP TABLE IF EXISTS ActiveSales;
DROP TABLE IF EXISTS Events;
DROP TABLE IF EXISTS Equipment;
DROP TABLE IF EXISTS Reviews;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS Movies;

CREATE TABLE Users
(
    ID           INT            NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    Username     NVARCHAR(100)  NOT NULL,
    Email        NVARCHAR(255)  NOT NULL,
    PasswordHash NVARCHAR(255)  NOT NULL,
    Balance      DECIMAL(15, 2) NOT NULL DEFAULT 0.00
);

CREATE TABLE Movies
(
    ID          INT            NOT NULL    IDENTITY(1, 1)  PRIMARY KEY,
    Title       NVARCHAR(255)  NOT NULL,
    Description NVARCHAR(MAX)  NOT NULL,
    Price       DECIMAL(15, 2) NOT NULL,
    ImageUrl    NVARCHAR(500)  NOT NULL
);

CREATE TABLE Equipment
(
    ID          INT            NOT NULL    IDENTITY(1, 1)  PRIMARY KEY,
    SellerID    INT            NOT NULL    REFERENCES Users(ID),
    Title       NVARCHAR(255)  NOT NULL,
    Category    NVARCHAR(100)  NOT NULL,
    Description NVARCHAR(MAX)  NOT NULL,
    Condition   NVARCHAR(50)   NOT NULL,
    Price       DECIMAL(15, 2) NOT NULL,
    ImageUrl    NVARCHAR(500)  NOT NULL,
    Status      NVARCHAR(50)   NOT NULL
);

CREATE TABLE Events
(
    ID          INT            NOT NULL    IDENTITY(1, 1)  PRIMARY KEY,
    MovieID     INT            NOT NULL    REFERENCES Movies(ID),
    Title       NVARCHAR(255)  NOT NULL,
    Description NVARCHAR(MAX)  NOT NULL,
    Date        DATETIME       NOT NULL,
    Location    NVARCHAR(255)  NOT NULL,
    TicketPrice DECIMAL(15, 2) NOT NULL,
    PosterUrl   NVARCHAR(500)  NOT NULL
);

CREATE TABLE ActiveSales
(
    ID                 INT           NOT NULL    IDENTITY(1, 1)  PRIMARY KEY,
    MovieID            INT           NOT NULL    REFERENCES Movies(ID),
    DiscountPercentage DECIMAL(5, 2) NOT NULL,
    StartTime          DATETIME      NOT NULL,
    EndTime            DATETIME      NOT NULL
);

CREATE TABLE Reviews
(
    ID         INT            NOT NULL    IDENTITY(1, 1)  PRIMARY KEY,
    MovieID    INT            NOT NULL    REFERENCES Movies(ID),
    UserID     INT            NOT NULL    REFERENCES Users(ID),
    StarRating INT  NOT NULL,
    Comment    NVARCHAR(MAX)  NULL,
    CreatedAt  DATETIME       NOT NULL    DEFAULT GETDATE()
);

CREATE TABLE OwnedMovies
(
    ID           INT       NOT NULL    IDENTITY(1, 1)  PRIMARY KEY,
    UserID       INT       NOT NULL    REFERENCES Users(ID),
    MovieID      INT       NOT NULL    REFERENCES Movies(ID),
    PurchaseDate DATETIME2 NOT NULL    DEFAULT GETDATE(),
    CONSTRAINT UX_OwnedMovies_User_Movie UNIQUE (UserID, MovieID)
);

CREATE TABLE OwnedTickets
(
    ID           INT       NOT NULL    IDENTITY(1, 1)  PRIMARY KEY,
    UserID       INT       NOT NULL    REFERENCES Users(ID),
    EventID      INT       NOT NULL    REFERENCES Events(ID),
    PurchaseDate DATETIME  NOT NULL    DEFAULT GETDATE()
);

CREATE TABLE Transactions
(
    ID              INT            NOT NULL    IDENTITY(1, 1)  PRIMARY KEY,
    BuyerID         INT            NOT NULL    REFERENCES Users(ID),
    SellerID        INT            NULL        REFERENCES Users(ID),
    EquipmentID     INT            NULL        REFERENCES Equipment(ID),
    MovieID         INT            NULL        REFERENCES Movies(ID),
    EventID         INT            NULL        REFERENCES Events(ID),
    Amount          DECIMAL(15, 2) NOT NULL,
    Type            NVARCHAR(50)   NOT NULL,
    Status          NVARCHAR(50)   NOT NULL,
    Timestamp       DATETIME       NOT NULL    DEFAULT GETDATE(),
    ShippingAddress NVARCHAR(500)  NULL
);