namespace MovieApp.Features.ReelsUpload.Models
{
    /// <summary>
    /// The request to upload a reel.
    /// </summary>
    public class ReelUploadRequest
    {
        public string LocalFilePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public int UploaderUserId { get; set; }
        public int? MovieId { get; set; }
    }
}
