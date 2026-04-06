namespace CalismaTakip.Models;

/// <summary>Hafta içi veya hafta sonu günü için varsayılan plan satırı şablonu.</summary>
public class PlanTemplateItem
{
    public int Id { get; set; }

    public PlanTemplateKind TemplateKind { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public string Title { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
