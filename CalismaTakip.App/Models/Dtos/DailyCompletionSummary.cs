namespace CalismaTakip.Models.Dtos;

/// <summary>Bir günün dört hedefe göre özet tamamlanma bilgisi.</summary>
public sealed class DailyCompletionSummary
{
    public static DailyCompletionSummary Empty(DateOnly date) =>
        new()
        {
            Date = date,
            HasRecord = false,
            TechnicalDone = false,
            SpeakingDone = false,
            GrammarDone = false,
            SleepDone = false,
            CompletedCount = 0,
            CompletionPercent = 0,
            Note = string.Empty
        };

    public required DateOnly Date { get; init; }

    public bool HasRecord { get; init; }

    public bool TechnicalDone { get; init; }

    public bool SpeakingDone { get; init; }

    public bool GrammarDone { get; init; }

    public bool SleepDone { get; init; }

    public int CompletedCount { get; init; }

    /// <summary>0–100 arası; tamamlanan / 4 * 100.</summary>
    public double CompletionPercent { get; init; }

    public string Note { get; init; } = string.Empty;
}
