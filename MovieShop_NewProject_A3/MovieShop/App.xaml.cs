using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using MovieShop.Repositories;
using MovieShop.Services;
using MovieShop.ViewModels;
using System;

namespace MovieShop
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static Window? _window;

        public static IServiceProvider Services { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDatabaseSingleton>(DatabaseSingleton.Instance);

            services.AddTransient<IUserRepository, UserRepo>();
            services.AddTransient<IMovieRepository, MovieRepo>();
            services.AddTransient<IEquipmentRepository, EquipmentRepo>();
            services.AddTransient<IEventRepository, EventRepo>();
            services.AddTransient<IActiveSalesRepository, ActiveSalesRepo>();
            services.AddTransient<IReviewRepository, ReviewRepo>();
            services.AddTransient<ITransactionRepository, TransactionRepo>();
            services.AddTransient<IInventoryRepository, InventoryRepo>();

            services.AddTransient<IMoviePurchaseService, MoviePurchaseService>();
            services.AddTransient<IMovieReviewService, MovieReviewService>();
            services.AddTransient<IMovieCatalogService, MovieCatalogService>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<MarketplaceViewModel>();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
