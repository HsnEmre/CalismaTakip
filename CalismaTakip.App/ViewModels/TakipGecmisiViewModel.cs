using System.Collections.ObjectModel;
using CalismaTakip.Helpers;
using CalismaTakip.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CalismaTakip.ViewModels;

public partial class TakipGecmisiViewModel : ObservableObject
{
    private readonly ITakipGecmisiService _historyService;

    public TakipGecmisiViewModel(ITakipGecmisiService historyService)
    {
        _historyService = historyService;
        var today = DateTime.Today;
        _filterStartDate = today.AddDays(-30);
        _filterEndDate = today;
        _ = ApplyFilterAsync();
    }

    public ObservableCollection<TakipGecmisiRowViewModel> Rows { get; } = new();

    [ObservableProperty]
    private DateTime? _filterStartDate;

    [ObservableProperty]
    private DateTime? _filterEndDate;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedRow))]
    private TakipGecmisiRowViewModel? _selectedRow;

    public bool HasSelectedRow => SelectedRow is not null;

    public bool ShowSelectRowHint => !HasNoRecords && SelectedRow is null;

    partial void OnSelectedRowChanged(TakipGecmisiRowViewModel? value)
    {
        OnPropertyChanged(nameof(ShowSelectRowHint));
    }

    partial void OnHasNoRecordsChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowSelectRowHint));
    }

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private bool _hasNoRecords;

    [ObservableProperty]
    private int _totalRecordDays;

    [ObservableProperty]
    private double _averageDailyCompletionPercent;

    [ObservableProperty]
    private int _fullyCompletedDays;

    [ObservableProperty]
    private int _totalTasksCompleted;

    [ObservableProperty]
    private int _totalTasksScheduled;

    [RelayCommand]
    private async Task ApplyFilterAsync()
    {
        try
        {
            StatusMessage = "Yükleniyor…";
            var start = ToDateOnly(FilterStartDate) ?? DateOnly.FromDateTime(DateTime.Today.AddDays(-30));
            var end = ToDateOnly(FilterEndDate) ?? DateOnly.FromDateTime(DateTime.Today);

            var result = await _historyService.GetHistoryAsync(start, end).ConfigureAwait(true);

            Rows.Clear();
            foreach (var row in result.Rows)
                Rows.Add(row);

            HasNoRecords = Rows.Count == 0;
            TotalRecordDays = result.TotalRecordDays;
            AverageDailyCompletionPercent = result.AverageDailyCompletionPercent;
            FullyCompletedDays = result.FullyCompletedDays;
            TotalTasksCompleted = result.TotalTasksCompleted;
            TotalTasksScheduled = result.TotalTasksScheduled;

            SelectedRow = null;
            StatusMessage = HasNoRecords ? "Kayıt bulunamadı." : null;
        }
        catch (Exception ex)
        {
            HasNoRecords = true;
            Rows.Clear();
            SelectedRow = null;
            StatusMessage = "Yükleme hatası.";
            UserMessage.ShowError(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [RelayCommand]
    private async Task ClearFilterAsync()
    {
        var today = DateTime.Today;
        FilterStartDate = today.AddDays(-30);
        FilterEndDate = today;
        await ApplyFilterAsync();
    }

    private static DateOnly? ToDateOnly(DateTime? value)
    {
        if (!value.HasValue)
            return null;
        return DateOnly.FromDateTime(value.Value.Date);
    }
}
