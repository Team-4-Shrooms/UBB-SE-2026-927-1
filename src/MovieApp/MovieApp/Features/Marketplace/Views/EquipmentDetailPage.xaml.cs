using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Features.Shared.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MovieApp.Features.Marketplace.Views
{
    public sealed partial class EquipmentDetailPage : Page
    {
        private readonly IEquipmentService _equipmentService = App.Services.GetRequiredService<IEquipmentService>();
        private readonly IUserRepository _userRepo = App.Services.GetRequiredService<IUserRepository>();
        private Equipment? _selectedItem;

        public EquipmentDetailPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Equipment item)
            {
                _selectedItem = item;
                await PopulateUIAsync();
            }
        }

        private async Task PopulateUIAsync()
        {
            if (_selectedItem == null)
            {
                return;
            }

            TitleLabel.Text = _selectedItem.Title;
            DescriptionLabel.Text = _selectedItem.Description ?? "No description available.";
            CategoryLabel.Text = _selectedItem.Category;
            ConditionLabel.Text = _selectedItem.Condition;
            PriceLabel.Text = $"Price: ${_selectedItem.Price:F2}";

            if (!string.IsNullOrEmpty(_selectedItem.ImageUrl))
            {
                try
                {
                    ItemImage.Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(_selectedItem.ImageUrl));
                }
                catch (UriFormatException)
                {
                }
            }

            decimal balance = await _userRepo.GetBalanceAsync(SessionManager.CurrentUserID);
            bool canAfford = balance >= _selectedItem.Price;
            ConfirmBuyButton.IsEnabled = canAfford;
            ErrorText.Visibility = canAfford ? Visibility.Collapsed : Visibility.Visible;
            if (!canAfford)
            {
                ErrorText.Text = $"Insufficient funds. Balance: {balance:C} — Price: {_selectedItem.Price:C}";
            }
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            ShippingModal.Visibility = Visibility.Visible;
        }

        private void CancelShipping_Click(object sender, RoutedEventArgs e)
        {
            ShippingModal.Visibility = Visibility.Collapsed;
        }

        private async void ConfirmShipping_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedItem == null)
            {
                return;
            }

            ModalErrorText.Visibility = Visibility.Collapsed;
            string error = "";

            if (string.IsNullOrWhiteSpace(ModalNameInput.Text))
            {
                error += "- Name is required.\n";
            }

            if (ModalAddressInput.Text.Length < 10)
            {
                error += "- Address too short (min 10 chars).\n";
            }

            string phone = ModalPhoneInput.Text.Trim();
            if (phone.Length != 10 || !phone.All(char.IsDigit))
            {
                error += "- Phone must be exactly 10 digits.\n";
            }

            if (!string.IsNullOrEmpty(error))
            {
                ModalErrorText.Text = error;
                ModalErrorText.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                int userId = SessionManager.CurrentUserID;
                AppDbContext dbContext = App.Services.GetRequiredService<AppDbContext>();

                User? user = await dbContext.Users.FindAsync(userId)
                    ?? throw new Exception("User not found.");
                
                var equipment = await dbContext.Equipment
                    .FirstOrDefaultAsync(e => e.Id == _selectedItem.Id)
                    ?? throw new Exception("Equipment not found.");

                if (user.Balance < _selectedItem.Price)
                    throw new InvalidOperationException("Insufficient balance.");

                user.Balance -= _selectedItem.Price;
                equipment.Status = EquipmentStatus.Sold;

                var transaction = new Transaction
                {
                    Buyer = user,
                    Seller = equipment.Seller,
                    Equipment = equipment,
                    Amount = -_selectedItem.Price,
                    Type = "EquipmentPurchase",
                    Status = "Completed",
                    Timestamp = DateTime.UtcNow,
                    ShippingAddress = ModalAddressInput.Text
                };

                dbContext.Transactions.Add(transaction);
                await dbContext.SaveChangesAsync();

                SessionManager.CurrentUserBalance = user.Balance;

                ShippingModal.Visibility = Visibility.Collapsed;
 
                var dialog = new ContentDialog
                {
                    Title = "Purchase successful",
                    Content = $"\"{_selectedItem.Title}\" has been purchased and added to your inventory.",
                    PrimaryButtonText = "OK",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = XamlRoot
                };
                await dialog.ShowAsync();
 
                Frame.GoBack();
            }
            catch (InvalidOperationException ex)
            {
                ModalErrorText.Text = ex.Message;
                ModalErrorText.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ModalErrorText.Text = "Transaction failed: " + ex.Message;
                ModalErrorText.Visibility = Visibility.Visible;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) => Frame.GoBack();
    }
}
