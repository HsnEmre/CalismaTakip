using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CalismaTakip.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly WeekdayPlanViewModel _weekday;
    private readonly WeekendPlanViewModel _weekend;
    private readonly DailyTrackingViewModel _daily;
    private readonly TakipGecmisiViewModel _history;

    public MainViewModel(
        WeekdayPlanViewModel weekday,
        WeekendPlanViewModel weekend,
        DailyTrackingViewModel daily,
        TakipGecmisiViewModel history)
    {
        _weekday = weekday;
        _weekend = weekend;
        _daily = daily;
        _history = history;
        CurrentPage = weekday;
    }

    [ObservableProperty]
    private object? _currentPage;

    [RelayCommand]
    private void ShowWeekday() => CurrentPage = _weekday;

    [RelayCommand]
    private void ShowWeekend() => CurrentPage = _weekend;

    [RelayCommand]
    private void ShowDaily() => CurrentPage = _daily;

    [RelayCommand]
    private void ShowHistory() => CurrentPage = _history;
}
