namespace CalismaTakip.Models;

/// <summary>Belirli bir güne ait günlük takip üst kaydı (not + satırlar).</summary>
public class DailyPlanTrackHeader
{
    public int Id { get; set; }

    public DateOnly TrackDate { get; set; }

    /// <summary>O gün oluşturulurken kullanılan şablon türü.</summary>
    public PlanTemplateKind TemplateKind { get; set; }

    public string Note { get; set; } = string.Empty;

    public ICollection<DailyPlanTrackItem> Items { get; set; } = new List<DailyPlanTrackItem>();
}
