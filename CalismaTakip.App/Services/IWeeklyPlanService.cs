using System.Collections.ObjectModel;
using CalismaTakip.ViewModels;

namespace CalismaTakip.Services;

public interface IWeeklyPlanService
{
    Task<ObservableCollection<WeekdayPlanRowViewModel>> GetWeekdayRowsAsync(CancellationToken cancellationToken = default);

    Task SaveWeekdayAsync(IReadOnlyList<WeekdayPlanRowViewModel> rows, CancellationToken cancellationToken = default);

    Task<ObservableCollection<WeekendPlanRowViewModel>> GetWeekendRowsAsync(CancellationToken cancellationToken = default);

    Task SaveWeekendAsync(IReadOnlyList<WeekendPlanRowViewModel> rows, CancellationToken cancellationToken = default);
}
