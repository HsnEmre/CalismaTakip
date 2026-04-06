namespace CalismaTakip.Models;

/// <summary>Bir günlük kayıtta tanım bazlı tamamlanma durumu.</summary>
public class DailyCheckCompletion
{
    public int Id { get; set; }

    public int DailyCheckRecordId { get; set; }

    public DailyCheckRecord? DailyCheckRecord { get; set; }

    public int DailyCheckDefinitionId { get; set; }

    public DailyCheckDefinition? DailyCheckDefinition { get; set; }

    public bool IsCompleted { get; set; }
}
