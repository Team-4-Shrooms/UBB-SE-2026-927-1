using System.ComponentModel.DataAnnotations;

namespace MovieApp.Web.Models
{
    public class AddReviewForm
    {
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Please write a comment.")]
        public string Comment { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int StarRating { get; set; } = 5;
    }
}
