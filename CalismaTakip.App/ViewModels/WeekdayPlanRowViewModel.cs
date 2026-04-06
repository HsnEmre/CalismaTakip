using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CalismaTakip.ViewModels;

public partial class WeekdayPlanRowViewModel : ObservableObject
{
    public WeekdayPlanRowViewModel(int timeSlotId, string timeRangeLabel)
    {
        TimeSlotId = timeSlotId;
        TimeRangeLabel = string.IsNullOrWhiteSpace(timeRangeLabel) ? string.Empty : timeRangeLabel;
    }

    public int TimeSlotId { get; }

    public string TimeRangeLabel { get; }

    [ObservableProperty]
    private string _mondayActivity = string.Empty;

    [ObservableProperty]
    private string _tuesdayActivity = string.Empty;

    [ObservableProperty]
    private string _wednesdayActivity = string.Empty;

    [ObservableProperty]
    private string _thursdayActivity = string.Empty;

    [ObservableProperty]
    private string _fridayActivity = string.Empty;

    public string? GetActivity(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => MondayActivity,
        DayOfWeek.Tuesday => TuesdayActivity,
        DayOfWeek.Wednesday => WednesdayActivity,
        DayOfWeek.Thursday => ThursdayActivity,
        DayOfWeek.Friday => FridayActivity,
        _ => string.Empty
    };
}
