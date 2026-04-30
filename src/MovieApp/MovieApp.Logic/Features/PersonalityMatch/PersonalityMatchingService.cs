using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;

namespace MovieApp.Logic.Features.PersonalityMatch
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

        private static readonly Dictionary<int, string> HardcodedUsernames = new()
        {
            [1] = "Alex Carter", [2] = "Alice Rivers", [3] = "Bob Chen", [4] = "Carol Hayes",
            [5] = "Dave Morris", [6] = "Eve Santos", [7] = "James Park", [8] = "Luna Kim",
            [9] = "Sam Taylor", [10] = "Nina Reeves", [11] = "Tom Walsh", [12] = "Zara Foster",
            [13] = "Kai Rivera",
        };

        private static readonly Dictionary<int, string> HardcodedFacebookAccounts = new()
        {
            [1] = "fb_alex_carter", [2] = "fb_alice_rivers", [3] = "fb_bob_chen", [4] = "fb_carol_hayes",
            [5] = "fb_dave_morris", [6] = "fb_eve_santos", [7] = "fb_james_park", [8] = "fb_luna_kim",
            [9] = "fb_sam_taylor", [10] = "fb_nina_reeves", [11] = "fb_tom_walsh", [12] = "fb_zara_foster",
            [13] = "fb_kai_rivera",
        };

        public PersonalityMatchingService(IPersonalityMatchRepository personalityMatchRepository)
        {
            this.personalityMatchRepository = personalityMatchRepository;
        }

        public async Task<List<MatchResult>> GetTopMatchesAsync(int userId, int count)
        {
            List<UserMoviePreference> currentUserPreferences =
                await this.personalityMatchRepository.GetCurrentUserPreferencesAsync(userId);

            if (currentUserPreferences.Count == 0) return new List<MatchResult>();

            Dictionary<int, double> currentUserScoreVector = BuildScoreVector(currentUserPreferences);
            List<UserMoviePreference> allOtherPreferences = await this.personalityMatchRepository.GetAllPreferencesExceptUserAsync(userId);

            Dictionary<int, List<UserMoviePreference>> otherUsersPreferences = allOtherPreferences
                .GroupBy(p => p.User.Id)
                .ToDictionary(g => g.Key, g => g.ToList());

            List<(int OtherUserId, double SimilarityPercentage)> userSimilarityScores =
                ComputeSimilarityScores(currentUserScoreVector, otherUsersPreferences);

            userSimilarityScores.Sort((first, second) => second.SimilarityPercentage.CompareTo(first.SimilarityPercentage));

            List<MatchResult> topMatches = new List<MatchResult>();
            foreach ((int otherUserId, double similarityPercentage) in userSimilarityScores.Take(count))
            {
                topMatches.Add(BuildMatchResult(otherUserId, similarityPercentage));
            }

            return topMatches;
        }

        public async Task<List<MatchResult>> GetRandomUsersAsync(int userId, int count)
        {
            List<int> randomUserIds = await this.personalityMatchRepository.GetRandomUserIdsAsync(userId, count);
            List<MatchResult> results = new List<MatchResult>();
            foreach (int randomUserId in randomUserIds)
            {
                results.Add(BuildMatchResult(randomUserId, MatchScoreForRandomUser));
            }
            return results;
        }

        public async Task<UserProfile?> GetUserProfileAsync(int userId)
        {
            return await personalityMatchRepository.GetUserProfileAsync(userId);
        }

        public async Task<List<MoviePreferenceDisplay>> GetTopMoviePreferencesAsync(int userId, int topMoviePreferencesCount)
        {
            List<MoviePreferenceDisplay> preferences = await this.personalityMatchRepository.GetTopPreferencesWithTitlesAsync(userId, topMoviePreferencesCount);
            for (int i = 0; i < preferences.Count; i++)
            {
                preferences[i].IsBestMovie = i == 0;
            }
            return preferences;
        }

        public async Task<string> GetUsernameAsync(int userId)
        {
            return await this.personalityMatchRepository.GetUsernameAsync(userId);
        }

        private static Dictionary<int, double> BuildScoreVector(List<UserMoviePreference> preferences)
        {
            Dictionary<int, double> scoreVector = new Dictionary<int, double>();
            foreach (UserMoviePreference preference in preferences)
            {
                scoreVector[preference.Id] = (double)preference.Score;
            }
            return scoreVector;
        }

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

            if (firstVectorMagnitudeSquared == 0 || secondVectorMagnitudeSquared == 0) return 0;

            return dotProduct / (Math.Sqrt(firstVectorMagnitudeSquared) * Math.Sqrt(secondVectorMagnitudeSquared));
        }

        private static string GetHardcodedUsername(int userId)
        {
            bool usernameFound = HardcodedUsernames.TryGetValue(userId, out string? username);
            return usernameFound ? username! : $"{FallbackUsernamePrefix} {userId}";
        }

        private static string GetHardcodedFacebookAccount(int userId)
        {
            bool facebookAccountFound = HardcodedFacebookAccounts.TryGetValue(userId, out string? facebookAccount);
            return facebookAccountFound ? facebookAccount! : $"{FallbackFacebookAccountPrefix}{userId}";
        }
    }
}
