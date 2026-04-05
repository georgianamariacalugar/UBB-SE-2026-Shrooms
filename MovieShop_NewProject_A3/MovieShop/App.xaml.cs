using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using MovieShop.Repositories;
using MovieShop.Services;
using MovieShop.ViewModels;

namespace MovieShop
{

    public partial class App : Application
    {
        public static Window? _window;

        public static IServiceProvider Services { get; private set; }

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

            services.AddTransient<IInventoryService, InventoryService>();
            services.AddTransient<IEquipmentPurchaseService, EquipmentPurchaseService>();
            services.AddTransient<IEventTicketService, EventTicketService>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<MarketplaceViewModel>();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
