using CalismaTakip.ViewModels;

namespace CalismaTakip.Services;

public sealed class TakipGecmisiQueryResult
{
    public IReadOnlyList<TakipGecmisiRowViewModel> Rows { get; init; } = Array.Empty<TakipGecmisiRowViewModel>();

    public int TotalRecordDays { get; init; }

    public double AverageDailyCompletionPercent { get; init; }

    public int FullyCompletedDays { get; init; }

    public int TotalTasksCompleted { get; init; }

    public int TotalTasksScheduled { get; init; }
}
