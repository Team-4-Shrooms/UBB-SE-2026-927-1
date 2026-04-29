using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MovieApp.DataLayer.Models;
using MovieApp.Features.Marketplace.ViewModels;
using System;

namespace MovieApp.Features.Marketplace.Views
{
    public sealed partial class MarketplacePage : Page
    {
        public MarketplaceViewModel ViewModel { get; } = App.Services.GetRequiredService<MarketplaceViewModel>();

        public MarketplacePage()
        {
            this.InitializeComponent();
            ViewModel.LoadData();
        }

        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryFilter?.SelectedItem is ComboBoxItem selectedItem)
            {
                string categoryContent = selectedItem.Content.ToString() ?? "";
                ViewModel.FilterByCategory(categoryContent == "All" ? null : categoryContent);
            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Equipment selectedItem)
            {
                Frame.Navigate(typeof(EquipmentDetailPage), selectedItem);
            }
        }

        private void GoToSell_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SellPage));
        }
    }
}
