using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using CalismaTakip.Helpers;
using CalismaTakip.Models;
using CalismaTakip.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CalismaTakip.ViewModels;

public partial class DailyTrackingViewModel : ObservableObject
{
    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    private readonly IDailyTrackingService _trackingService;

    public DailyTrackingViewModel(IDailyTrackingService trackingService)
    {
        _trackingService = trackingService;
        _ = LoadAsync();
    }

    public ObservableCollection<DailyPlanTrackRowViewModel> PlanRows { get; } = new();

    [ObservableProperty]
    private DateTime? _selectedDateUi = DateTime.Today;

    [ObservableProperty]
    private string _note = string.Empty;

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private string _activePlanLabel = string.Empty;

    [ObservableProperty]
    private string _dateHeaderLine = string.Empty;

    [ObservableProperty]
    private int _totalTasks;

    [ObservableProperty]
    private int _completedTasks;

    [ObservableProperty]
    private double _completionPercentValue;

    [ObservableProperty]
    private string _completionPercentDisplay = "0%";

    public DateOnly SelectedDate =>
        SelectedDateUi.HasValue
            ? DateOnly.FromDateTime(SelectedDateUi.Value.Date)
            : DateOnly.FromDateTime(DateTime.Today);

    public void NotifyRowChanged() => RefreshSummary();

    public void ApplyShellFromDate(PlanTemplateKind kind)
    {
        ActivePlanLabel = $"Aktif plan: {PlanTemplateKindResolver.ToTurkishPlanLabel(kind)}";
        var d = SelectedDate;
        var dayName = Turkish.DateTimeFormat.GetDayName(d.DayOfWeek);
        if (string.IsNullOrWhiteSpace(dayName))
            dayName = d.DayOfWeek.ToString();
        DateHeaderLine = $"{d:dd.MM.yyyy} - {dayName}";
    }

    public void RefreshSummary()
    {
        TotalTasks = PlanRows.Count;
        CompletedTasks = PlanRows.Count(r => r.IsCompleted);
        CompletionPercentValue = TotalTasks == 0
            ? 0
            : Math.Round(CompletedTasks * 100.0 / TotalTasks, 1);
        CompletionPercentDisplay = $"{CompletionPercentValue:0.#}%";
    }

    partial void OnSelectedDateUiChanged(DateTime? value)
    {
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            StatusMessage = "Kaydediliyor…";
            await _trackingService.SaveAsync(this).ConfigureAwait(true);
            await LoadAsync().ConfigureAwait(true);
            StatusMessage = "Kaydedildi.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Kayıt başarısız.";
            UserMessage.ShowError(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [RelayCommand]
    private async Task ResetPlanAsync()
    {
        var confirm = MessageBox.Show(
            "Bu tarih için kayıtlı işaretlemeler silinecek ve plan, şablondan yeniden yüklenecek. Devam edilsin mi?",
            "Planı yenile",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes)
            return;

        try
        {
            StatusMessage = "Yenileniyor…";
            await _trackingService.DeleteTrackForDateAsync(SelectedDate).ConfigureAwait(true);
            await LoadAsync().ConfigureAwait(true);
            StatusMessage = "Plan şablondan yenilendi.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Yenileme başarısız.";
            UserMessage.ShowError(ex.InnerException?.Message ?? ex.Message);
        }
    }

    private async Task LoadAsync()
    {
        try
        {
            await _trackingService.LoadIntoAsync(this).ConfigureAwait(true);
            RefreshSummary();
            StatusMessage = null;
        }
        catch (Exception ex)
        {
            PlanRows.Clear();
            RefreshSummary();
            StatusMessage = "Yükleme hatası.";
            UserMessage.ShowError(ex.InnerException?.Message ?? ex.Message);
        }
    }
}
