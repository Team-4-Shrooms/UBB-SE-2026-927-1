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

        public MarketplaceViewModel(IEquipmentRepository equipmentRepo)
        {
            _repository = equipmentRepo;
            LoadData();
        }

        public void LoadData()
        {
            var data = _repository.FetchAvailableEquipment() ?? new List<Equipment>();
            _allOriginalItems = data;

            UpdateDisplayList(_allOriginalItems);
        }

        public void FilterByCategory(string? category)
        {
            var filtered = string.IsNullOrEmpty(category) || category == "All"
                ? _allOriginalItems
                : _allOriginalItems.Where(x => x.Category == category).ToList();

            UpdateDisplayList(filtered);
        }

        private void UpdateDisplayList(List<Equipment> items)
        {
            AvailableItems.Clear();
            foreach (var item in items)
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
