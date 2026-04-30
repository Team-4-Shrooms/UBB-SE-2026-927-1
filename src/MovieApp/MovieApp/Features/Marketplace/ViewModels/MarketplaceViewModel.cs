using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using MovieApp.DataLayer.Models;       
using MovieApp.DataLayer.Interfaces.Repositories; 
using MovieApp.Features.Shared.Models;

namespace MovieApp.Features.Marketplace.ViewModels 
{
    public class MarketplaceViewModel : INotifyPropertyChanged
    {
        private readonly IEquipmentRepository _repository;

        private List<Equipment> _allOriginalItems = new List<Equipment>();

        public ObservableCollection<Equipment> AvailableItems { get; set; } = new ObservableCollection<Equipment>();

        public decimal UserBalance => 5000.00m; 

        public MarketplaceViewModel(IEquipmentRepository equipmentRepository)
        {
            _repository = equipmentRepository;
        }

        public async Task LoadDataAsync()
        {
            List<Equipment> fetchedData = await _repository.FetchAvailableEquipmentAsync();
            _allOriginalItems = fetchedData;

            UpdateDisplayList(_allOriginalItems);
        }

        public void FilterByCategory(string? category)
        {
            List<Equipment> filtered = string.IsNullOrEmpty(category) || category == "All"
                ? _allOriginalItems
                : _allOriginalItems.Where(equipment => equipment.Category == category).ToList();
            UpdateDisplayList(filtered);
        }

        private void UpdateDisplayList(List<Equipment> items)
        {
            AvailableItems.Clear();
            foreach (Equipment item in items)
            {
                if (item != null)
                {
                    AvailableItems.Add(item);
                }
            }

            OnPropertyChanged(nameof(AvailableItems));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string StatusMessage => SessionManager.CurrentUserID == 0
            ? "Please log in to purchase equipment."
            : string.Empty;
    }
}
