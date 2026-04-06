namespace CalismaTakip.Models;

/// <summary>Saat aralığı tanımı; hafta içi veya hafta sonu planına bağlıdır.</summary>
public class TimeSlot
{
    public int Id { get; set; }

    public PlanKind PlanKind { get; set; }

    /// <summary>Gün içi başlangıç (ör. 06:30).</summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>Gün içi bitiş.</summary>
    public TimeSpan EndTime { get; set; }

    public int SortOrder { get; set; }

    public ICollection<WeeklyPlanItem> WeeklyPlanItems { get; set; } = new List<WeeklyPlanItem>();
}
