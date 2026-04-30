using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using MovieApp.Http; // Ensures ApiClient is recognized

namespace MovieApp
{
    public partial class App : Application
    {
        private Window? _window;

        // 1. Expose the services globally so your ViewModels/Pages can access them
        public static IServiceProvider Services { get; private set; } = null!;

        public App()
        {
            InitializeComponent();
            
            // 2. Initialize the DI container when the app starts
            Services = ConfigureServices();
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // 3. Register your ApiClient
            services.AddHttpClient<ApiClient>(client => 
            {
                // Make sure this URL matches your running Web API!
                client.BaseAddress = new Uri("https://localhost:7196/"); 
            });

            // Note: When you get your ViewModels back, you'll register them here!
            // e.g., services.AddTransient<MyViewModel>();

            return services.BuildServiceProvider();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
