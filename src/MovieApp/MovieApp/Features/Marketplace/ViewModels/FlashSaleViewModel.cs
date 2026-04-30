using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MovieApp.Features.Marketplace.ViewModels
{
    public class FlashSaleViewModel : INotifyPropertyChanged
    {
        private DispatcherTimer? _timer;
        private string _displayText = string.Empty;
        private string _timerText = string.Empty;
        private DateTime _expiryDate;
        private bool _isActive;

        private Action? _onExpiredAction;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public string TimerText
        {
            get => _timerText;
            set
            {
                _timerText = value;
                OnPropertyChanged();
            }
        }

        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                OnPropertyChanged();
            }
        }

        public FlashSaleViewModel(DateTime saleEndTime, Action? onExpired = null)
        {
            _onExpiredAction = onExpired;

            if (saleEndTime <= DateTime.Now)
            {
                IsActive = false;
                return;
            }

            _expiryDate = saleEndTime;
            IsActive = true;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += TimerTick;
            _timer.Start();

            UpdateCountdown();
        }

        private void TimerTick(object? sender, object? e)
        {
            UpdateCountdown();
        }

        private void UpdateCountdown()
        {
            TimeSpan timeRemaining = _expiryDate - DateTime.Now;

            if (timeRemaining.TotalSeconds > 0)
            {
                DisplayText = "Flash sale";
                TimerText = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                                (int)timeRemaining.TotalHours,
                                                (int)timeRemaining.Minutes,
                                                (int)timeRemaining.Seconds);
            }
            else
            {
                _timer?.Stop();
                DisplayText = "Flash sale has expired!";
                TimerText = "00:00:00";

                IsActive = false;

                _onExpiredAction?.Invoke();
            }
        }

        public Visibility BannerVisibility => IsActive ? Visibility.Visible : Visibility.Collapsed;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
