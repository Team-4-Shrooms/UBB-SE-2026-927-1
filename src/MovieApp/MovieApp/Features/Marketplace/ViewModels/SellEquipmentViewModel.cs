using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MovieApp.Features.Shared.Models;

namespace MovieApp.Features.Marketplace.ViewModels
{
    public class SellEquipmentViewModel : INotifyPropertyChanged
    {
        private readonly IEquipmentService _equipmentService = App.Services.GetRequiredService<IEquipmentService>();
        private readonly IUserRepository _userRepository = App.Services.GetRequiredService<IUserRepository>();

        private string _newItemTitle = string.Empty;
        private string _newItemDesc = string.Empty;
        private string _priceInput = string.Empty;
        private decimal _validatedPrice;
        private string _priceErrorMessage = string.Empty;
        private bool _canPost;

        public string NewItemTitle
        {
            get => _newItemTitle;
            set { _newItemTitle = value; OnPropertyChanged(); ValidateForm(); }
        }

        public string NewItemDesc
        {
            get => _newItemDesc;
            set { _newItemDesc = value; OnPropertyChanged(); ValidateForm(); }
        }

        public string PriceInput
        {
            get => _priceInput;
            set { _priceInput = value; OnPropertyChanged(); ValidateForm(); }
        }

        public string PriceErrorMessage
        {
            get => _priceErrorMessage;
            set { _priceErrorMessage = value; OnPropertyChanged(); }
        }

        public bool CanPost
        {
            get => _canPost;
            set { _canPost = value; OnPropertyChanged(); }
        }

        public decimal ValidatedPrice => _validatedPrice;

        public async Task SubmitListingAsync(string? category, string? condition, string imageUrl)
        {
            MovieApp.WebApi.Data.AppDbContext dbContext = App.Services.GetRequiredService<MovieApp.WebApi.Data.AppDbContext>();
            User? user = await dbContext.Users.FindAsync(SessionManager.CurrentUserID);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            Equipment newItem = new Equipment
            {
                Seller = user,
                Title = NewItemTitle,
                Description = NewItemDesc,
                Price = ValidatedPrice,
                Category = category ?? "Unknown",
                Condition = condition ?? "Unknown",
                ImageUrl = imageUrl,
                Status = EquipmentStatus.Available
            };

            await _equipmentService.ListItemAsync(newItem);
        }

        private void ValidateForm()
        {
            bool isPriceValid = decimal.TryParse(_priceInput, out decimal result);
            bool isTitleValid = !string.IsNullOrWhiteSpace(_newItemTitle);

            if (!isPriceValid && !string.IsNullOrEmpty(_priceInput))
            {
                PriceErrorMessage = "Please enter a valid numeric price!";
                CanPost = false;
                return;
            }

            if (isPriceValid && result <= 0)
            {
                PriceErrorMessage = "Price must be greater than 0!";
                CanPost = false;
                return;
            }

            if (isPriceValid && isTitleValid)
            {
                _validatedPrice = result;
                PriceErrorMessage = string.Empty;
                CanPost = true;
            }
            else
            {
                CanPost = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
