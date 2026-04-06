using CalismaTakip.ViewModels;

namespace CalismaTakip.Services;

/// <summary>Filtrelenmiş takip geçmişi ve özet istatistikler.</summary>
public sealed class TakipGecmisiQueryResult
{
    public IReadOnlyList<TakipGecmisiRowViewModel> Rows { get; init; } = Array.Empty<TakipGecmisiRowViewModel>();

    public int TotalRecordDays { get; init; }

    public int TechnicalDoneDays { get; init; }

    public int SpeakingDoneDays { get; init; }

    public int GrammarDoneDays { get; init; }

    public int SleepDoneDays { get; init; }
}
