using CalismaTakip.Helpers;
using CalismaTakip.Models;

namespace CalismaTakip.Models.Dtos;

/// <summary>Günlük plan satırlarına göre tamamlanma özeti (takvim / istatistik).</summary>
public sealed class DailyCompletionSummary
{
    public static DailyCompletionSummary Empty(DateOnly date) =>
        new()
        {
            Date = date,
            HasRecord = false,
            CompletedTaskCount = 0,
            TotalTaskCount = 0,
            CompletionPercent = 0,
            Note = string.Empty,
            PlanTypeLabel = string.Empty
        };

    public required DateOnly Date { get; init; }

    public bool HasRecord { get; init; }

    public int CompletedTaskCount { get; init; }

    public int TotalTaskCount { get; init; }

    /// <summary>0–100: tamamlanan / toplam satır * 100.</summary>
    public double CompletionPercent { get; init; }

    public string Note { get; init; } = string.Empty;

    public string PlanTypeLabel { get; init; } = string.Empty;

    public static DailyCompletionSummary FromHeader(DailyPlanTrackHeader header)
    {
        ArgumentNullException.ThrowIfNull(header);
        var items = header.Items?.ToList() ?? new List<DailyPlanTrackItem>();
        var total = items.Count;
        var done = items.Count(i => i.IsCompleted);
        var hasRecord = total > 0;
        var pct = total == 0 ? 0 : Math.Round(done * 100.0 / total, 1);

        return new DailyCompletionSummary
        {
            Date = header.TrackDate,
            HasRecord = hasRecord,
            CompletedTaskCount = done,
            TotalTaskCount = total,
            CompletionPercent = pct,
            Note = header.Note ?? string.Empty,
            PlanTypeLabel = PlanTemplateKindResolver.ToTurkishPlanLabel(header.TemplateKind)
        };
    }
}
