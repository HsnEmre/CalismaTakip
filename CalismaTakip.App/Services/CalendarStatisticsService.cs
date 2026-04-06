using CalismaTakip.Data;
using CalismaTakip.Helpers;
using CalismaTakip.Models;
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

        var records = await db.DailyCheckRecords
            .AsNoTracking()
            .Include(r => r.Completions)
            .ThenInclude(c => c.DailyCheckDefinition)
            .Where(r => r.Date >= inclusiveStart && r.Date <= inclusiveEnd)
            .ToListAsync(cancellationToken);

        var dict = new Dictionary<DateOnly, DailyCompletionSummary>();
        foreach (var r in records)
            dict[r.Date] = DailyCompletionHelper.ToSummary(r, r.Date);

        return dict;
    }

    public async Task<DailyCompletionSummary> GetCompletionSummaryAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var record = await db.DailyCheckRecords
            .AsNoTracking()
            .Include(r => r.Completions)
            .ThenInclude(c => c.DailyCheckDefinition)
            .FirstOrDefaultAsync(r => r.Date == date, cancellationToken);

        return record == null
            ? DailyCompletionSummary.Empty(date)
            : DailyCompletionHelper.ToSummary(record, date);
    }

    public async Task<MonthlyStatisticsDto> GetMonthlyStatisticsAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var records = await db.DailyCheckRecords
            .AsNoTracking()
            .Include(r => r.Completions)
            .ThenInclude(c => c.DailyCheckDefinition)
            .Where(r => r.Date >= start && r.Date <= end)
            .ToListAsync(cancellationToken);

        var totalRecorded = records.Count;
        if (totalRecorded == 0)
        {
            return new MonthlyStatisticsDto
            {
                Year = year,
                Month = month,
                TotalRecordedDays = 0,
                TechnicalDoneDays = 0,
                SpeakingDoneDays = 0,
                GrammarDoneDays = 0,
                SleepDoneDays = 0,
                AverageDailyCompletionPercent = 0,
                FullCompletionDays = 0,
                ZeroCompletionDays = 0,
                BestDayCompletionPercent = 0,
                TotalPerformancePercent = 0
            };
        }

        var summaries = records.Select(r => DailyCompletionHelper.ToSummary(r, r.Date)).ToList();

        var technical = summaries.Count(s => s.TechnicalDone);
        var speaking = summaries.Count(s => s.SpeakingDone);
        var grammar = summaries.Count(s => s.GrammarDone);
        var sleep = summaries.Count(s => s.SleepDone);
        var full = summaries.Count(s => s.CompletedCount == 4);
        var zero = summaries.Count(s => s.CompletedCount == 0);
        var avgDaily = summaries.Average(s => s.CompletionPercent);
        var best = summaries.Max(s => s.CompletionPercent);
        var totalSlots = totalRecorded * 4;
        var totalDone = summaries.Sum(s => s.CompletedCount);
        var totalPerf = totalSlots > 0 ? totalDone * 100.0 / totalSlots : 0;

        return new MonthlyStatisticsDto
        {
            Year = year,
            Month = month,
            TotalRecordedDays = totalRecorded,
            TechnicalDoneDays = technical,
            SpeakingDoneDays = speaking,
            GrammarDoneDays = grammar,
            SleepDoneDays = sleep,
            AverageDailyCompletionPercent = Math.Round(avgDaily, 1),
            FullCompletionDays = full,
            ZeroCompletionDays = zero,
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

    /// <summary>Pazartesi=0 … Pazar=6 olacak şekilde ayın ilk gününe kadar geri sayım.</summary>
    private static int GetMondayBasedOffset(DateOnly firstOfMonth)
    {
        var dow = firstOfMonth.DayOfWeek;
        return dow switch
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
