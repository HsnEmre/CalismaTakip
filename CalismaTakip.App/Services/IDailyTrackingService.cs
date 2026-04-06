using CalismaTakip.ViewModels;

namespace CalismaTakip.Services;

public interface IDailyTrackingService
{
    Task LoadIntoAsync(DailyTrackingViewModel viewModel, CancellationToken cancellationToken = default);

    Task SaveAsync(DailyTrackingViewModel viewModel, CancellationToken cancellationToken = default);
}
