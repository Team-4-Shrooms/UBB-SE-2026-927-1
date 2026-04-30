using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.Features.Wallet.ViewModels;

namespace MovieApp.Features.Wallet.Views
{
    public sealed partial class WalletPage : Page
    {
        public WalletViewModel ViewModel { get; } = App.Services.GetRequiredService<WalletViewModel>();

        public WalletPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadTransactionsAsync();
        }
    }
}
