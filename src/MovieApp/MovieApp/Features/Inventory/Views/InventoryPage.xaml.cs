using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MovieApp.DataLayer.Interfaces.Repositories;
using MovieApp.DataLayer.Models;
using MovieApp.Logic.Interfaces.Services;
using MovieApp.Features.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieApp.WebApi.Data;
using System.Linq;

namespace MovieApp.Features.Inventory.Views
{
    public sealed partial class InventoryPage : Page
    {
        private readonly IInventoryRepository _repository = App.Services.GetRequiredService<IInventoryRepository>();
        private readonly IInventoryService _inventoryService = App.Services.GetRequiredService<IInventoryService>();
        private readonly AppDbContext _context = App.Services.GetRequiredService<AppDbContext>();

        public InventoryPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                var err = new ContentDialog
                {
                    Title = "Error loading inventory",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = XamlRoot
                };
                await err.ShowAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            int userId = SessionManager.CurrentUserID;
            if (userId <= 0)
            {
                return;
            }

            _context.ChangeTracker.Clear();

            MoviesGrid.ItemsSource = await _repository.GetOwnedMoviesAsync(userId);

            List<OwnedTicket> tickets = await _context.OwnedTickets
                .Include(ot => ot.Event)
                .Where(ot => ot.User.Id == userId)
                .ToListAsync();
            TicketsGrid.ItemsSource = tickets;

            List<int> boughtEquipmentIds = await _context.Transactions
                .Where(t => t.Buyer.Id == userId && t.Type == "EquipmentPurchase" && t.Equipment != null)
                .Select(t => t.Equipment!.Id)
                .ToListAsync();

            EquipmentGrid.ItemsSource = await _context.Equipment
                .Where(e => boughtEquipmentIds.Contains(e.Id))
                .ToListAsync();
        }

        private void MoviesGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Movie movie)
            {
                Frame.Navigate(typeof(MovieDetail.Views.MovieDetailPage), new MovieDetailNavArgs { Movie = movie });
            }
        }

        private async void RemoveMovie_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Microsoft.UI.Xaml.FrameworkElement frameworkElement && frameworkElement.DataContext is Movie movie)
            {
                ContentDialog removeDialog = new ContentDialog
                {
                    Title = "Remove movie",
                    Content = $"Are you sure you want to remove '{movie.Title}' from your library? This will allow you to purchase it again.",
                    PrimaryButtonText = "Remove",
                    CloseButtonText = "Cancel",
                    XamlRoot = XamlRoot
                };

                if (await removeDialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    try
                    {
                        int userId = SessionManager.CurrentUserID;
                        List<OwnedMovie> ownerships = await _context.OwnedMovies
                            .Where(om => om.User.Id == userId && om.Movie.Id == movie.Id)
                            .ToListAsync();

                        if (ownerships.Any())
                        {
                            _context.OwnedMovies.RemoveRange(ownerships);

                            User? user = await _context.Users.FindAsync(userId);
                            Movie? dbMovie = await _context.Movies.FindAsync(movie.Id);

                            Transaction transaction = new Transaction
                            {
                                Buyer = user,
                                Movie = dbMovie,
                                Amount = 0m,
                                Type = "RemoveOwnedMovie",
                                Status = "Completed",
                                Timestamp = DateTime.UtcNow
                            };

                            _context.Transactions.Add(transaction);
                            await _context.SaveChangesAsync();
                        }
                        
                        await LoadDataAsync();
                    }
                    catch (Exception ex)
                    {
                        var err = new ContentDialog
                        {
                            Title = "Error",
                            Content = ex.Message,
                            CloseButtonText = "OK",
                            XamlRoot = XamlRoot
                        };
                        await err.ShowAsync();
                    }
                }
            }
        }

        private async void RemoveTicket_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Microsoft.UI.Xaml.FrameworkElement fe && fe.DataContext is OwnedTicket ot)
            {
                var dlg = new ContentDialog
                {
                    Title = "Remove ticket",
                    Content = $"Remove your ticket for '{ot.Event.Title}'? This will allow you to buy it again.",
                    PrimaryButtonText = "Remove",
                    CloseButtonText = "Cancel",
                    XamlRoot = XamlRoot
                };

                if (await dlg.ShowAsync() == ContentDialogResult.Primary)
                {
                    try
                    {
                        var userId = SessionManager.CurrentUserID;
                        var ownerships = await _context.OwnedTickets
                            .Where(ot2 => ot2.User.Id == userId && ot2.Event.Id == ot.Event.Id)
                            .ToListAsync();

                        if (ownerships.Any())
                        {
                            _context.OwnedTickets.RemoveRange(ownerships);

                            var user = await _context.Users.FindAsync(userId);
                            var movieEvent = await _context.MovieEvents.FindAsync(ot.Event.Id);

                            var transaction = new Transaction
                            {
                                Buyer = user,
                                Event = movieEvent,
                                Amount = 0m,
                                Type = "RemoveOwnedTicket",
                                Status = "Completed",
                                Timestamp = DateTime.UtcNow
                            };

                            _context.Transactions.Add(transaction);
                            await _context.SaveChangesAsync();
                        }

                        await LoadDataAsync();
                    }
                    catch (Exception ex)
                    {
                        var err = new ContentDialog
                        {
                            Title = "Error",
                            Content = ex.Message,
                            CloseButtonText = "OK",
                            XamlRoot = XamlRoot
                        };
                        await err.ShowAsync();
                    }
                }
            }
        }
        private async void RemoveEquipment_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Microsoft.UI.Xaml.FrameworkElement fe && fe.DataContext is Equipment eq)
            {
                var dlg = new ContentDialog
                {
                    Title = "Remove equipment",
                    Content = $"Remove '{eq.Title}' from your inventory? This will make it available in the marketplace again.",
                    PrimaryButtonText = "Remove",
                    CloseButtonText = "Cancel",
                    XamlRoot = XamlRoot
                };

                if (await dlg.ShowAsync() == ContentDialogResult.Primary)
                {
                    var userId = SessionManager.CurrentUserID;
                    
                    var transaction = await _context.Transactions
                        .FirstOrDefaultAsync(t => t.Buyer.Id == userId && t.Equipment.Id == eq.Id && t.Type == "EquipmentPurchase");
                    
                    if (transaction != null)
                    {
                        _context.Transactions.Remove(transaction);
                    }

                    var dbEq = await _context.Equipment.FindAsync(eq.Id);
                    if (dbEq != null)
                    {
                        dbEq.Status = EquipmentStatus.Available;
                    }

                    await _context.SaveChangesAsync();
                    await LoadDataAsync();
                }
            }
        }
    }
}
