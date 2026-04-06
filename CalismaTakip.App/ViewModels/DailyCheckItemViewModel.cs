using CommunityToolkit.Mvvm.ComponentModel;

namespace CalismaTakip.ViewModels;

public partial class DailyCheckItemViewModel : ObservableObject
{
    public DailyCheckItemViewModel(int definitionId, string displayName)
    {
        DefinitionId = definitionId;
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? string.Empty : displayName;
    }

    public int DefinitionId { get; }

    public string DisplayName { get; }

    [ObservableProperty]
    private bool _isCompleted;
}
