using CalismaTakip.Data;
using CalismaTakip.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CalismaTakip.Services;

public class CalendarStatisticsService : ICalendarStatisticsService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public CalendarStatisticsService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyDictionary<DateOnly, DailyCompletionSummary>> GetCompletionSummariesForRangeAsync(
        DateOnly inclusiveStart,
        DateOnly inclusiveEnd,
        CancellationToken cancellationToken = default)
    {
        if (inclusiveEnd < inclusiveStart)
            (inclusiveStart, inclusiveEnd) = (inclusiveEnd, inclusiveStart);

        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var headers = await db.DailyPlanTrackHeaders
            .AsNoTracking()
            .Include(h => h.Items)
            .Where(h => h.TrackDate >= inclusiveStart && h.TrackDate <= inclusiveEnd)
            .ToListAsync(cancellationToken);

        var dict = new Dictionary<DateOnly, DailyCompletionSummary>();
        foreach (var h in headers)
            dict[h.TrackDate] = DailyCompletionSummary.FromHeader(h);

        return dict;
    }

    public async Task<DailyCompletionSummary> GetCompletionSummaryAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var header = await db.DailyPlanTrackHeaders
            .AsNoTracking()
            .Include(h => h.Items)
            .FirstOrDefaultAsync(h => h.TrackDate == date, cancellationToken);

        return header == null
            ? DailyCompletionSummary.Empty(date)
            : DailyCompletionSummary.FromHeader(header);
    }

    public async Task<MonthlyStatisticsDto> GetMonthlyStatisticsAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var headers = await db.DailyPlanTrackHeaders
            .AsNoTracking()
            .Include(h => h.Items)
            .Where(h => h.TrackDate >= start && h.TrackDate <= end)
            .ToListAsync(cancellationToken);

        var totalRecorded = headers.Count;
        if (totalRecorded == 0)
        {
            return new MonthlyStatisticsDto
            {
                Year = year,
                Month = month,
                TotalRecordedDays = 0,
                TotalTasksCompletedInMonth = 0,
                TotalTasksScheduledInMonth = 0,
                AverageDailyCompletionPercent = 0,
                FullCompletionDays = 0,
                ZeroCompletionDays = 0,
                PartialCompletionDays = 0,
                BestDayCompletionPercent = 0,
                TotalPerformancePercent = 0
            };
        }

        var summaries = headers.Select(DailyCompletionSummary.FromHeader).ToList();
        var sumDone = headers.Sum(h => h.Items.Count(i => i.IsCompleted));
        var sumTotal = headers.Sum(h => h.Items.Count);

        var full = headers.Count(h => h.Items.Count > 0 && h.Items.All(i => i.IsCompleted));
        var zero = headers.Count(h => h.Items.Count > 0 && h.Items.All(i => !i.IsCompleted));
        var partial = headers.Count(h =>
        {
            var n = h.Items.Count;
            if (n == 0)
                return false;
            var d = h.Items.Count(i => i.IsCompleted);
            return d > 0 && d < n;
        });

        var withTasks = summaries.Where(s => s.TotalTaskCount > 0).ToList();
        var avgDaily = withTasks.Count > 0
            ? withTasks.Average(s => s.CompletionPercent)
            : 0;
        var best = withTasks.Count > 0 ? withTasks.Max(s => s.CompletionPercent) : 0;
        var totalPerf = sumTotal > 0 ? sumDone * 100.0 / sumTotal : 0;

        return new MonthlyStatisticsDto
        {
            Year = year,
            Month = month,
            TotalRecordedDays = totalRecorded,
            TotalTasksCompletedInMonth = sumDone,
            TotalTasksScheduledInMonth = sumTotal,
            AverageDailyCompletionPercent = Math.Round(avgDaily, 1),
            FullCompletionDays = full,
            ZeroCompletionDays = zero,
            PartialCompletionDays = partial,
            BestDayCompletionPercent = Math.Round(best, 1),
            TotalPerformancePercent = Math.Round(totalPerf, 1)
        };
    }

    public async Task<IReadOnlyList<CalendarDayItem>> GetCalendarDaysForMonthAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var firstOfMonth = new DateOnly(year, month, 1);
        var offset = GetMondayBasedOffset(firstOfMonth);
        var gridStart = firstOfMonth.AddDays(-offset);
        var gridEnd = gridStart.AddDays(41);

        var dict = await GetCompletionSummariesForRangeAsync(gridStart, gridEnd, cancellationToken).ConfigureAwait(false);

        var list = new List<CalendarDayItem>(42);
        for (var i = 0; i < 42; i++)
        {
            var d = gridStart.AddDays(i);
            var inMonth = d.Month == month && d.Year == year;
            if (!dict.TryGetValue(d, out var summary))
                summary = DailyCompletionSummary.Empty(d);

            list.Add(new CalendarDayItem
            {
                Date = d,
                BelongsToDisplayedMonth = inMonth,
                Summary = summary
            });
        }

        return list;
    }

    private static int GetMondayBasedOffset(DateOnly firstOfMonth)
    {
        return firstOfMonth.DayOfWeek switch
        {
            DayOfWeek.Monday => 0,
            DayOfWeek.Tuesday => 1,
            DayOfWeek.Wednesday => 2,
            DayOfWeek.Thursday => 3,
            DayOfWeek.Friday => 4,
            DayOfWeek.Saturday => 5,
            DayOfWeek.Sunday => 6,
            _ => 0
        };
    }
}
