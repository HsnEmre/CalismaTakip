using System.Windows.Media;
using CalismaTakip.Models;
using CalismaTakip.Models.Dtos;

namespace CalismaTakip.Helpers;

public enum CompletionVisualTier
{
    Neutral,
    Weak,
    Medium,
    Good
}

/// <summary>DailyCheckRecord üzerinden tamamlanma hesapları ve görsel katman.</summary>
public static class DailyCompletionHelper
{
    public const string KeyTechnical = "technical";
    public const string KeySpeaking = "speaking";
    public const string KeyGrammar = "grammar";
    public const string KeySleep = "sleep";

    private static readonly Brush NeutralBrush = CreateBrush(0xE8, 0xEA, 0xEF);
    private static readonly Brush WeakBrush = CreateBrush(0xFD, 0xE2, 0xE1);
    private static readonly Brush MediumBrush = CreateBrush(0xFE, 0xF5, 0xD4);
    private static readonly Brush GoodBrush = CreateBrush(0xD8, 0xF3, 0xE0);
    private static readonly Brush SelectedBorderBrush = CreateBrush(0x25, 0x63, 0xEB);

    public static int GetCompletionCount(DailyCheckRecord? record)
    {
        if (record?.Completions == null || record.Completions.Count == 0)
            return 0;

        var n = 0;
        if (IsKeyCompleted(record, KeyTechnical)) n++;
        if (IsKeyCompleted(record, KeySpeaking)) n++;
        if (IsKeyCompleted(record, KeyGrammar)) n++;
        if (IsKeyCompleted(record, KeySleep)) n++;
        return n;
    }

    public static double GetCompletionRatePercent(DailyCheckRecord? record)
    {
        return GetCompletionCount(record) * 25.0;
    }

    public static double GetCompletionRatePercent(int completedCount)
    {
        if (completedCount < 0) completedCount = 0;
        if (completedCount > 4) completedCount = 4;
        return completedCount * 25.0;
    }

    public static DailyCompletionSummary ToSummary(DailyCheckRecord? record, DateOnly date)
    {
        if (record == null)
            return DailyCompletionSummary.Empty(date);

        var tech = IsKeyCompleted(record, KeyTechnical);
        var speak = IsKeyCompleted(record, KeySpeaking);
        var gram = IsKeyCompleted(record, KeyGrammar);
        var sleep = IsKeyCompleted(record, KeySleep);
        var count = (tech ? 1 : 0) + (speak ? 1 : 0) + (gram ? 1 : 0) + (sleep ? 1 : 0);

        return new DailyCompletionSummary
        {
            Date = date,
            HasRecord = true,
            TechnicalDone = tech,
            SpeakingDone = speak,
            GrammarDone = gram,
            SleepDone = sleep,
            CompletedCount = count,
            CompletionPercent = GetCompletionRatePercent(count),
            Note = record.Note ?? string.Empty
        };
    }

    public static CompletionVisualTier GetCompletionTier(DailyCompletionSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);
        if (!summary.HasRecord)
            return CompletionVisualTier.Neutral;

        return summary.CompletedCount switch
        {
            0 => CompletionVisualTier.Weak,
            1 or 2 => CompletionVisualTier.Medium,
            _ => CompletionVisualTier.Good
        };
    }

    public static Brush GetStatusBrush(DailyCompletionSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);
        var tier = GetCompletionTier(summary);
        return tier switch
        {
            CompletionVisualTier.Neutral => NeutralBrush,
            CompletionVisualTier.Weak => WeakBrush,
            CompletionVisualTier.Medium => MediumBrush,
            CompletionVisualTier.Good => GoodBrush,
            _ => NeutralBrush
        };
    }

    public static Brush GetSelectionBorderBrush() => SelectedBorderBrush;

    public static string ToDoneLabel(bool done) => done ? "Yapıldı" : "Yapılmadı";

    private static bool IsKeyCompleted(DailyCheckRecord record, string key)
    {
        if (record.Completions == null)
            return false;

        foreach (var c in record.Completions)
        {
            var k = c.DailyCheckDefinition?.Key;
            if (string.Equals(k, key, StringComparison.OrdinalIgnoreCase))
                return c.IsCompleted;
        }

        return false;
    }

    private static SolidColorBrush CreateBrush(byte r, byte g, byte b)
    {
        var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
        brush.Freeze();
        return brush;
    }
}
