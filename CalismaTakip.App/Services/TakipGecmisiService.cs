using System.Globalization;
using CalismaTakip.Data;
using CalismaTakip.Models;
using CalismaTakip.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CalismaTakip.Services;

public class TakipGecmisiService : ITakipGecmisiService
{
    private const string KeyTechnical = "technical";
    private const string KeySpeaking = "speaking";
    private const string KeyGrammar = "grammar";
    private const string KeySleep = "sleep";

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

        var records = await db.DailyCheckRecords
            .AsNoTracking()
            .Include(r => r.Completions)
            .ThenInclude(c => c.DailyCheckDefinition)
            .Where(r => r.Date >= start && r.Date <= end)
            .OrderByDescending(r => r.Date)
            .ToListAsync(cancellationToken);

        var rows = records.Select(MapRow).ToList();

        return new TakipGecmisiQueryResult
        {
            Rows = rows,
            TotalRecordDays = records.Count,
            TechnicalDoneDays = records.Count(r => IsCompleted(r, KeyTechnical)),
            SpeakingDoneDays = records.Count(r => IsCompleted(r, KeySpeaking)),
            GrammarDoneDays = records.Count(r => IsCompleted(r, KeyGrammar)),
            SleepDoneDays = records.Count(r => IsCompleted(r, KeySleep))
        };
    }

    private static bool IsCompleted(DailyCheckRecord record, string key)
    {
        if (record.Completions == null || record.Completions.Count == 0)
            return false;

        foreach (var c in record.Completions)
        {
            var defKey = c.DailyCheckDefinition?.Key;
            if (string.Equals(defKey, key, StringComparison.OrdinalIgnoreCase))
                return c.IsCompleted;
        }

        return false;
    }

    private static TakipGecmisiRowViewModel MapRow(DailyCheckRecord record)
    {
        var date = record.Date;
        var dateDisplay = date.ToString("dd.MM.yyyy", CultureInfo.GetCultureInfo("tr-TR"));
        var note = record.Note ?? string.Empty;

        return new TakipGecmisiRowViewModel(
            record.Id,
            date,
            dateDisplay,
            ToDisplay(IsCompleted(record, KeyTechnical)),
            ToDisplay(IsCompleted(record, KeySpeaking)),
            ToDisplay(IsCompleted(record, KeyGrammar)),
            ToDisplay(IsCompleted(record, KeySleep)),
            note);
    }

    private static string ToDisplay(bool done) => done ? "Yapıldı" : "Yapılmadı";
}
