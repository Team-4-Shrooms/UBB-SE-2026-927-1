using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MovieApp.Features.Marketplace.ViewModels;
using System;

namespace MovieApp.Features.Marketplace.Views
{
    public sealed partial class SellPage : Page
    {
        public SellEquipmentViewModel ViewModel { get; } = App.Services.GetRequiredService<SellEquipmentViewModel>();

        public SellPage()
        {
            this.InitializeComponent();
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string? category = (CategoryInput.SelectedItem as ComboBoxItem)?.Content.ToString();
            string? condition = (ConditionInput.SelectedItem as ComboBoxItem)?.Content.ToString();
            string imageUrl = ImageUrlInput.Text;

            try
            {
                await ViewModel.SubmitListingAsync(category, condition, imageUrl);

                var dialog = new ContentDialog
                {
                    Title = "Listing Created",
                    Content = "Your equipment has been listed successfully.",
                    PrimaryButtonText = "OK",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = XamlRoot
                };
                await dialog.ShowAsync();

                Frame.GoBack();
            }
            catch (Exception exception)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = exception.Message,
                    PrimaryButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
