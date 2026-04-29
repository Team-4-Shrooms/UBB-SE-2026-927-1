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
        private readonly IEventRepository _eventRepo = App.Services.GetRequiredService<IEventRepository>();
        private readonly IUserRepository _userRepo = App.Services.GetRequiredService<IUserRepository>();
        private readonly IEventService _eventService = App.Services.GetRequiredService<IEventService>();
        private readonly AppDbContext _context = App.Services.GetRequiredService<AppDbContext>();

        public MovieEventsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is MovieEventsNavArgs args)
            {
                _movie = args.Movie;
            }

            await LoadEventsAsync();
            ApplyFilters();
        }

        private async Task LoadEventsAsync()
        {
            if (_movie == null)
            {
                TitleBlock.Text = "Movie Events";
                _allEvents = await _eventRepo.GetAllEventsAsync();
            }
            else
            {
                TitleBlock.Text = $"Events - {_movie.Title}";
                _allEvents = await _eventRepo.GetEventsByMovieIdAsync(_movie.Id);
            }
            
            _context.ChangeTracker.Clear();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();

        private void FilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void ApplyFilters()
        {
            if (_allEvents == null) return;

            var filtered = _allEvents.AsEnumerable();

            var search = SearchBox?.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                filtered = filtered.Where(ev =>
                    (ev.Title ?? "").Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (ev.Description ?? "").Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (ev.Location ?? "").Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var sel = (FilterCombo?.SelectedItem as ComboBoxItem)?.Content as string ?? "All";
            if (sel == "Upcoming")
                filtered = filtered.Where(ev => ev.Date >= DateTime.Now);
            else if (sel == "Past")
                filtered = filtered.Where(ev => ev.Date < DateTime.Now);

            EventsList.ItemsSource = filtered.ToList();
            UpdateBuyButtons();
        }

        private void UpdateBuyButtons()
        {
            var userId = SessionManager.CurrentUserID;

            foreach (var item in EventsList.Items)
            {
                var lvi = EventsList.ContainerFromItem(item) as ListViewItem;
                if (lvi == null) continue;
                
                var btn = FindDescendantByName(lvi, "BuyTicketButton") as Button;
                if (btn == null) continue;

                var ev = item as MovieEvent;
                var balance = _userRepo.GetBalance(userId);
                var canBuy = ev != null && balance >= ev.TicketPrice && ev.Date >= DateTime.Now;
                btn.IsEnabled = canBuy;
                btn.Opacity = canBuy ? 1.0 : 0.55;
            }
        }

        private static object? FindDescendantByName(DependencyObject parent, string name)
        {
            if (parent == null) return null;
            var count = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement fe && fe.Name == name)
                    return fe;
                var found = FindDescendantByName(child, name);
                if (found != null) return found;
            }
            return null;
        }

        private void BuyTicket_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe || fe.DataContext is not MovieEvent me)
                return;
 
            Frame.Navigate(typeof(BuyTicketPage), me);
        }
    }
}
