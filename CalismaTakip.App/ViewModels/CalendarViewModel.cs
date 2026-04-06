using System.Collections.ObjectModel;
using System.Globalization;
using CalismaTakip.Helpers;
using CalismaTakip.Models.Dtos;
using CalismaTakip.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CalismaTakip.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
    private readonly ICalendarStatisticsService _calendarService;
    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    public CalendarViewModel(ICalendarStatisticsService calendarService)
    {
        _calendarService = calendarService;
        var today = DateOnly.FromDateTime(DateTime.Today);
        _displayYear = today.Year;
        _displayMonth = today.Month;
        _ = ReloadAsync();
    }

    public ObservableCollection<CalendarDayCellViewModel> Cells { get; } = new();

    [ObservableProperty]
    private int _displayYear;

    [ObservableProperty]
    private int _displayMonth;

    [ObservableProperty]
    private string _monthTitle = string.Empty;

    [ObservableProperty]
    private DateOnly? _selectedDate;

    [ObservableProperty]
    private DailyCompletionSummary? _selectedDaySummary;

    [ObservableProperty]
    private bool _hasSelection;

    [ObservableProperty]
    private bool _hasDetailRecord;

    [ObservableProperty]
    private bool _showNoRecordMessage;

    [ObservableProperty]
    private string _selectedDateDisplay = string.Empty;

    [ObservableProperty]
    private string _detailTechnicalText = string.Empty;

    [ObservableProperty]
    private string _detailSpeakingText = string.Empty;

    [ObservableProperty]
    private string _detailGrammarText = string.Empty;

    [ObservableProperty]
    private string _detailSleepText = string.Empty;

    [ObservableProperty]
    private string _detailPercentText = string.Empty;

    [ObservableProperty]
    private string _detailNoteText = string.Empty;

    [ObservableProperty]
    private string? _statusMessage;

    [RelayCommand]
    private async Task PreviousMonthAsync()
    {
        var d = new DateOnly(DisplayYear, DisplayMonth, 1).AddMonths(-1);
        DisplayYear = d.Year;
        DisplayMonth = d.Month;
        await ReloadAsync();
    }

    [RelayCommand]
    private async Task NextMonthAsync()
    {
        var d = new DateOnly(DisplayYear, DisplayMonth, 1).AddMonths(1);
        DisplayYear = d.Year;
        DisplayMonth = d.Month;
        await ReloadAsync();
    }

    [RelayCommand]
    private async Task SelectDayAsync(CalendarDayCellViewModel? cell)
    {
        if (cell == null)
            return;

        SelectedDate = cell.Date;
        HasSelection = true;

        var summary = await _calendarService.GetCompletionSummaryAsync(cell.Date).ConfigureAwait(true);
        cell.SetSummary(summary);
        SelectedDaySummary = summary;
        HasDetailRecord = summary.HasRecord;
        ShowNoRecordMessage = !summary.HasRecord;
        SelectedDateDisplay = cell.Date.ToString("dd MMMM yyyy", Turkish);
        DetailTechnicalText = DailyCompletionHelper.ToDoneLabel(summary.TechnicalDone);
        DetailSpeakingText = DailyCompletionHelper.ToDoneLabel(summary.SpeakingDone);
        DetailGrammarText = DailyCompletionHelper.ToDoneLabel(summary.GrammarDone);
        DetailSleepText = DailyCompletionHelper.ToDoneLabel(summary.SleepDone);
        DetailPercentText = $"{summary.CompletionPercent:0}%";
        DetailNoteText = summary.Note ?? string.Empty;

        foreach (var c in Cells)
            c.RefreshSelectionState(SelectedDate);
    }

    private async Task ReloadAsync()
    {
        try
        {
            StatusMessage = "Yükleniyor…";
            UpdateMonthTitle();

            var items = await _calendarService
                .GetCalendarDaysForMonthAsync(DisplayYear, DisplayMonth)
                .ConfigureAwait(true);

            Cells.Clear();
            foreach (var item in items)
                Cells.Add(new CalendarDayCellViewModel(item));

            ClearSelectionUi();
            StatusMessage = null;
        }
        catch (Exception ex)
        {
            Cells.Clear();
            ClearSelectionUi();
            StatusMessage = "Takvim yüklenemedi.";
            UserMessage.ShowError(ex.InnerException?.Message ?? ex.Message);
        }
    }

    private void ClearSelectionUi()
    {
        SelectedDate = null;
        SelectedDaySummary = null;
        HasSelection = false;
        HasDetailRecord = false;
        ShowNoRecordMessage = false;
        SelectedDateDisplay = string.Empty;
        DetailTechnicalText = string.Empty;
        DetailSpeakingText = string.Empty;
        DetailGrammarText = string.Empty;
        DetailSleepText = string.Empty;
        DetailPercentText = string.Empty;
        DetailNoteText = string.Empty;
        foreach (var c in Cells)
            c.RefreshSelectionState(null);
    }

    private void UpdateMonthTitle()
    {
        var name = Turkish.DateTimeFormat.GetMonthName(DisplayMonth);
        if (string.IsNullOrWhiteSpace(name))
            name = DisplayMonth.ToString(Turkish);
        MonthTitle = $"{name} {DisplayYear}";
    }
}

