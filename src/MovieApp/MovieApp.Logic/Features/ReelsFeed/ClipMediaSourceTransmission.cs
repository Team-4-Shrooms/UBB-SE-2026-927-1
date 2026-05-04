namespace MovieApp.Logic.Features.ReelsFeed
{
    /// <summary>
    /// DTO passed from playback service to presentation layer so media object
    /// creation can happen outside the service.
    /// </summary>
    public sealed class ClipMediaSourceTransmission
    {
        /// <summary>
        /// Gets or sets the clip URL.
        /// </summary>
        public string VideoUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this URL had been prefetched.
        /// </summary>
        public bool WasPrefetched { get; set; }
    }
}
