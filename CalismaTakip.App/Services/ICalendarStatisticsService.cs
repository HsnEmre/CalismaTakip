using CalismaTakip.Models.Dtos;

namespace CalismaTakip.Services;

public interface ICalendarStatisticsService
{
    /// <summary>Aralıktaki her gün için özet; kayıt yoksa <see cref="DailyCompletionSummary.Empty"/>.</summary>
    Task<IReadOnlyDictionary<DateOnly, DailyCompletionSummary>> GetCompletionSummariesForRangeAsync(
        DateOnly inclusiveStart,
        DateOnly inclusiveEnd,
        CancellationToken cancellationToken = default);

    Task<DailyCompletionSummary> GetCompletionSummaryAsync(DateOnly date, CancellationToken cancellationToken = default);

    Task<MonthlyStatisticsDto> GetMonthlyStatisticsAsync(int year, int month, CancellationToken cancellationToken = default);

    /// <summary>Takvim ızgarası için 42 günlük liste (Pazartesi başlangıçlı).</summary>
    Task<IReadOnlyList<CalendarDayItem>> GetCalendarDaysForMonthAsync(int year, int month, CancellationToken cancellationToken = default);
}
