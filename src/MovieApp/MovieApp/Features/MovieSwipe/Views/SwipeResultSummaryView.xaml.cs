using Microsoft.UI.Xaml.Controls;
using MovieApp.Features.MovieSwipe.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace MovieApp.Features.MovieSwipe.Views
{
    public sealed partial class SwipeResultSummaryView : Page
    {
        public MovieSwipeViewModel ViewModel { get; }

        public SwipeResultSummaryView()
        {
            this.InitializeComponent();
            ViewModel = Ioc.Default.GetRequiredService<MovieSwipeViewModel>();
            this.DataContext = ViewModel;
        }
    }
}
