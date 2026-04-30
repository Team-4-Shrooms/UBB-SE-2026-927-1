using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MovieApp.Features.Marketplace.ViewModels;

namespace MovieApp.Features.Marketplace.Views
{
    public sealed partial class FlashSaleBanner : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(FlashSaleViewModel), typeof(FlashSaleBanner), new PropertyMetadata(null));

        public FlashSaleViewModel? ViewModel
        {
            get => (FlashSaleViewModel?)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public FlashSaleBanner()
        {
            InitializeComponent();
        }
    }
}
