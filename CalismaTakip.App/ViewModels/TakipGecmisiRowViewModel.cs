namespace CalismaTakip.ViewModels;

public sealed class TakipGecmisiRowViewModel
{
    public TakipGecmisiRowViewModel(
        int headerId,
        DateOnly date,
        string dateDisplay,
        string taskRatioText,
        string percentText,
        string note)
    {
        HeaderId = headerId;
        Date = date;
        DateDisplay = dateDisplay ?? string.Empty;
        TaskRatioText = taskRatioText ?? string.Empty;
        PercentText = percentText ?? string.Empty;
        Note = note ?? string.Empty;
    }

    public int HeaderId { get; }

    public DateOnly Date { get; }

    public string DateDisplay { get; }

    public string TaskRatioText { get; }

    public string PercentText { get; }

    public string Note { get; }
}
