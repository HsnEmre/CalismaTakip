using System.IO;

namespace CalismaTakip.Data;

public static class DatabasePaths
{
    public static string GetDefaultDatabasePath()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CalismaTakip");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "app.db");
    }

    public static string GetConnectionString()
    {
        return $"Data Source={GetDefaultDatabasePath()}";
    }
}
