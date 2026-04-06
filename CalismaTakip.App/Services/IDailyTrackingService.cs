using CalismaTakip.ViewModels;

namespace CalismaTakip.Services;

public interface IDailyTrackingService
{
    Task LoadIntoAsync(DailyTrackingViewModel viewModel, CancellationToken cancellationToken = default);

    Task SaveAsync(DailyTrackingViewModel viewModel, CancellationToken cancellationToken = default);

    /// <summary>Veritabanındaki günlük kaydı silinir; şablondan yeniden yükleme ViewModel tarafında yapılır.</summary>
    Task DeleteTrackForDateAsync(DateOnly date, CancellationToken cancellationToken = default);
}
