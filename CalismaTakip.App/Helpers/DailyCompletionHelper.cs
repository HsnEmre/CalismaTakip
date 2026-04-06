using System.Windows.Media;
using CalismaTakip.Models.Dtos;

namespace CalismaTakip.Helpers;

public enum CompletionVisualTier
{
    Neutral,
    Weak,
    Medium,
    Good
}

/// <summary>Günlük plan tamamlanma oranı ve takvim renkleri.</summary>
public static class DailyCompletionHelper
{
    private static readonly Brush NeutralBrush = CreateBrush(0xE8, 0xEA, 0xEF);
    private static readonly Brush WeakBrush = CreateBrush(0xFD, 0xE2, 0xE1);
    private static readonly Brush MediumBrush = CreateBrush(0xFE, 0xF5, 0xD4);
    private static readonly Brush GoodBrush = CreateBrush(0xD8, 0xF3, 0xE0);
    private static readonly Brush SelectedBorderBrush = CreateBrush(0x25, 0x63, 0xEB);

    public static CompletionVisualTier GetCompletionTier(DailyCompletionSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);
        if (!summary.HasRecord || summary.TotalTaskCount == 0)
            return CompletionVisualTier.Neutral;

        var r = summary.CompletedTaskCount / (double)summary.TotalTaskCount;
        if (r <= 0)
            return CompletionVisualTier.Weak;
        if (r < 1)
            return CompletionVisualTier.Medium;
        return CompletionVisualTier.Good;
    }

    public static Brush GetStatusBrush(DailyCompletionSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);
        var tier = GetCompletionTier(summary);
        return tier switch
        {
            CompletionVisualTier.Neutral => NeutralBrush,
            CompletionVisualTier.Weak => WeakBrush,
            CompletionVisualTier.Medium => MediumBrush,
            CompletionVisualTier.Good => GoodBrush,
            _ => NeutralBrush
        };
    }

    public static Brush GetSelectionBorderBrush() => SelectedBorderBrush;

    private static SolidColorBrush CreateBrush(byte r, byte g, byte b)
    {
        var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
        brush.Freeze();
        return brush;
    }
}
