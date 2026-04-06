using System.Globalization;
using CalismaTakip.Helpers;
using CalismaTakip.Models.Dtos;
using CalismaTakip.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CalismaTakip.ViewModels;

public partial class StatisticsViewModel : ObservableObject
{
    private readonly ICalendarStatisticsService _calendarService;
    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    public StatisticsViewModel(ICalendarStatisticsService calendarService)
    {
        _calendarService = calendarService;
        var today = DateOnly.FromDateTime(DateTime.Today);
        _displayYear = today.Year;
        _displayMonth = today.Month;
        _stats = new MonthlyStatisticsDto { Year = _displayYear, Month = _displayMonth };
        _ = LoadAsync();
    }

    [ObservableProperty]
    private int _displayYear;

    [ObservableProperty]
    private int _displayMonth;

    [ObservableProperty]
    private string _monthTitle = string.Empty;

    [ObservableProperty]
    private MonthlyStatisticsDto _stats;

    [ObservableProperty]
    private string? _statusMessage;

    /// <summary>Ay içinde tamamlanan / planlanan tüm görev slotları oranı (%).</summary>
    [ObservableProperty]
    private double _taskSlotCompletionPercent;

    [ObservableProperty]
    private double _averageCompletionBar;

    [ObservableProperty]
    private double _totalPerformanceBar;

    [ObservableProperty]
    private double _bestDayBar;

    [RelayCommand]
    private async Task PreviousMonthAsync()
    {
        var d = new DateOnly(DisplayYear, DisplayMonth, 1).AddMonths(-1);
        DisplayYear = d.Year;
        DisplayMonth = d.Month;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task NextMonthAsync()
    {
        var d = new DateOnly(DisplayYear, DisplayMonth, 1).AddMonths(1);
        DisplayYear = d.Year;
        DisplayMonth = d.Month;
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            StatusMessage = "Yükleniyor…";
            UpdateMonthTitle();

            var dto = await _calendarService
                .GetMonthlyStatisticsAsync(DisplayYear, DisplayMonth)
                .ConfigureAwait(true);

            Stats = dto;
            ApplyBarValues(dto);
            StatusMessage = dto.TotalRecordedDays == 0 ? "Bu ay için kayıt bulunamadı." : null;
        }
        catch (Exception ex)
        {
            Stats = new MonthlyStatisticsDto { Year = DisplayYear, Month = DisplayMonth };
            ApplyBarValues(Stats);
            StatusMessage = "İstatistikler yüklenemedi.";
            UserMessage.ShowError(ex.InnerException?.Message ?? ex.Message);
        }
    }

    private void ApplyBarValues(MonthlyStatisticsDto d)
    {
        TaskSlotCompletionPercent = d.TotalTasksScheduledInMonth <= 0
            ? 0
            : Math.Round(d.TotalTasksCompletedInMonth * 100.0 / d.TotalTasksScheduledInMonth, 1);

        AverageCompletionBar = d.AverageDailyCompletionPercent;
        TotalPerformanceBar = d.TotalPerformancePercent;
        BestDayBar = d.BestDayCompletionPercent;
    }

    private void UpdateMonthTitle()
    {
        var name = Turkish.DateTimeFormat.GetMonthName(DisplayMonth);
        if (string.IsNullOrWhiteSpace(name))
            name = DisplayMonth.ToString(Turkish);
        MonthTitle = $"{name} {DisplayYear}";
    }
}
