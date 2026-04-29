\# Unified MovieApp Smoke-Test Checklist

\*\*April 29, 2026\*\*



\## Contents

1\. \[Introduction](#1-introduction)

2\. \[Navigation \& Shell Verification](#2-navigation--shell-verification)

3\. \[Core Functional Flow (Real Data)](#3-core-functional-flow-real-data)

4\. \[Content Creation \& Management](#4-content-creation--management)

5\. \[Technical Standards \& Stability](#5-technical-standards--stability)

6\. \[Conclusion](#6-conclusion)

7\. \[References](#references)



\---



\## 1 Introduction

This document serves as a comprehensive smoke-test checklist for verifying the successful integration of the PureCaffeine features into the unified MovieApp architecture. The migration aims at consolidating all functionalities to operate seamlessly against a shared Entity Framework Core (EF Core) database, delivering a consistent user experience and data integrity.



The checklist is structured to validate navigation and shell integration, core functional flows with real data, content creation and management capabilities, as well as adherence to technical standards and application stability. Each section contains detailed verification points to ensure critical aspects of the migration have been thoroughly tested and confirmed.



\## 2 Navigation \& Shell Verification

This section focuses on ensuring that the unified MovieApp shell correctly incorporates and presents all PureCaffeine features through the NavigationView sidebar, while maintaining a single-window application constraint. It also verifies the removal of obsolete legacy navigation components.



\### Verification Items

\- \[ ] \*\*Sidebar Reachability:\*\* Confirm that all 7 PureCaffeine features appear in the NavigationView sidebar and their respective pages load correctly. The features include: Reels Upload, Trailer Scraping, Reels Editing, Movie Swipe, Movie Tournament, Personality Match, Reels Feed.

\- \[ ] \*\*Single Window Constraint:\*\* Validate that selecting any navigation item loads the feature within the main application frame, ensuring no new windows open separately.

\- \[ ] \*\*Legacy Cleanup:\*\* Confirm removal of the old PureCaffeine `MainWindow.xaml` navigation in favor of the unified shell to prevent conflicting navigation paths.



\## 3 Core Functional Flow (Real Data)

This section verifies that all core features operate correctly using live data sourced from the unified Movies database rather than isolated or mocked datasets. It also confirms that user interactions and preferences persist accurately.



\### Verification Items

\- \[ ] \*\*Unified Movie Data:\*\* Confirm all movie titles, posters, and metadata displayed by features are sourced from the central Movies table, ensuring consistency across the app.

\- \[ ] \*\*Reels Feed:\*\* Open the reels feed; verify videos load and play smoothly. Confirm that the UI updates `IsLiked` status and `LikeCount` in real time upon user interaction.

\- \[ ] \*\*Movie Swipe:\*\* Perform multiple swipes (Right for Like, Left for Skip). Ensure these preferences are recorded in the `UserMoviePreference` database table accurately.

\- \[ ] \*\*Movie Tournament:\*\* Execute a tournament cycle (Setup, Match, Winner selection). Confirm the champion movie receives a preference score increase in the database as expected.

\- \[ ] \*\*Personality Match:\*\* Load the match list and verify compatibility scores are computed using actual swiped data through cosine similarity algorithms.

\- \[ ] \*\*User Details:\*\* Navigate to a `MatchedUserDetail` page and confirm the correct display of matched user profile statistics and their top movie preferences.



\## 4 Content Creation \& Management

This section ensures that content creation and editing features function properly, including persistence of new data and updates in the unified database schema.



\### Verification Items

\- \[ ] \*\*Reels Upload:\*\* Upload a local MP4 file, link it to a movie from the unified database via autocomplete functionality, and verify that the new reel entity persists in the database.

\- \[ ] \*\*Trailer Scraping:\*\* Search for a movie title and run a scrape job. Confirm that job logs appear in the UI and that new video reels are generated accordingly.

\- \[ ] \*\*Reels Editing:\*\* Select a reel from the user's gallery, apply cropping edits, and verify that the `CropDataJson` field updates correctly in the database.



\## 5 Technical Standards \& Stability

This section addresses adherence to coding standards, dependency injection correctness, and database integrity to ensure system robustness and maintainability.



\### Verification Items

\- \[ ] \*\*Dependency Injection:\*\* Confirm that the application launches without `System.InvalidOperationException`, indicating all Task 8 services and ViewModels are properly registered in `App.xaml.cs`.

\- \[ ] \*\*Rule 11 (Explicit Typing):\*\* Verify that `var` keywords have been replaced with explicit types in all relevant directories: `Features/MovieSwipe`, `Features/ReelsFeed`, `Features/ReelsEditing`, `Features/ReelsUpload`, and `Features/TrailerScraping`.

\- \[ ] \*\*Database Integrity:\*\* Validate usage of navigation properties (e.g., `Reel.Movie`, `Reel.CreatorUser`) instead of foreign key IDs in all data displays within these features.



\## 6 Conclusion

The migration of PureCaffeine features into the unified MovieApp environment demands rigorous verification to guarantee functional completeness, data consistency, and technical soundness. This checklist provides a structured approach to validate critical pathways and components, ensuring that all integrated features operate harmoniously on the shared EF Core database. 



Successful completion of these tests will confirm that the unified architecture not only fulfills the functional requirements but also adheres to best practices in software design and stability standards. It lays a solid foundation for further enhancements and maintenance within a consolidated framework.



\## References

\* Microsoft Documentation: Entity Framework Core - https://learn.microsoft.com/en-us/ef/core/

\* Microsoft Docs: NavigationView Control - https://learn.microsoft.com/en-us/windows/apps/design/controls/navigationview

\* Official C# Coding Conventions - https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

\* MVVM Pattern Guidance - https://learn.microsoft.com/en-us/windows/communitytoolkit/mvvm/introduction

\* EF Core Relationship Navigation - https://learn.microsoft.com/en-us/ef/core/modeling/relationships

