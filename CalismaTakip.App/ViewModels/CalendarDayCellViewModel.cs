using System.Windows.Media;
using CalismaTakip.Helpers;
using CalismaTakip.Models.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CalismaTakip.ViewModels;

public partial class CalendarDayCellViewModel : ObservableObject
{
    public CalendarDayCellViewModel(CalendarDayItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        Date = item.Date;
        IsCurrentMonth = item.BelongsToDisplayedMonth;
        Summary = item.Summary;
        ApplySummary();
        RefreshSelectionState(null);
    }

    public DateOnly Date { get; }

    public bool IsCurrentMonth { get; }

    public double LabelOpacity => IsCurrentMonth ? 1 : 0.42;

    public DailyCompletionSummary Summary { get; private set; }

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private Brush _selectionBorder = Brushes.Transparent;

    [ObservableProperty]
    private Brush _cellBackground = Brushes.Transparent;

    [ObservableProperty]
    private string _completionShortText = string.Empty;

    [ObservableProperty]
    private string _percentShortText = string.Empty;

    public void SetSummary(DailyCompletionSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);
        Summary = summary;
        ApplySummary();
    }

    public void RefreshSelectionState(DateOnly? selectedDate)
    {
        IsSelected = selectedDate.HasValue && selectedDate.Value == Date;
        SelectionBorder = IsSelected
            ? DailyCompletionHelper.GetSelectionBorderBrush()
            : Brushes.Transparent;
    }

    private void ApplySummary()
    {
        CellBackground = DailyCompletionHelper.GetStatusBrush(Summary);
        if (!Summary.HasRecord)
        {
            CompletionShortText = "—";
            PercentShortText = string.Empty;
        }
        else
        {
            CompletionShortText = $"{Summary.CompletedTaskCount}/{Summary.TotalTaskCount}";
            PercentShortText = $"{Summary.CompletionPercent:0}%";
        }
    }
}
