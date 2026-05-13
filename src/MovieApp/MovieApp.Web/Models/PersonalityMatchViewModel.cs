using MovieApp.DataLayer.Models;
using System.Collections.Generic;

namespace MovieApp.Web.Models
{
    public class PersonalityMatchIndexViewModel
    {
        public int QuestionIndex { get; set; }
        public int TotalQuestions { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
    }

    public class PersonalityMatchResultViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public List<string> CommonPreferences { get; set; } = new();
        public double MatchScore { get; set; }
    }

    public class PersonalityMatchResultsViewModel
    {
        public List<PersonalityMatchResultViewModel> Matches { get; set; } = new();
    }
}
