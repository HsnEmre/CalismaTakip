namespace CalismaTakip.Models;

/// <summary>Günlük takipte tek bir zaman dilimi satırı.</summary>
public class DailyPlanTrackItem
{
    public int Id { get; set; }

    public int HeaderId { get; set; }

    public DailyPlanTrackHeader? Header { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public string Title { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public bool IsCompleted { get; set; }
}
