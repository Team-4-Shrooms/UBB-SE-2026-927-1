# WebApi Endpoint Documentation

This folder contains the HTTP endpoints for the MovieApp WebApi. Each controller is a thin wrapper over one repository from the DataLayer project.

## Conventions

- Base route prefix: `api/...`.
- Success responses are returned as `200 OK` unless noted otherwise.
- Some write operations translate repository exceptions into `400 Bad Request`.
- Route parameters are strongly typed as integers unless the route says otherwise.
- Request bodies are bound directly to the model or to a small request record defined inside the controller.
- The `PersonalityMatch` routes use distinct path segments such as `all-preferences` and `current-preferences` to avoid route conflicts.

## Active Sales

Base route: `api/active-sales`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/current` | Returns the currently active sales ordered by ending time. | None | `200 OK` with the active sale list. |
| GET | `/best-discounts` | Returns the best discount percentage per movie id. | None | `200 OK` with a dictionary keyed by movie id. |

## Audio Library

Base route: `api/audio-library`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/tracks` | Returns all music tracks. | None | `200 OK` with the track list. |
| GET | `/tracks/{musicTrackId}` | Returns a single track by id. | `musicTrackId` route parameter | `200 OK` with the track result. |

## Events

Base route: `api/events`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/` | Returns all events. | None | `200 OK` with the event list. |
| GET | `/{eventId}` | Returns an event by id. | `eventId` route parameter | `200 OK` with the event result. |
| GET | `/movie/{movieId}` | Returns all events for a movie. | `movieId` route parameter | `200 OK` with the matching events. |
| POST | `/{eventId}/purchase` | Purchases one ticket for the given event and user. | Body: `PurchaseTicketRequest { userId }` | `200 OK` on success, `400 Bad Request` if the repository rejects the purchase. |

## Equipment

Base route: `api/equipment`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/available` | Returns the equipment that is currently available. | None | `200 OK` with the available equipment list. |
| POST | `/` | Lists a new equipment item for sale. | Body: `Equipment` | `200 OK` on success, `400 Bad Request` when the item is invalid. |
| POST | `/{equipmentId}/purchase` | Purchases an equipment item. | Body: `PurchaseEquipmentRequest { buyerId, price, address }` | `200 OK` on success, `400 Bad Request` if the purchase fails. |

## Interactions

Base route: `api/interactions`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| POST | `/` | Inserts a user/reel interaction record. | Body: `UserReelInteraction` | `200 OK` on success. |
| POST | `/users/{userId}/reels/{reelId}` | Inserts or updates the interaction for a user and reel. | Route parameters | `200 OK` on success. |
| PUT | `/users/{userId}/reels/{reelId}/like` | Toggles the like state for a reel interaction. | Route parameters | `200 OK` on success. |
| PUT | `/users/{userId}/reels/{reelId}/view` | Updates watch duration and watch percentage for a reel. | Body: `UpdateViewDataRequest { watchDurationSeconds, watchPercentage }` | `200 OK` on success. |
| GET | `/users/{userId}/reels/{reelId}` | Returns a user-specific interaction for a reel. | Route parameters | `200 OK` with the interaction result. |
| GET | `/reels/{reelId}/likes` | Returns the like count for a reel. | `reelId` route parameter | `200 OK` with the like count. |
| GET | `/reels/{reelId}/movie-id` | Returns the movie id associated with a reel. | `reelId` route parameter | `200 OK` with the movie id result. |

## Inventory

Base route: `api/inventory`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/users/{userId}/movies` | Returns owned movies for a user. | `userId` route parameter | `200 OK` with the owned movie list. |
| DELETE | `/users/{userId}/movies/{movieId}` | Removes an owned movie from a user. | Route parameters | `200 OK` on success. |
| GET | `/users/{userId}/tickets` | Returns owned event tickets for a user. | `userId` route parameter | `200 OK` with the ticket list. |
| DELETE | `/users/{userId}/tickets/{eventId}` | Removes an owned ticket from a user. | Route parameters | `200 OK` on success. |
| GET | `/users/{userId}/equipment` | Returns owned equipment for a user. | `userId` route parameter | `200 OK` with the equipment list. |

## Movies

Base route: `api/movies`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/` | Returns all movies. | None | `200 OK` with the movie list. |
| GET | `/{movieId}` | Returns a movie by id. | `movieId` route parameter | `200 OK` with the movie result. |
| GET | `/search` | Returns the top 10 movies whose names match the partial query. | Query: `partialMovieName` | `200 OK` with matching movies. |
| GET | `/{movieId}/owned/{userId}` | Checks whether the user owns the movie. | Route parameters | `200 OK` with a boolean result. |
| POST | `/{movieId}/purchase` | Purchases a movie for a user. | Body: `PurchaseMovieRequest { userId, finalPrice }` | `200 OK` on success, `400 Bad Request` if the purchase fails. |

## Movie Tournament

Base route: `api/movie-tournament`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/users/{userId}/pool-size` | Returns the size of the user's tournament pool. | `userId` route parameter | `200 OK` with the pool size. |
| GET | `/users/{userId}/pool` | Returns the tournament pool for a user. | Route parameter `userId`, query `poolSize` | `200 OK` with the pool contents. |
| POST | `/users/{userId}/movies/{movieId}/boost` | Boosts a movie score for tournament ranking. | Body: `BoostMovieScoreRequest { scoreBoost }` | `200 OK` on success. |

## Personality Match

Base route: `api/personality-match`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/users/{excludedUserId}/all-preferences` | Returns all preferences except the excluded user’s. | `excludedUserId` route parameter | `200 OK` with the preference list. |
| GET | `/users/{userId}/current-preferences` | Returns the current user's preferences. | `userId` route parameter | `200 OK` with the preference list. |
| GET | `/users/{userId}/profile` | Returns the user's profile for matching. | `userId` route parameter | `200 OK` with the profile result. |
| GET | `/users/{excludedUserId}/random-user-ids` | Returns random user ids excluding one user. | Route parameter `excludedUserId`, query `userIdsCount` | `200 OK` with the id list. |
| GET | `/users/{userId}/username` | Returns the username for a user. | `userId` route parameter | `200 OK` with the username result. |
| GET | `/users/{userId}/top-preferences` | Returns the user's top preferences with titles. | Route parameter `userId`, query `topMoviePreferencesCount` | `200 OK` with the top preference list. |

## Preferences

Base route: `api/preferences`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/users/{userId}/movies/{movieId}/exists` | Checks whether a preference already exists. | Route parameters | `200 OK` with a boolean result. |
| POST | `/` | Inserts a preference score for a user and movie. | Body: `InsertPreferenceRequest { userId, movieId, score }` | `200 OK` on success. |
| PUT | `/users/{userId}/movies/{movieId}/boost` | Boosts an existing preference score. | Body: `UpdatePreferenceRequest { boost }` | `200 OK` on success. |

## Profiles

Base route: `api/profiles`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/users/{userId}` | Returns the profile for a user. | `userId` route parameter | `200 OK` with the profile result. |
| POST | `/users/{userId}/build` | Builds a profile from a user's interactions. | `userId` route parameter | `200 OK` with the built profile result. |
| PUT | `/` | Inserts or updates a `UserProfile`. | Body: `UserProfile` | `200 OK` on success. |

## Recommendations

Base route: `api/recommendations`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/users/{userId}/has-preferences` | Checks whether a user has any preferences. | `userId` route parameter | `200 OK` with a boolean result. |
| GET | `/reels` | Returns all reels used for recommendations. | None | `200 OK` with the reel list. |
| GET | `/users/{userId}/preference-scores` | Returns preference scores for a user. | `userId` route parameter | `200 OK` with the score list. |
| GET | `/like-counts` | Returns like counts for all reels. | None | `200 OK` with the like-count list. |
| GET | `/likes/within/{days}` | Returns likes created within the given number of days. | `days` route parameter | `200 OK` with the filtered like data. |

## Reels

Base route: `api/reels`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/users/{userId}` | Returns all reels for a user. | `userId` route parameter | `200 OK` with the reel list. |
| GET | `/{reelId}` | Returns a reel by id. | `reelId` route parameter | `200 OK` with the reel result. |
| PUT | `/{reelId}` | Updates reel edit metadata. | Body: `UpdateReelEditsRequest { cropDataJson, backgroundMusicId, videoUrl }` | `200 OK` with the number of affected rows. |
| DELETE | `/{reelId}` | Deletes a reel. | `reelId` route parameter | `200 OK` on success. |

## Reviews

Base route: `api/reviews`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/movie/{movieId}` | Returns all reviews for a movie. | `movieId` route parameter | `200 OK` with the review list. |
| POST | `/` | Adds a new review. | Body: `AddReviewRequest { movieId, userId, starRating, comment }` | `200 OK` on success, `400 Bad Request` if the repository rejects the review. |
| GET | `/movie/{movieId}/count` | Returns the review count for a movie. | `movieId` route parameter | `200 OK` with the count. |
| POST | `/counts` | Returns review counts for multiple movie ids. | Body: `MovieIdsRequest { movieIds }` | `200 OK` with the counts. |
| GET | `/movie/{movieId}/buckets` | Returns star-rating buckets for a movie. | `movieId` route parameter | `200 OK` with the bucket data. |

## Scrape Jobs

Base route: `api/scrape-jobs`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| POST | `/` | Creates a scrape job. | Body: `ScrapeJob` | `200 OK` with the created job result. |
| PUT | `/` | Updates a scrape job. | Body: `ScrapeJob` | `200 OK` on success. |
| POST | `/logs` | Adds a scrape job log entry. | Body: `ScrapeJobLog` | `200 OK` on success. |
| GET | `/` | Returns all scrape jobs. | None | `200 OK` with the job list. |
| GET | `/{jobId}/logs` | Returns logs for a specific job. | `jobId` route parameter | `200 OK` with the log list. |
| GET | `/logs` | Returns all scrape job logs. | None | `200 OK` with the log list. |
| GET | `/dashboard-stats` | Returns dashboard statistics. | None | `200 OK` with the stats result. |
| GET | `/search-movies` | Searches movies by partial title. | Query: `partialName` | `200 OK` with the search results. |
| GET | `/movie-id` | Finds a movie id by title. | Query: `title` | `200 OK` with the movie id result. |
| GET | `/reel-exists` | Checks whether a reel already exists for a video URL. | Query: `videoUrl` | `200 OK` with a boolean result. |
| POST | `/reels` | Inserts a scraped reel. | Body: `Reel` | `200 OK` with the inserted reel result. |
| GET | `/movies` | Returns all movies available to scraping workflows. | None | `200 OK` with the movie list. |
| GET | `/reels` | Returns all reels available to scraping workflows. | None | `200 OK` with the reel list. |

## Transactions

Base route: `api/transactions`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| POST | `/` | Logs a transaction. | Body: `Transaction` | `200 OK` on success, `400 Bad Request` when the payload is invalid. |
| GET | `/users/{userId}` | Returns all transactions for a user. | `userId` route parameter | `200 OK` with the transaction list. |
| PUT | `/{transactionId}/status` | Updates a transaction status string. | Body: `UpdateTransactionStatusRequest { newStatus }` | `200 OK` on success. |

## Users

Base route: `api/users`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| GET | `/users/{userId}/balance` | Returns the balance for a user. | `userId` route parameter | `200 OK` with the balance value. |
| PUT | `/users/{userId}/balance` | Updates the balance for a user. | Body: `UpdateBalanceRequest { newBalance }` | `200 OK` on success. |

## Video Storage

Base route: `api/video-storage`

| Method | Route | Description | Request | Response |
| --- | --- | --- | --- | --- |
| POST | `/reels` | Inserts a reel into video storage. | Body: `Reel` | `200 OK` with the inserted reel result. |

## Example Responses

The following examples show the kind of payloads that appear in Swagger UI and are useful when wiring a client.

### Current sales

```json
[
	{
		"id": 1,
		"movie": {
			"id": 42,
			"title": "Inception"
		},
		"discountPercentage": 25,
		"startTime": "2026-04-28T00:00:00Z",
		"endTime": "2026-05-05T00:00:00Z"
	}
]
```

### Movie lookup

```json
{
	"id": 42,
	"title": "Inception",
	"price": 14.99,
	"genre": "Sci-Fi"
}
```

### Purchase movie request

```json
{
	"userId": 7,
	"finalPrice": 11.24
}
```

### Review list

```json
[
	{
		"movieId": 42,
		"userId": 7,
		"starRating": 5,
		"comment": "Strong story and pacing."
	}
]
```

### Update balance request

```json
{
	"newBalance": 129.5
}
```

### Scrape job result

```json
{
	"id": 101,
	"status": "Completed",
	"title": "Top sci-fi movies",
	"createdAt": "2026-04-28T12:30:00Z"
}
```

## Notes for Consumers

- Most controllers are thin pass-through layers and do not add extra validation beyond repository-level checks.
- The request body record names in this file match the controller-local records, which makes it easier to align client payloads with the implemented API.
- If the API surface changes, regenerate this document from the controllers so it stays in sync with the routes.