using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using MovieApp.DataLayer.Models;
using MovieApp.Features.ReelsUpload.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace MovieApp.Features.ReelsUpload.Views
{
    /// <summary>
    /// Code-behind for the Reels Upload page.
    /// </summary>
    public sealed partial class ReelsUploadPage : Page
    {
        public ReelsUploadViewModel ViewModel { get; }

        public ReelsUploadPage()
        {
            ViewModel = Ioc.Default.GetRequiredService<ReelsUploadViewModel>();
            this.InitializeComponent();
        }

        private void MovieAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only trigger search if user typed it (not if we programmatically populated it)
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ViewModel.SearchMovieCommand.Execute(sender.Text);
            }
        }

        private void MovieAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is Movie pickedMovie)
            {
                // Update the text box to perfectly match the chosen name
                sender.Text = pickedMovie.Title;
                ViewModel.SelectMovieCommand.Execute(pickedMovie);
            }
        }
    }
}
