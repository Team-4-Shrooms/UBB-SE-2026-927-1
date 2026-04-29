namespace MovieApp.Features.ReelsEditing.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using MovieApp.Logic.Models;
    using MovieApp.Features.ReelsEditing.Services;

    /// <summary>
    /// ViewModel for the music selection dialog.
    /// </summary>
    public partial class MusicSelectionDialogViewModel : ObservableObject
    {
        private readonly IAudioLibraryRepository audioLibrary;

        [ObservableProperty]
        private ObservableCollection<MusicTrack> availableTracks = new ();

        [ObservableProperty]
        private MusicTrack? selectedTrack;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicSelectionDialogViewModel"/> class.
        /// </summary>
        /// <param name="audioLibrary">The audio library service used to fetch tracks.</param>
        public MusicSelectionDialogViewModel(IAudioLibraryRepository audioLibrary)
        {
            this.audioLibrary = audioLibrary;
        }

        /// <summary>
        /// Loads the available music tracks from the audio library asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [RelayCommand]
        private async Task LoadTracksAsync()
        {
            IList<MusicTrack> tracks = await this.audioLibrary.GetAllTracksAsync();
            this.AvailableTracks.Clear();

            foreach (MusicTrack musicTrack in tracks)
            {
                this.AvailableTracks.Add(musicTrack);
            }
        }

        /// <summary>
        /// Selects a specific music track.
        /// </summary>
        /// <param name="track">The music track to select.</param>
        [RelayCommand]
        private void SelectTrack(MusicTrack track)
        {
            this.SelectedTrack = track;
        }
    }
}
