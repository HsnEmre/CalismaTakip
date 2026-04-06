using System.Windows.Media;
using CalismaTakip.Helpers;
using CalismaTakip.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CalismaTakip.ViewModels;

public partial class DailyPlanTrackRowViewModel : ObservableObject
{
    private static readonly Brush DoneBrush = CreateFrozenBrush(0xE8, 0xF5, 0xE9);
    private static readonly Brush NeutralBrush = Brushes.White;

    private readonly Action? _onChanged;

    public DailyPlanTrackRowViewModel(PlanTemplateItem template, Action? onChanged)
    {
        ArgumentNullException.ThrowIfNull(template);
        _onChanged = onChanged;
        ItemId = 0;
        StartTime = template.StartTime;
        EndTime = template.EndTime;
        Title = template.Title ?? string.Empty;
        SortOrder = template.SortOrder;
        TimeRangeText = TimeRangeFormatter.Format(StartTime, EndTime);
        _isCompleted = false;
        _statusText = "Yapılmadı";
        RowBackground = NeutralBrush;
    }

    public DailyPlanTrackRowViewModel(DailyPlanTrackItem item, Action? onChanged)
    {
        ArgumentNullException.ThrowIfNull(item);
        _onChanged = onChanged;
        ItemId = item.Id;
        StartTime = item.StartTime;
        EndTime = item.EndTime;
        Title = item.Title ?? string.Empty;
        SortOrder = item.SortOrder;
        TimeRangeText = TimeRangeFormatter.Format(StartTime, EndTime);
        _isCompleted = item.IsCompleted;
        _statusText = item.IsCompleted ? "Yapıldı" : "Yapılmadı";
        RowBackground = item.IsCompleted ? DoneBrush : NeutralBrush;
    }

    public int ItemId { get; }

    public TimeSpan StartTime { get; }

    public TimeSpan EndTime { get; }

    public string Title { get; }

    public int SortOrder { get; }

    public string TimeRangeText { get; }

    [ObservableProperty]
    private bool _isCompleted;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private Brush _rowBackground = NeutralBrush;

    partial void OnIsCompletedChanged(bool value)
    {
        StatusText = value ? "Yapıldı" : "Yapılmadı";
        RowBackground = value ? DoneBrush : NeutralBrush;
        _onChanged?.Invoke();
    }

    private static SolidColorBrush CreateFrozenBrush(byte r, byte g, byte b)
    {
        var bch = new SolidColorBrush(Color.FromRgb(r, g, b));
        bch.Freeze();
        return bch;
    }
}
