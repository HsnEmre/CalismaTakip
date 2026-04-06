using System.Windows;

namespace CalismaTakip.Helpers;

public static class UserMessage
{
    public static void ShowError(string message, string title = "Hata")
    {
        if (string.IsNullOrWhiteSpace(message))
            message = "Bilinmeyen bir hata oluştu.";
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static void ShowInfo(string message, string title = "Bilgi")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
