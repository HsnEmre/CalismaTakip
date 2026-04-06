namespace CalismaTakip.ViewModels;

/// <summary>Geçmiş tablosu ve detay paneli için salt okunur satır.</summary>
public sealed class TakipGecmisiRowViewModel
{
    public TakipGecmisiRowViewModel(
        int recordId,
        DateOnly date,
        string dateDisplay,
        string technicalDisplay,
        string speakingDisplay,
        string grammarDisplay,
        string sleepDisplay,
        string note)
    {
        RecordId = recordId;
        Date = date;
        DateDisplay = dateDisplay ?? string.Empty;
        TechnicalDisplay = technicalDisplay ?? string.Empty;
        SpeakingDisplay = speakingDisplay ?? string.Empty;
        GrammarDisplay = grammarDisplay ?? string.Empty;
        SleepDisplay = sleepDisplay ?? string.Empty;
        Note = note ?? string.Empty;
    }

    public int RecordId { get; }

    public DateOnly Date { get; }

    public string DateDisplay { get; }

    public string TechnicalDisplay { get; }

    public string SpeakingDisplay { get; }

    public string GrammarDisplay { get; }

    public string SleepDisplay { get; }

    public string Note { get; }
}
