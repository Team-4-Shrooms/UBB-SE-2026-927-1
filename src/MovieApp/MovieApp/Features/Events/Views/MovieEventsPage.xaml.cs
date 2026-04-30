using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.DataLayer.Models;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Features.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MovieApp.Features.Events.Views
{
    public sealed partial class MovieEventsPage : Page
    {
        private Movie? _movie;
        private List<MovieEvent>? _allEvents;
        private readonly IEventRepository _eventRepository = App.Services.GetRequiredService<IEventRepository>();
        private readonly IUserRepository _userRepository = App.Services.GetRequiredService<IUserRepository>();
        private readonly IEventService _eventService = App.Services.GetRequiredService<IEventService>();
        private readonly AppDbContext _context = App.Services.GetRequiredService<AppDbContext>();

        public MovieEventsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs @event)
        {
            base.OnNavigatedTo(@event);

            try
            {
                if (@event.Parameter is MovieEventsNavArgs args)
                {
                    _movie = args.Movie;
                }

                await LoadEventsAsync();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error loading events",
                    Content = "Could not load events. Please ensure the backend server is running.",
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }

        private async Task LoadEventsAsync()
        {
            if (_movie == null)
            {
                TitleBlock.Text = "Movie Events";
                _allEvents = await _eventRepository.GetAllEventsAsync();
            }
            else
            {
                TitleBlock.Text = $"Events - {_movie.Title}";
                _allEvents = await _eventRepository.GetEventsByMovieIdAsync(_movie.Id);
            }

            _context.ChangeTracker.Clear();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();

        private void FilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void ApplyFilters()
        {
            if (_allEvents == null)
            {
                return;
            }

            IEnumerable<MovieEvent> filtered = _allEvents.AsEnumerable();

            string? search = SearchBox?.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                filtered = filtered.Where(ev =>
                    (ev.Title ?? "").Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (ev.Description ?? "").Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (ev.Location ?? "").Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            string selectedFilter = (FilterCombo?.SelectedItem as ComboBoxItem)?.Content as string ?? "All";
            if (selectedFilter == "Upcoming")
            {
                filtered = filtered.Where(@event => @event.Date >= DateTime.Now);
            }
            else if (selectedFilter == "Past")
            {
                filtered = filtered.Where(@event => @event.Date < DateTime.Now);
            }

            EventsList.ItemsSource = filtered.ToList();
            UpdateBuyButtons();
        }

        private void UpdateBuyButtons()
        {
            int userId = SessionManager.CurrentUserID;

            foreach (object item in EventsList.Items)
            {
                ListViewItem? eventsListItem = EventsList.ContainerFromItem(item) as ListViewItem;
                if (eventsListItem == null)
                {
                    continue;
                }

                Button? buyTicketButton = FindDescendantByName(eventsListItem, "BuyTicketButton") as Button;
                if (buyTicketButton == null)
                {
                    continue;
                }

                MovieEvent? @event = item as MovieEvent;
                decimal balance = SessionManager.CurrentUserBalance;
                bool canBuy = @event != null && balance >= @event.TicketPrice && @event.Date >= DateTime.Now;
                buyTicketButton.IsEnabled = canBuy;
                buyTicketButton.Opacity = canBuy ? 1.0 : 0.55;
            }
        }

        private static object? FindDescendantByName(DependencyObject parent, string name)
        {
            if (parent == null)
            {
                return null;
            }

            int childCount = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement frameworkElement && frameworkElement.Name == name)
                {
                    return frameworkElement;
                }

                object? found = FindDescendantByName(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private void BuyTicket_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement frameworkElement || frameworkElement.DataContext is not MovieEvent movieEvent)
            {
                return;
            }

            Frame.Navigate(typeof(BuyTicketPage), movieEvent);
        }

    }
}
