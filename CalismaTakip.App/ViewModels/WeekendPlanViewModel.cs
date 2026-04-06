using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CalismaTakip.Helpers;
using CalismaTakip.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CalismaTakip.ViewModels;

public partial class WeekendPlanViewModel : ObservableObject
{
    private readonly IWeeklyPlanService _planService;

    public WeekendPlanViewModel(IWeeklyPlanService planService)
    {
        _planService = planService;
        _ = LoadAsync();
    }

    public ObservableCollection<WeekendPlanRowViewModel> Rows { get; } = new();

    [ObservableProperty]
    private string? _statusMessage;

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            StatusMessage = "Kaydediliyor…";
            await _planService.SaveWeekendAsync(Rows.ToList()).ConfigureAwait(true);
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
            var rows = await _planService.GetWeekendRowsAsync().ConfigureAwait(true);
            Rows.Clear();
            foreach (var r in rows)
                Rows.Add(r);
            StatusMessage = rows.Count == 0 ? "Plan verisi bulunamadı." : null;
        }
        catch (Exception ex)
        {
            StatusMessage = "Yükleme hatası.";
            UserMessage.ShowError(ex.InnerException?.Message ?? ex.Message);
        }
    }
}
