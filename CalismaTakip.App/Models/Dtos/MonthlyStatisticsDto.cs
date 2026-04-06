namespace CalismaTakip.Models.Dtos;

/// <summary>Belirli bir takvim ayı için plan satırı bazlı istatistikler.</summary>
public sealed class MonthlyStatisticsDto
{
    public int Year { get; init; }

    public int Month { get; init; }

    /// <summary>En az bir plan satırı kaydı olan gün sayısı.</summary>
    public int TotalRecordedDays { get; init; }

    /// <summary>Ay içinde işaretlenen toplam “yapıldı” satırı.</summary>
    public int TotalTasksCompletedInMonth { get; init; }

    /// <summary>Ay içindeki kayıtlı günlerdeki toplam plan satırı.</summary>
    public int TotalTasksScheduledInMonth { get; init; }

    /// <summary>Kayıtlı günler üzerinden ortalama günlük tamamlama yüzdesi.</summary>
    public double AverageDailyCompletionPercent { get; init; }

    /// <summary>Tüm satırları tamamlanan gün sayısı.</summary>
    public int FullCompletionDays { get; init; }

    /// <summary>Kayıt varken hiçbir satır tamamlanmayan gün sayısı.</summary>
    public int ZeroCompletionDays { get; init; }

    /// <summary>Kısmen tamamlanan gün sayısı (0 &lt; oran &lt; 100).</summary>
    public int PartialCompletionDays { get; init; }

    public double BestDayCompletionPercent { get; init; }

    /// <summary>Tamamlanan satır / planlanan satır * 100 (ay geneli).</summary>
    public double TotalPerformancePercent { get; init; }
}
