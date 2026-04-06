using System.Globalization;
using CalismaTakip.Data;
using CalismaTakip.Models;
using CalismaTakip.Models.Dtos;
using CalismaTakip.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CalismaTakip.Services;

public class TakipGecmisiService : ITakipGecmisiService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public TakipGecmisiService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<TakipGecmisiQueryResult> GetHistoryAsync(
        DateOnly start,
        DateOnly end,
        CancellationToken cancellationToken = default)
    {
        if (end < start)
            (start, end) = (end, start);

        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var headers = await db.DailyPlanTrackHeaders
            .AsNoTracking()
            .Include(h => h.Items)
            .Where(h => h.TrackDate >= start && h.TrackDate <= end)
            .OrderByDescending(h => h.TrackDate)
            .ToListAsync(cancellationToken);

        var culture = CultureInfo.GetCultureInfo("tr-TR");
        var rows = headers.Select(h => MapRow(h, culture)).ToList();

        var withTasks = headers.Where(h => h.Items.Count > 0).ToList();
        var avg = withTasks.Count == 0
            ? 0
            : withTasks.Average(h =>
            {
                var s = DailyCompletionSummary.FromHeader(h);
                return s.CompletionPercent;
            });

        var full = headers.Count(h => h.Items.Count > 0 && h.Items.All(i => i.IsCompleted));
        var sumDone = headers.Sum(h => h.Items.Count(i => i.IsCompleted));
        var sumTotal = headers.Sum(h => h.Items.Count);

        return new TakipGecmisiQueryResult
        {
            Rows = rows,
            TotalRecordDays = headers.Count,
            AverageDailyCompletionPercent = Math.Round(avg, 1),
            FullyCompletedDays = full,
            TotalTasksCompleted = sumDone,
            TotalTasksScheduled = sumTotal
        };
    }

    private static TakipGecmisiRowViewModel MapRow(DailyPlanTrackHeader header, CultureInfo culture)
    {
        var s = DailyCompletionSummary.FromHeader(header);
        var dateDisplay = header.TrackDate.ToString("dd.MM.yyyy", culture);
        var ratio = s.TotalTaskCount == 0
            ? "—"
            : $"{s.CompletedTaskCount}/{s.TotalTaskCount}";
        var pct = s.TotalTaskCount == 0 ? "—" : $"{s.CompletionPercent:0.#}%";

        return new TakipGecmisiRowViewModel(
            header.Id,
            header.TrackDate,
            dateDisplay,
            ratio,
            pct,
            header.Note ?? string.Empty);
    }
}
