using CommunityToolkit.Mvvm.ComponentModel;

namespace CalismaTakip.ViewModels;

public partial class WeekendPlanRowViewModel : ObservableObject
{
    public WeekendPlanRowViewModel(int timeSlotId, string timeRangeLabel)
    {
        TimeSlotId = timeSlotId;
        TimeRangeLabel = string.IsNullOrWhiteSpace(timeRangeLabel) ? string.Empty : timeRangeLabel;
    }

    public int TimeSlotId { get; }

    public string TimeRangeLabel { get; }

    [ObservableProperty]
    private string _saturdayActivity = string.Empty;

    [ObservableProperty]
    private string _sundayActivity = string.Empty;
}
