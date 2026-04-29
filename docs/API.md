# MovieApp WebApi Reference

This document reflects the current controller surface in `MovieApp.WebApi`.

## Conventions

- All routes are rooted under `api/...`.
- Query parameters are named exactly as they appear in the controller signatures.
- Request bodies use the DTO classes defined in `MovieApp.WebApi.DTOs`.
- JSON serialization uses the usual ASP.NET Core camelCase shape.
- Single-item lookups currently return `200 OK` with `null` when the repository does not find a record; the controllers do not translate these cases into `404 Not Found`.
- Write endpoints (POST/PUT that produce no data) return `200 OK` with an empty body.

## Active Sales

Base route: `api/active-sales`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/current` | None | `200 OK` with an array of `ActiveSaleDto`. |

## Audio Library

Base route: `api/audio-library`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/tracks` | None | `200 OK` with an array of `MusicTrackDto`. |
| GET | `/tracks/{musicTrackId}` | `musicTrackId` route parameter | `200 OK` with `MusicTrackDto?`. |

## Equipment

Base route: `api/equipment`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/available` | None | `200 OK` with an array of `EquipmentDto`. |
| GET | `/{id}` | `id` route parameter | `200 OK` with `EquipmentDto?`. |
| POST | `/` | `EquipmentListItemRequestBody` | `200 OK` on success. |

## Events

Base route: `api/events`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/` | None | `200 OK` with an array of `MovieEventDto`. |
| GET | `/{eventId}` | `eventId` route parameter | `200 OK` with `MovieEventDto?`. |
| GET | `/movie/{movieId}` | `movieId` route parameter | `200 OK` with an array of `MovieEventDto`. |
| GET | `/{eventId}/tickets/{userId}` | `eventId`, `userId` route parameters | `200 OK` with `bool` indicating whether the user has a ticket. |
| POST | `/tickets` | `AddOwnedTicketRequestBody` | `200 OK` on success. |

## Interactions

Base route: `api/interactions`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| POST | `/` | `InsertInteractionRequestBody` | `200 OK` on success. |
| POST | `/users/{userId}/reels/{reelId}` | `userId`, `reelId` route parameters | `200 OK` on success. |
| PUT | `/users/{userId}/reels/{reelId}/like` | `userId`, `reelId` route parameters | `200 OK` on success. |
| PUT | `/users/{userId}/reels/{reelId}/view` | `userId`, `reelId` route parameters and `UpdateViewDataRequestBody` | `200 OK` on success. |
| GET | `/users/{userId}/reels/{reelId}` | `userId`, `reelId` route parameters | `200 OK` with `UserReelInteractionDto?`. |
| GET | `/users/{userId}` | `userId` route parameter | `200 OK` with an array of `UserReelInteractionDto`. |
| GET | `/reels/{reelId}/likes` | `reelId` route parameter | `200 OK` with an integer like count. |
| GET | `/reels/{reelId}/movie-id` | `reelId` route parameter | `200 OK` with `int?`. |

## Inventory

Base route: `api/inventory`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/users/{userId}/movies` | `userId` route parameter | `200 OK` with an array of `OwnedMovieDto`. |
| GET | `/users/{userId}/movies/{movieId}/ownerships` | `userId`, `movieId` route parameters | `200 OK` with an array of `OwnedMovieDto`. |
| POST | `/movies/ownerships/remove` | JSON array of ownership IDs (`int[]`) | `200 OK` on success. |
| GET | `/users/{userId}/events/{eventId}/tickets` | `userId`, `eventId` route parameters | `200 OK` with an array of `OwnedTicketDto`. |
| POST | `/events/tickets/remove` | JSON array of ticket ownership IDs (`int[]`) | `200 OK` on success. |
| POST | `/ownedmovies` | `AddOwnedMovieRequestBody` | `200 OK` on success. |

## Movies

Base route: `api/movies`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/{movieId}` | `movieId` route parameter | `200 OK` with `MovieDto`, or `404 Not Found` if not found. |
| GET | `/search?partialMovieName=...` | Optional `partialMovieName` query parameter | `200 OK` with an array of `MovieDto` (max 10). |
| GET | `/{movieId}/owned/{userId}` | `movieId` and `userId` route parameters | `200 OK` with `bool`. |

## Movie Tournament

Base route: `api/movie-tournament`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/users/{userId}/pool-size` | `userId` route parameter | `200 OK` with an integer pool size. |
| GET | `/users/{userId}/pool?poolSize=...` | `userId` route parameter and required `poolSize` query parameter | `200 OK` with an array of `MovieDto`. |
| POST | `/users/{userId}/movies/{movieId}/boost` | `userId`, `movieId` route parameters and `BoostMovieScoreRequestBody` | `200 OK` on success. |

## Personality Match

Base route: `api/personality-match`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/users/{userId}/current-preferences` | `userId` route parameter | `200 OK` with an array of `UserMoviePreferenceDto`. |
| GET | `/users/{userId}/profile` | `userId` route parameter | `200 OK` with `UserProfileDto?`. |
| GET | `/users/{excludedUserId}/random-user-ids?userIdsCount=...` | `excludedUserId` route parameter and required `userIdsCount` query parameter | `200 OK` with an array of integers. |
| GET | `/users/{excludedUserId}/others-preferences` | `excludedUserId` route parameter | `200 OK` with an array of `UserMoviePreferenceDto` for all users except the excluded one. |

## Preferences

Base route: `api/preferences`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/users/{userId}/movies/{movieId}/exists` | `userId`, `movieId` route parameters | `200 OK` with `bool`. |
| POST | `/` | `InsertPreferenceRequestBody` | `200 OK` on success. |
| PUT | `/users/{userId}/movies/{movieId}/boost` | `userId`, `movieId` route parameters and `UpdatePreferenceRequestBody` | `200 OK` on success. |

## Profiles

Base route: `api/profiles`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/users/{userId}` | `userId` route parameter | `200 OK` with `UserProfileDto?`. |
| POST | `/` | `UpsertProfileRequestBody` | `200 OK` on success. |

## Recommendations

Base route: `api/recommendations`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/users/{userId}/has-preferences` | `userId` route parameter | `200 OK` with `bool`. |
| GET | `/reels` | None | `200 OK` with an array of `ReelDto`. |
| GET | `/users/{userId}/preference-scores` | `userId` route parameter | `200 OK` with a `Dictionary<int, decimal>` serialized as a JSON object keyed by movie id. |
| GET | `/like-counts` | None | `200 OK` with a `Dictionary<int, int>` serialized as a JSON object keyed by reel id. |
| GET | `/likes/within/{days}` | `days` route parameter | `200 OK` with an array of `UserReelInteractionDto`. |

## Reels

Base route: `api/reels`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/users/{userId}` | `userId` route parameter | `200 OK` with an array of `ReelDto`. |
| GET | `/{reelId}` | `reelId` route parameter | `200 OK` with `ReelDto?`. |
| PUT | `/{reelId}` | `reelId` route parameter and `UpdateReelEditsRequestBody` | `200 OK` with the number of affected rows. |
| DELETE | `/{reelId}` | `reelId` route parameter | `200 OK` on success. |

## Reviews

Base route: `api/reviews`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/movie/{movieId}` | `movieId` route parameter | `200 OK` with an array of `MovieReviewDto`. |
| GET | `/movie/{movieId}/ratings` | `movieId` route parameter | `200 OK` with an array of `decimal` raw star ratings. |
| POST | `/` | `AddReviewRequestBody` | `200 OK` on success. |
| POST | `/counts` | `GetReviewCountsRequestBody` | `200 OK` with a `Dictionary<int, int>` serialized as a JSON object keyed by movie id. |

## Scrape Jobs

Base route: `api/scrape-jobs`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| POST | `/` | `ScrapeJobRequestBody` | `200 OK` with `ScrapeJobDto`. |
| PUT | `/{jobId}` | `jobId` route parameter and `ScrapeJobRequestBody` | `200 OK` on success. |
| POST | `/logs` | `AddLogEntryRequestBody` | `200 OK` on success. |
| GET | `/` | None | `200 OK` with an array of `ScrapeJobDto`. |
| GET | `/{jobId}/logs` | `jobId` route parameter | `200 OK` with an array of `ScrapeJobLogDto`. |
| GET | `/logs` | None | `200 OK` with an array of `ScrapeJobLogDto`. |
| GET | `/dashboard-stats` | None | `200 OK` with `DashboardStatsDto`. |
| GET | `/search-movies?partialName=...` | Required `partialName` query parameter | `200 OK` with an array of `MovieDto`. |
| GET | `/movie-id?title=...` | Required `title` query parameter | `200 OK` with `int?`. |
| GET | `/reel-exists?videoUrl=...` | Required `videoUrl` query parameter | `200 OK` with `bool`. |
| POST | `/reels` | `InsertReelRequestBody` | `200 OK` with `ReelDto`. |
| GET | `/movies` | None | `200 OK` with an array of `MovieDto`. |
| GET | `/reels` | None | `200 OK` with an array of `ReelDto`. |

## Transactions

Base route: `api/transactions`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/users/{userId}` | `userId` route parameter | `200 OK` with an array of `TransactionDto`. |
| POST | `/` | `LogTransactionRequestBody` | `200 OK` on success. |
| PUT | `/{transactionId}/status` | `transactionId` route parameter and `UpdateTransactionStatusRequestBody` | `200 OK` on success. |

## Users

Base route: `api/users`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/{userId}` | `userId` route parameter | `200 OK` with `UserDto?`. |
| GET | `/{userId}/balance` | `userId` route parameter | `200 OK` with the user balance as a decimal. |
| PUT | `/{userId}/balance` | `userId` route parameter and `UpdateBalanceRequestBody` | `200 OK` on success. |

## Video Storage

Base route: `api/video-storage`

| Method | Route | Request | Response |
| --- | --- | --- | --- |
| POST | `/reels` | `InsertReelRequestBody` | `200 OK` with `ReelDto`. `400 Bad Request` if IDs are invalid. `404 Not Found` if entities are not found. |

## Request Body Schemas

These are the custom request DTOs used by the current endpoints.

| DTO | JSON fields |
| --- | --- |
| `EquipmentListItemRequestBody` | `title`, `category`, `description`, `condition`, `price`, `imageUrl`, `sellerId` |
| `AddOwnedMovieRequestBody` | `userId`, `movieId` |
| `AddOwnedTicketRequestBody` | `userId`, `eventId` |
| `InsertInteractionRequestBody` | `isLiked`, `watchDurationSeconds`, `watchPercentage`, `viewedAt`, `userId`, `reelId` |
| `UpdateViewDataRequestBody` | `watchDurationSeconds`, `watchPercentage` |
| `BoostMovieScoreRequestBody` | `scoreBoost` |
| `InsertPreferenceRequestBody` | `userId`, `movieId`, `score` |
| `UpdatePreferenceRequestBody` | `boost` |
| `UpsertProfileRequestBody` | `totalLikes`, `totalWatchTimeSeconds`, `averageWatchTimeSeconds`, `totalClipsViewed`, `likeToViewRatio`, `lastUpdated`, `userId` |
| `AddReviewRequestBody` | `movieId`, `userId`, `starRating`, `comment` |
| `UpdateReelEditsRequestBody` | `cropDataJson`, `backgroundMusicId`, `videoUrl` |
| `GetReviewCountsRequestBody` | `movieIds` |
| `LogTransactionRequestBody` | `amount`, `type`, `status`, `timestamp`, `shippingAddress`, `buyerId`, `sellerId`, `equipmentId`, `movieId`, `eventId` |
| `UpdateTransactionStatusRequestBody` | `newStatus` |
| `ScrapeJobRequestBody` | `searchQuery`, `maxResults`, `status`, `moviesFound`, `reelsCreated`, `startedAt`, `completedAt`, `errorMessage` |
| `AddLogEntryRequestBody` | `level`, `message`, `timestamp`, `scrapeJobId` |
| `InsertReelRequestBody` | `videoUrl`, `thumbnailUrl`, `title`, `caption`, `featureDurationSeconds`, `cropDataJson`, `backgroundMusicId`, `source`, `genre`, `createdAt`, `lastEditedAt`, `movieId`, `creatorUserId` |
| `UpdateBalanceRequestBody` | `newBalance` |

## Response DTO Schemas

Key response types beyond primitives and arrays.

| DTO | Fields |
| --- | --- |
| `UserDto` | `id`, `username`, `email`, `balance` |
| `OwnedMovieDto` | `id`, `purchaseDate`, `user` (`UserReferenceDto`), `movie` (`MovieReferenceDto`) |
| `OwnedTicketDto` | `id`, `purchaseDate`, `user` (`UserReferenceDto`), `event` (`MovieEventReferenceDto`) |

## Notes for Consumers

- The controllers are intentionally thin and mostly forward to repository methods.
- Several lookup endpoints return nullable DTOs or nullable primitive values instead of `404` responses (exception: `GET /api/movies/{movieId}` returns `404`).
- Write endpoints that have no meaningful return value respond with `200 OK` and an empty body.
- The remove inventory endpoints (`POST /api/inventory/movies/ownerships/remove` and `POST /api/inventory/events/tickets/remove`) accept a plain JSON array of integer IDs, not a wrapper object.
- If the route surface changes, regenerate this document from the controllers and DTOs so it stays aligned with the implemented API.
