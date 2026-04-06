using System.Windows;
using CalismaTakip.Data;
using CalismaTakip.Services;
using CalismaTakip.ViewModels;
using CalismaTakip.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CalismaTakip;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlite(DatabasePaths.GetConnectionString()));
        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
        services.AddSingleton<IWeeklyPlanService, WeeklyPlanService>();
        services.AddSingleton<IDailyTrackingService, DailyTrackingService>();
        services.AddSingleton<ITakipGecmisiService, TakipGecmisiService>();
        services.AddSingleton<ICalendarStatisticsService, CalendarStatisticsService>();
        services.AddSingleton<WeekdayPlanViewModel>();
        services.AddSingleton<WeekendPlanViewModel>();
        services.AddSingleton<DailyTrackingViewModel>();
        services.AddSingleton<TakipGecmisiViewModel>();
        services.AddSingleton<CalendarViewModel>();
        services.AddSingleton<StatisticsViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        try
        {
            var init = _serviceProvider.GetRequiredService<IDatabaseInitializer>();
            init.Initialize();
        }
        catch (Exception ex)
        {
            Helpers.UserMessage.ShowError(
                $"Veritabanı başlatılamadı: {ex.InnerException?.Message ?? ex.Message}");
            Shutdown();
            return;
        }

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
