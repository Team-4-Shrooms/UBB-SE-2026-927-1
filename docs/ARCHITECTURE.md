Relationship Breakdown
**1. One-to-One Relationships**
- Users ↔ UserProfiles: Every user has exactly one profile containing their aggregated analytics. The UserId acts as the foreign key in the UserProfiles table.
```
```
Field | Id | Username | Email | PasswordHash | Balance
:--- | :--- | :--- | :--- | :--- | :--- 
**Data Type** | **int** | **string** | **string** | **string** | **decimal**
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️ *(1 to 1)*
Field | Id | TotalLikes | TotalWatchTimeSeconds | TotalClipsViewed | LikeToViewRatio | LastUpdated | AverageWatchTimeSeconds | UserId
:--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :---
**Data Type** | **int** | **int** | **big int** | **int** | **decimal** | **datetime2** | **decimal** | **int**
```
```
- Movies ↔ ActiveSales: A movie may optionally have one active sale attached to it. The MovieId acts as the foreign key in the ActiveSales table.
```
```
Field | Id | Title | Description | Rating | Price | PrimaryGenre | PosterUrl | ReleaseYear | IsOnSale | ActiveSaleDiscountPercent | Synopsis
:--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :---
**Data Type** | **int** | **nvarchar** | **nvarchar** | **decimal** | **decimal** | **nvarchar** | **nvarchar** | **int** | **bit** | **decimal** | **nvarchar**
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️ *(1 to 0..1)* 
Field | Id | DiscountPercentage | StartDate | EndDate | MovieId
:--- | :--- | :--- | :--- | :--- | :---
**Data Type** | **int** | **decimal** | **datetime2** | **datetime2** | **int**
```
```
**2. One-to-Many Relationships**
- Users → Reels: Users act as creators. A single user can create multiple short-form video reels.
```
```
Field | Id | Username | Email | PasswordHash | Balance
:--- | :--- | :--- | :--- | :--- | :--- 
**Data Type** | **int** | **string** | **string** | **string** | **decimal**
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️ *(1 to n)*
Field | Id | VideoUrl | ThumbnailUrl | Title | Caption | FeatureDurationSeconds | CropDataJson | BackgroundMusicId | Source | Genre | CreatedAt | LastEditedAt | MovieId | CreatorUserId
:--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :---  | :---  | :---  | :--- 
**Data Type** | **int** | **nvarchar** | **nvarchar** | **nvarchar** | **nvarchar** | **decimal** | **nvarchar** | **int** | **nvarchar** | **nvarchar** | **datetime2** | **datetime2** | **int** | **int** |
```
```
- Movies → Reels: A movie can have multiple associated reels.
```
```
Field | Id | Title | Description | Rating | Price | PrimaryGenre | PosterUrl | ReleaseYear | IsOnSale | ActiveSaleDiscountPercent | Synopsis
:--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :---
**Data Type** | **int** | **nvarchar** | **nvarchar** | **decimal** | **decimal** | **nvarchar** | **nvarchar** | **int** | **bit** | **decimal** | **nvarchar**
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️ *(1 to n)* 
Field | Id | Title | Caption | VideoUrl | ThumbnailUrl | FeatureDurationSeconds | CropDataJson | BackgroundMusicId | Source | Genre | CreatedAt | LastEditedAt | MovieId | CreatorUserId
:--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :---  | :---  | :---  | :--- 
**Data Type** | **int** | **nvarchar** | **nvarchar** | **nvarchar** | **nvarchar** | **decimal** | **nvarchar** | **int** | **nvarchar** | **nvarchar** | **datetime2** | **datetime2** | **int** | **int** |
```
```
- Users → Equipments: A user can list multiple pieces of filming equipment for sale.
```
```
Field | Id | Username | Email | PasswordHash | Balance
:--- | :--- | :--- | :--- | :--- | :--- 
**Data Type** | **int** | **string** | **string** | **string** | **decimal**
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️ *(1 to n)*
Field | Id | Title | Category| Description| Condition | Price | ImageUrl | Status | SellerId
:--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--
**Data Type** | **int** | **nvarchar** | **nvarchar** | **nvarchar** | **nvarchar** | **decimal** | **nvarchar** | **int** | **int**
```
```
- Movies → MovieEvents: A single movie can have multiple real-world screening events.
```
```
Field | Id | Title | Description | Rating | Price | PrimaryGenre | PosterUrl | ReleaseYear | IsOnSale | ActiveSaleDiscountPercent | Synopsis
:--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :---
**Data Type** | **int** | **nvarchar** | **nvarchar** | **decimal** | **decimal** | **nvarchar** | **nvarchar** | **int** | **bit** | **decimal** | **nvarchar**
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️ *(1 to n)* 
Field | Id | Title | Description | Date | Location | TicketPrice | PosterUrl | MovieId
:--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :---
**Data Type** | **int** | **nvarchar** | **nvarchar** | **datetime2** | **nvarchar** | **decimal** | **nvarchar** | **int**
```
```
- ScrapeJobs → ScrapeJobLogs: System background jobs log their operational status into multiple log entries.
```
```
Field | Id | SearchQuery | MaxResults | Status | MoviesFound | ReelsCreated | StartedAt | CompletedAt | ErrorMessage
:--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :---
**Data Type** | **int** | **nvarchar** | **int** | **nvarchar** | **int** | **int** | **datetime2** | **datetime2** | **nvarchar**
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⬇️ *(1 to n)* 
Field | Level | Message | Timestamp | ScrapeJobId
:--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :---
**Data Type** | **int** | **nvarchar** | **datetime2** | **int**
```
```
**3. Many-to-Many Relationships**
- OwnedMovies: Connects a User to a Movie representing a digital purchase.
```
```
Field | Id | PurchasedDate | UserId | MovieId
--- | --- | --- | --- | ---
**Data Type** | **int** | **datetime2** | **int** | **int**
```
```
- OwnedTicket: Connects a User to a MovieEvent by the user's tickets.
```
```
Field | Id | PurchaseDate | UserId | EventId
--- | --- | --- | --- | ---
**Data Type** | **int** | **datetime2** | **int** | **int**
```
```
- UserReelInteractions: Connects a User to a Reel by the user's interaction informations.
```
```
Field | Id | IsLiked | WatchPercentage | ViewedAt | WatchDurationSeconds | UserId | ReelId
--- | --- | --- | --- | --- | --- | --- | ---
**Data Type** | **int** | **bit** | **decimal** | **datetime2** | **decimal** | **int** | **int**
```
```
- UserMoviePreferences: Connects a User to a Movie by the user's preferences.
```
```
Field | Id | Score | LastModified | ChangeFromPreviousValue | UserId | MovieId | ReelId
--- | --- | --- | --- | --- | --- | --- | ---
**Data Type** | **int** | **decimal** | **datetime2** | **int** | **int** | **int** | **int**
```
```
- MovieReviews: Connects a User to a Movie by the user's reviews of the Movie.
```
```
Field | Id | StarRating | Comment | CreatedAt | MovieId | UserId
--- | --- | --- | --- | --- | --- | ---
**Data Type** | **int** | **decimal** | **nvarchar** | **datetime2** | **int** | **int**
```
```
