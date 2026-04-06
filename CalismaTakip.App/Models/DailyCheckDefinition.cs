namespace CalismaTakip.Models;

/// <summary>Günlük takip ekranındaki checkbox satırlarının tanımı (genişletilebilir).</summary>
public class DailyCheckDefinition
{
    public int Id { get; set; }

    /// <summary>Kod anahtarı (örn. technical).</summary>
    public string Key { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public ICollection<DailyCheckCompletion> Completions { get; set; } = new List<DailyCheckCompletion>();
}
