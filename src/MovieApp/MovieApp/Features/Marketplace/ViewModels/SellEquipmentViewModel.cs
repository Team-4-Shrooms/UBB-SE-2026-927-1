using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MovieApp.Features.Shared.Models;
using Windows.Storage.Pickers;
using WinRT.Interop;

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
        private string _selectedImagePath = string.Empty;
        private string _selectedFileName = "Click to select gear photo";
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

        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set { _selectedImagePath = value; OnPropertyChanged(); ValidateForm(); }
        }

        public string SelectedFileName
        {
            get => _selectedFileName;
            set { _selectedFileName = value; OnPropertyChanged(); }
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

            // Use the URL if provided, otherwise use the local path
            string finalImageUrl = !string.IsNullOrWhiteSpace(imageUrl) ? imageUrl : SelectedImagePath;

            Equipment newItem = new Equipment
            {
                Seller = user,
                Title = NewItemTitle,
                Description = NewItemDesc,
                Price = ValidatedPrice,
                Category = category ?? "Unknown",
                Condition = condition ?? "Unknown",
                ImageUrl = finalImageUrl,
                Status = EquipmentStatus.Available
            };

            await _equipmentService.ListItemAsync(newItem);
        }

        public async Task SelectImageAsync()
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".webp");

            // Initialize with window handle
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, windowHandle);

            var file = await filePicker.PickSingleFileAsync();
            if (file != null)
            {
                SelectedImagePath = file.Path;
                SelectedFileName = file.Name;
            }
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
