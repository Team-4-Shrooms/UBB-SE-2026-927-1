# Endpoint Plan

This folder will contain one endpoint file per repository from the DataLayer project.

## Step Plan

1. Keep each controller aligned to one repository.
2. Reuse the WebApi `AppDbContext` through the shared `IMovieAppDbContext` contract.
3. Register the concrete repository types in dependency injection.
4. Keep read routes simple and write routes explicit about their required inputs.
5. Translate repository exceptions into `400 Bad Request` responses where the repository already signals invalid operations.

## Repository Map

- ActiveSalesRepository: current sales and best discount per movie.
- AudioLibraryRepository: all tracks and track lookup.
- InteractionRepository: insert, upsert, like, view updates, and reel statistics.
- EventRepository: event listing, event lookup, and ticket purchase.
- EquipmentRepository: available equipment, listing, and purchase.
- MovieRepository: movie listing, lookup, ownership check, purchase, and search.
- InventoryRepository: owned movies, tickets, equipment, and removal operations.
- MovieTournamentRepository: pool size, pool retrieval, and score boosting.
- ProfileRepository: profile read, build, and upsert.
- PreferenceRepository: existence check, insert, and update.
- PersonalityMatchRepository: preference and profile retrieval for matching.
- RecommendationRepository: preference presence, reels, scores, and like activity.
- ReelRepository: user reels, reel lookup, edit updates, and deletion.
- ReviewRepository: review listing, creation, counts, and rating buckets.
- VideoStorageRepository: reel insertion into storage.
- UserRepository: balance read and update.
- TransactionRepository: transaction logging, per-user listing, and status updates.
- ScrapeJobRepository: job lifecycle, logs, stats, and media lookup helpers.

If the route grouping feels too coarse or too granular, these files can be split or merged later without changing the repository contracts.