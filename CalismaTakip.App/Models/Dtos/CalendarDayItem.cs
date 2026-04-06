namespace CalismaTakip.Models.Dtos;

/// <summary>Takvim ızgarasındaki tek bir gün için veri taşıyıcısı.</summary>
public sealed class CalendarDayItem
{
    public required DateOnly Date { get; init; }

    /// <summary>Görüntülenen ayın içinde mi (padding günleri için false).</summary>
    public bool BelongsToDisplayedMonth { get; init; }

    public required DailyCompletionSummary Summary { get; init; }
}
