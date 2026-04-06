namespace CalismaTakip.Models;

/// <summary>Belirli bir güne ait takip kaydı ve not.</summary>
public class DailyCheckRecord
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public string Note { get; set; } = string.Empty;

    public ICollection<DailyCheckCompletion> Completions { get; set; } = new List<DailyCheckCompletion>();
}
