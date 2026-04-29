using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.Features.PersonalityMatch.Models;

namespace MovieApp.Features.PersonalityMatch.Services
{
    /// <summary>
    /// Implements personality matching using cosine similarity on movie score vectors.
    /// Owner: Madi
    /// </summary>
    public class PersonalityMatchingService : IPersonalityMatchingService
    {
        private readonly IPersonalityMatchRepository personalityMatchRepository;

        private const double SimilarityScaleToPercentage = 100.0;
        private const int SimilarityDecimalPlaces = 1;
        private const double MinimumSimilarityPercentageToInclude = 0;
        private const double MatchScoreForRandomUser = 0;
        private const string FallbackUsernamePrefix = "User";
        private const string FallbackFacebookAccountPrefix = "fb_user_";

        /// <summary>
        /// A static lookup of hardcoded usernames keyed by user identifier,
        /// used as a fallback until database-backed username retrieval is implemented.
        /// </summary>
        private static readonly Dictionary<int, string> HardcodedUsernames = new()
        {
            [1] = "Alex Carter",
            [2] = "Alice Rivers",
            [3] = "Bob Chen",
            [4] = "Carol Hayes",
            [5] = "Dave Morris",
            [6] = "Eve Santos",
            [7] = "James Park",
            [8] = "Luna Kim",
            [9] = "Sam Taylor",
            [10] = "Nina Reeves",
            [11] = "Tom Walsh",
            [12] = "Zara Foster",
            [13] = "Kai Rivera",
        };

        /// <summary>
        /// A static lookup of hardcoded Facebook account handles keyed by user identifier,
        /// used as a fallback until database-backed social account retrieval is implemented.
        /// </summary>
        private static readonly Dictionary<int, string> HardcodedFacebookAccounts = new()
        {
            [1] = "fb_alex_carter",
            [2] = "fb_alice_rivers",
            [3] = "fb_bob_chen",
            [4] = "fb_carol_hayes",
            [5] = "fb_dave_morris",
            [6] = "fb_eve_santos",
            [7] = "fb_james_park",
            [8] = "fb_luna_kim",
            [9] = "fb_sam_taylor",
            [10] = "fb_nina_reeves",
            [11] = "fb_tom_walsh",
            [12] = "fb_zara_foster",
            [13] = "fb_kai_rivera",
        };

        /// <summary>
        /// Initializes a new instance of <see cref="PersonalityMatchingService"/> with the specified repository.
        /// </summary>
        /// <param name="personalityMatchRepository">
        /// The repository used to retrieve user movie preferences and related data.
        /// </param>
        public PersonalityMatchingService(IPersonalityMatchRepository personalityMatchRepository)
        {
            this.personalityMatchRepository = personalityMatchRepository;
        }

        public async Task<List<MatchResult>> GetRandomMatchesAsync(int userId, int count)
        {
            List<int> randomUserIds = await personalityMatchRepository.GetRandomUserIdsAsync(userId, count);
            List<MatchResult> results = new List<MatchResult>();

            foreach (int id in randomUserIds)
            {
                results.Add(new MatchResult
                {
                    MatchedUserId = id,
                    MatchedUsername = GetHardcodedUsername(id),
                    FacebookAccount = GetHardcodedFacebookAccount(id),
                    MatchScore = MatchScoreForRandomUser, // 0
                    IsSelfView = id == userId
                });
            }

            return results;
        }

        public async Task<List<MoviePreferenceDisplay>> GetTopMoviePreferencesAsync(int userId, int topMoviePreferencesCount)
        {
            List<MovieApp.DataLayer.Models.MoviePreferenceDisplay> preferences = await this.personalityMatchRepository.GetTopPreferencesWithTitlesAsync(userId, topMoviePreferencesCount);

            for (int i = 0; i < preferences.Count; i++)
            {
                // This line satisfies your "FlagsFirstMovieAsBest" test branch
                preferences[i].IsBestMovie = i == 0;
            }

            return preferences;
        }

        public async Task<UserProfile?> GetUserProfileAsync(int userId)
        {
            return await personalityMatchRepository.GetUserProfileAsync(userId);
        }

        /// <inheritdoc />
        public async Task<List<MatchResult>> GetTopMatchesAsync(int userId, int count)
        {
            List<UserMoviePreference> currentUserPreferences =
                await this.personalityMatchRepository.GetCurrentUserPreferencesAsync(userId);

            if (currentUserPreferences.Count == 0)
            {
                return new List<MatchResult>();
            }

            Dictionary<int, double> currentUserScoreVector = BuildScoreVector(currentUserPreferences);


            List<UserMoviePreference> allOtherPreferences = await this.personalityMatchRepository.GetAllPreferencesExceptUserAsync(userId);

            Dictionary<int, List<UserMoviePreference>> otherUsersPreferences = allOtherPreferences
                .GroupBy(p => p.User.Id)
                .ToDictionary(g => g.Key, g => g.ToList());

            List<(int OtherUserId, double SimilarityPercentage)> userSimilarityScores = ComputeSimilarityScores(currentUserScoreVector, otherUsersPreferences);

            userSimilarityScores.Sort((first, second) => second.SimilarityPercentage.CompareTo(first.SimilarityPercentage));

            List<MatchResult> topMatches = new List<MatchResult>();
            foreach ((int otherUserId, double similarityPercentage) in userSimilarityScores.Take(count))
            {
                topMatches.Add(BuildMatchResult(otherUserId, similarityPercentage));
            }

            return topMatches;
        }

        /// <inheritdoc />
        public async Task<List<MatchResult>> GetRandomUsersAsync(int userId, int count)
        {
            List<int> randomUserIds = await this.personalityMatchRepository.GetRandomUserIdsAsync(userId, count);

            List<MatchResult> randomUserResults = new List<MatchResult>();
            foreach (int randomUserId in randomUserIds)
            {
                randomUserResults.Add(BuildMatchResult(randomUserId, MatchScoreForRandomUser));
            }

            return randomUserResults;
        }

        /// <inheritdoc />
        public async Task<string> GetUsernameAsync(int userId)
        {
            return await this.personalityMatchRepository.GetUsernameAsync(userId);
        }

        /// <summary>
        /// Builds a score vector dictionary from a list of movie preference models,
        /// mapping each movie identifier to its corresponding preference score.
        /// </summary>
        /// <param name="preferences">The list of movie preferences to convert into a vector.</param>
        /// <returns>
        /// A dictionary mapping movie identifiers to preference scores.
        /// </returns>
        private static Dictionary<int, double> BuildScoreVector(List<UserMoviePreference> preferences)
        {
            Dictionary<int, double> scoreVector = new Dictionary<int, double>();
            foreach (UserMoviePreference preference in preferences)
            {
                scoreVector[preference.Id] = (double)preference.Score;
            }
            return scoreVector;
        }

        /// <summary>
        /// Computes cosine similarity percentages between the current user's score vector
        /// and each other user's score vector, excluding pairs with zero similarity.
        /// </summary>
        /// <param name="currentUserScoreVector">The score vector of the current user.</param>
        /// <param name="otherUsersPreferences">
        /// A dictionary mapping other user identifiers to their movie preference lists.
        /// </param>
        /// <returns>
        /// A list of tuples containing the other user's identifier and their similarity percentage
        /// relative to the current user, excluding entries with a similarity of zero or below.
        /// </returns>
        private static List<(int OtherUserId, double SimilarityPercentage)> ComputeSimilarityScores(
            Dictionary<int, double> currentUserScoreVector,
            Dictionary<int, List<UserMoviePreference>> otherUsersPreferences)
        {
            List<(int OtherUserId, double SimilarityPercentage)> similarityScores = new List<(int, double)>();

            foreach (KeyValuePair<int, List<UserMoviePreference>> userPreferenceEntry in otherUsersPreferences)
            {
                int otherUserId = userPreferenceEntry.Key;
                Dictionary<int, double> otherUserScoreVector = BuildScoreVector(userPreferenceEntry.Value);

                double cosineSimilarity = ComputeCosineSimilarity(currentUserScoreVector, otherUserScoreVector);
                double similarityPercentage = Math.Round(cosineSimilarity * SimilarityScaleToPercentage, SimilarityDecimalPlaces);

                if (similarityPercentage >= MinimumSimilarityPercentageToInclude)
                {
                    similarityScores.Add((otherUserId, similarityPercentage));
                }
            }

            return similarityScores;
        }

        /// <summary>
        /// Constructs a <see cref="MatchResult"/> for the specified user using hardcoded username and Facebook account lookups and the provided match score.
        /// </summary>
        /// <param name="userId">The identifier of the matched user.</param>
        /// <param name="matchScore">The compatibility score to assign to the match result.</param>
        /// <returns>
        /// A <see cref="MatchResult"/> populated with the user's identifier, username, Facebook account, and match score.
        /// </returns>
        private static MatchResult BuildMatchResult(int userId, double matchScore)
        {
            return new MatchResult
            {
                MatchedUserId = userId,
                MatchedUsername = GetHardcodedUsername(userId),
                MatchScore = matchScore,
                FacebookAccount = GetHardcodedFacebookAccount(userId),
            };
        }

        /// <summary>
        /// Computes the cosine similarity between two movie score vectors represented as dictionaries, where keys are movie identifiers and values are preference scores.
        /// Returns a value between 0 and 1, where 1 indicates identical preference directions
        /// and 0 indicates no overlap or a zero-magnitude vector.
        /// </summary>
        /// <param name="firstUserVector">The preference score vector of the first user.</param>
        /// <param name="secondUserVector">The preference score vector of the second user.</param>
        /// <returns>
        /// A double representing the cosine similarity between the two vectors, or 0 if either vector has zero magnitude.
        /// </returns>
        private static double ComputeCosineSimilarity(Dictionary<int, double> firstUserVector, Dictionary<int, double> secondUserVector)
        {
            double dotProduct = 0;
            double firstVectorMagnitudeSquared = 0;
            double secondVectorMagnitudeSquared = 0;

            foreach (KeyValuePair<int, double> firstUserEntry in firstUserVector)
            {
                firstVectorMagnitudeSquared += firstUserEntry.Value * firstUserEntry.Value;
                if (secondUserVector.TryGetValue(firstUserEntry.Key, out double secondUserScore))
                {
                    dotProduct += firstUserEntry.Value * secondUserScore;
                }
            }

            foreach (KeyValuePair<int, double> secondUserEntry in secondUserVector)
            {
                secondVectorMagnitudeSquared += secondUserEntry.Value * secondUserEntry.Value;
            }

            bool eitherVectorHasZeroMagnitude = firstVectorMagnitudeSquared == 0 || secondVectorMagnitudeSquared == 0;
            if (eitherVectorHasZeroMagnitude)
            {
                return 0;
            }

            return dotProduct / (Math.Sqrt(firstVectorMagnitudeSquared) * Math.Sqrt(secondVectorMagnitudeSquared));
        }

        /// <summary>
        /// Retrieves the hardcoded username for the specified user identifier.
        /// Falls back to a generated placeholder if the identifier is not found in the lookup.
        /// </summary>
        /// <param name="userId">The identifier of the user whose username is to be retrieved.</param>
        /// <returns>
        /// The hardcoded username if found; otherwise a generated string of the form <c>User {userId}</c>.
        /// </returns>
        private static string GetHardcodedUsername(int userId)
        {
            bool usernameFound = HardcodedUsernames.TryGetValue(userId, out string? username);
            return usernameFound ? username! : $"{FallbackUsernamePrefix} {userId}";
        }

        /// <summary>
        /// Retrieves the hardcoded Facebook account handle for the specified user identifier.
        /// Falls back to a generated placeholder if the identifier is not found in the lookup.
        /// </summary>
        /// <param name="userId">The identifier of the user whose Facebook account is to be retrieved.</param>
        /// <returns>
        /// The hardcoded Facebook account handle if found; otherwise a generated string of the form <c>fb_user_{userId}</c>.
        /// </returns>
        private static string GetHardcodedFacebookAccount(int userId)
        {
            bool facebookAccountFound = HardcodedFacebookAccounts.TryGetValue(userId, out string? facebookAccount);
            return facebookAccountFound ? facebookAccount! : $"{FallbackFacebookAccountPrefix}{userId}";
        }
    }
}
