namespace CalismaTakip.Models.Dtos;

/// <summary>Belirli bir takvim ayı için toplanan istatistikler.</summary>
public sealed class MonthlyStatisticsDto
{
    public int Year { get; init; }

    public int Month { get; init; }

    /// <summary>O ayda en az bir kayıt bulunan gün sayısı.</summary>
    public int TotalRecordedDays { get; init; }

    public int TechnicalDoneDays { get; init; }

    public int SpeakingDoneDays { get; init; }

    public int GrammarDoneDays { get; init; }

    public int SleepDoneDays { get; init; }

    /// <summary>Kayıtlı günler üzerinden ortalama günlük tamamlama yüzdesi.</summary>
    public double AverageDailyCompletionPercent { get; init; }

    /// <summary>4/4 tamamlanan kayıtlı gün sayısı.</summary>
    public int FullCompletionDays { get; init; }

    /// <summary>Kayıt varken hiçbir hedefin işaretlenmediği gün sayısı (0/4).</summary>
    public int ZeroCompletionDays { get; init; }

    /// <summary>Ay içindeki kayıtlı günler arasında en yüksek günlük tamamlama yüzdesi.</summary>
    public double BestDayCompletionPercent { get; init; }

    /// <summary>Tüm tamamlanan hedefler / (kayıtlı gün * 4) * 100.</summary>
    public double TotalPerformancePercent { get; init; }
}
