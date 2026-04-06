using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CalismaTakip.Helpers;
using CalismaTakip.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CalismaTakip.ViewModels;

public partial class DailyTrackingViewModel : ObservableObject
{
    private readonly IDailyTrackingService _trackingService;

    public DailyTrackingViewModel(IDailyTrackingService trackingService)
    {
        _trackingService = trackingService;
        _ = LoadAsync();
    }

    [ObservableProperty]
    private DateTime? _selectedDateUi = DateTime.Today;

    [ObservableProperty]
    private string _note = string.Empty;

    [ObservableProperty]
    private string? _statusMessage;

    public ObservableCollection<DailyCheckItemViewModel> CheckItems { get; } = new();

    public DateOnly SelectedDate =>
        SelectedDateUi.HasValue
            ? DateOnly.FromDateTime(SelectedDateUi.Value.Date)
            : DateOnly.FromDateTime(DateTime.Today);

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
            StatusMessage = "Kaydedildi.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Kayıt başarısız.";
            UserMessage.ShowError(ex.InnerException?.Message ?? ex.Message);
        }
    }

    private async Task LoadAsync()
    {
        try
        {
            await _trackingService.LoadIntoAsync(this).ConfigureAwait(true);
            StatusMessage = null;
        }
        catch (Exception ex)
        {
            StatusMessage = "Yükleme hatası.";
            UserMessage.ShowError(ex.InnerException?.Message ?? ex.Message);
        }
    }
}
