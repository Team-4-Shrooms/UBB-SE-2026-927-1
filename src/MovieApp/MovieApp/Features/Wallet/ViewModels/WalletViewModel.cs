using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;
using CommunityToolkit.Mvvm.Input;
using MovieApp.Features.Shared.Models;
using System;

namespace MovieApp.Features.Wallet.ViewModels
{
    public class WalletViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // --- Balance ---
        private decimal _balance;
        public decimal Balance
        {
            get => _balance;
            set
            {
                _balance = value;
                OnPropertyChanged(nameof(Balance));
                OnPropertyChanged(nameof(DisplayBalance));
            }
        }
        public string DisplayBalance => Balance.ToString("C");

        // --- TopUp Form Fields ---
        private string _cardHolderName = string.Empty;
        public string CardHolderName
        {
            get => _cardHolderName;
            set { _cardHolderName = value; OnPropertyChanged(nameof(CardHolderName)); }
        }

        private string _cardNumber = string.Empty;
        public string CardNumber
        {
            get => _cardNumber;
            set { _cardNumber = value; OnPropertyChanged(nameof(CardNumber)); }
        }

        private string _expirationDate = string.Empty;
        public string ExpirationDate
        {
            get => _expirationDate;
            set { _expirationDate = value; OnPropertyChanged(nameof(ExpirationDate)); }
        }

        private string _cvv = string.Empty;
        public string CVV
        {
            get => _cvv;
            set { _cvv = value; OnPropertyChanged(nameof(CVV)); }
        }

        private double _topUpAmount;
        public double TopUpAmount
        {
            get => _topUpAmount;
            set { _topUpAmount = value; OnPropertyChanged(nameof(TopUpAmount)); }
        }

        // --- Feedback Messages ---
        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        private string _successMessage = string.Empty;
        public string SuccessMessage
        {
            get => _successMessage;
            set { _successMessage = value; OnPropertyChanged(nameof(SuccessMessage)); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>();

        public IRelayCommand TopUpCommand { get; }
        public IAsyncRelayCommand LoadTransactionsCommand { get; }

        private readonly ITransactionRepository _transactionRepo;
        private readonly IUserRepository _userRepo;

        public WalletViewModel(IUserRepository userRepo, ITransactionRepository transactionRepo)
        {
            _userRepo = userRepo;
            _transactionRepo = transactionRepo;
            
            TopUpCommand = new RelayCommand(ExecuteTopUp);
            LoadTransactionsCommand = new AsyncRelayCommand(LoadTransactionsAsync);
            
            _balance = SessionManager.CurrentUserBalance;
        }

        public async Task LoadTransactionsAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await Task.Run(() => _transactionRepo.GetTransactionsByUserId(SessionManager.CurrentUserID));

                Transactions.Clear();
                foreach (var t in result.OrderByDescending(t => t.Timestamp))
                    Transactions.Add(t);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load transactions: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void ExecuteTopUp()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (!ValidateCard())
                return;

            decimal amount = (decimal)TopUpAmount;
            
            try
            {
                var dbContext = App.Services.GetRequiredService<MovieApp.WebApi.Data.AppDbContext>();
                var user = await dbContext.Users.FindAsync(SessionManager.CurrentUserID);
                if (user == null) return;

                user.Balance += amount;
                
                var transaction = new Transaction
                {
                    Buyer = user,
                    Amount = amount,
                    Type = "TopUp",
                    Status = "Completed",
                    Timestamp = DateTime.UtcNow
                };

                dbContext.Transactions.Add(transaction);
                await dbContext.SaveChangesAsync();

                Balance = user.Balance;
                SessionManager.CurrentUserBalance = Balance;
                
                Transactions.Insert(0, transaction);

                SuccessMessage = $"Successfully added {amount:C} to your wallet!";
                ClearForm();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Transaction failed: " + ex.Message;
            }
        }

        private bool ValidateCard()
        {
            if (string.IsNullOrWhiteSpace(CardHolderName))
            {
                ErrorMessage = "Please enter the cardholder name.";
                return false;
            }

            if (CardNumber.Length != 16 || !long.TryParse(CardNumber, out _))
            {
                ErrorMessage = "Card number must be exactly 16 digits.";
                return false;
            }

            if (CVV.Length != 3 || !int.TryParse(CVV, out _))
            {
                ErrorMessage = "CVV must be exactly 3 digits.";
                return false;
            }

            if (TopUpAmount <= 0)
            {
                ErrorMessage = "Amount must be greater than 0.";
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            CardHolderName = string.Empty;
            CardNumber = string.Empty;
            ExpirationDate = string.Empty;
            CVV = string.Empty;
            TopUpAmount = 0;
        }
    }
}
