namespace CalismaTakip.Models;

/// <summary>Belirli bir gün ve saat bloğu için planlanan aktivite metni.</summary>
public class WeeklyPlanItem
{
    public int Id { get; set; }

    public int TimeSlotId { get; set; }

    public TimeSlot? TimeSlot { get; set; }

    public PlanKind PlanKind { get; set; }

    /// <summary>Pazartesi–Pazar (.NET <see cref="DayOfWeek"/>).</summary>
    public DayOfWeek DayOfWeek { get; set; }

    public string ActivityText { get; set; } = string.Empty;
}
