using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Imaging;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Features.Shared.Models;
using System;
using System.Threading.Tasks;
using MovieApp.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MovieApp.Features.Events.Views
{
    public sealed partial class BuyTicketPage : Page
    {
        private MovieEvent? _event;
        private readonly IEventRepository _eventRepository = App.Services.GetRequiredService<IEventRepository>();
        private readonly IUserRepository _userRepository = App.Services.GetRequiredService<IUserRepository>();
        private readonly IEventService _eventService = App.Services.GetRequiredService<IEventService>();
        private readonly AppDbContext _context = App.Services.GetRequiredService<AppDbContext>();

        public BuyTicketPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs @event)
        {
            base.OnNavigatedTo(@event);

            try
            {
                if (@event.Parameter is int id)
                {
                    _event = await _eventRepository.GetEventByIdAsync(id);
                }
                else if (@event.Parameter is MovieEvent movieEvent)
                {
                    _event = movieEvent;
                }

                if (_event == null)
                {
                    return;
                }

                PopulateUI();
                UpdateButtonState();
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error loading event",
                    Content = "Could not load event details. Please ensure the backend server is running.",
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await errorDialog.ShowAsync();
                if (Frame.CanGoBack) Frame.GoBack();
            }
        }

        private void PopulateUI()
        {
            if (_event == null)
            {
                return;
            }

            TitleText.Text = _event.Title;
            DescriptionText.Text = _event.Description ?? "";
            DateText.Text = _event.Date.ToString("dd MMM yyyy");
            LocationText.Text = _event.Location;
            PriceText.Text = _event.TicketPrice.ToString("C");

            if (!string.IsNullOrEmpty(_event.PosterUrl))
            {
                try
                {
                    PosterImage.Source = new BitmapImage(new Uri(_event.PosterUrl));
                }
                catch (UriFormatException)
                {
                }
            }
        }

        private void UpdateButtonState()
        {
            if (!SessionManager.IsLoggedIn)
            {
                ConfirmButton.IsEnabled = false;
                InsufficientText.Text = "You must be signed in to purchase.";
                InsufficientText.Visibility = Visibility.Visible;
                return;
            }

            if (_event == null)
            {
                ConfirmButton.IsEnabled = false;
                return;
            }

            decimal balance = SessionManager.CurrentUserBalance;
            bool canBuy = balance >= _event.TicketPrice && _event.Date >= DateTime.Now;
            ConfirmButton.IsEnabled = canBuy;

            if (!canBuy)
            {
                if (_event.Date < DateTime.Now)
                {
                    InsufficientText.Text = "This event has already passed.";
                }
                else
                {
                    InsufficientText.Text = $"Insufficient funds. Balance: {balance:C} — Price: {_event.TicketPrice:C}";
                }

                InsufficientText.Visibility = Visibility.Visible;
            }
            else
            {
                InsufficientText.Visibility = Visibility.Collapsed;
            }
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_event == null) return;

            if (!SessionManager.IsLoggedIn)
            {
                var dialog = new ContentDialog
                {
                    Title = "Sign in",
                    Content = "Please sign in to continue.",
                    PrimaryButtonText = "Sign in",
                    CloseButtonText = "Cancel",
                    XamlRoot = XamlRoot
                };

                if (await dialog.ShowAsync() != ContentDialogResult.Primary)
                    return;

                SessionManager.CurrentUserID = 1;
                UpdateButtonState();
                return;
            }

            try
            {
                var userId = SessionManager.CurrentUserID;
                var user = await _context.Users.FindAsync(userId) 
                    ?? throw new Exception("User not found.");
                
                var movieEvent = await _context.MovieEvents
                    .Include(@event => @event.Movie)
                    .FirstOrDefaultAsync(@event => @event.Id == _event.Id)
                    ?? throw new Exception("Event not found.");

                if (user.Balance < movieEvent.TicketPrice)
                    throw new InvalidOperationException("Insufficient balance.");

                user.Balance -= movieEvent.TicketPrice;

                var ticket = new OwnedTicket
                {
                    User = user,
                    Event = movieEvent,
                    PurchaseDate = DateTime.UtcNow
                };

                var transaction = new Transaction
                {
                    Buyer = user,
                    Event = movieEvent,
                    Amount = -movieEvent.TicketPrice,
                    Type = "EventTicket",
                    Status = "Completed",
                    Timestamp = DateTime.UtcNow
                };

                _context.OwnedTickets.Add(ticket);
                _context.Transactions.Add(transaction);
                
                await _context.SaveChangesAsync();
                
                SessionManager.CurrentUserBalance = user.Balance;

                UpdateButtonState();
 
                var dialog = new ContentDialog
                {
                    Title = "Purchase successful",
                    Content = $"Ticket for '{_event.Title}' purchased and added to your library.",
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await dialog.ShowAsync();
 
                Frame.GoBack();
            }
            catch (InvalidOperationException exception)
            {
                var error = new ContentDialog
                {
                    Title = "Cannot complete purchase",
                    Content = exception.Message,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await error.ShowAsync();
            }
        }
    }
}
