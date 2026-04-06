using CalismaTakip.Models;

namespace CalismaTakip.Data;

/// <summary>PlanTemplateItems tablosu için varsayılan satırlar (mevcut haftalık plan metinleriyle uyumlu).</summary>
internal static class PlanTemplateSeed
{
    internal static void SeedIfEmpty(AppDbContext db)
    {
        if (db.PlanTemplateItems.Any())
            return;

        foreach (var row in GetWeekdayRows())
        {
            db.PlanTemplateItems.Add(new PlanTemplateItem
            {
                TemplateKind = PlanTemplateKind.Weekday,
                StartTime = row.Start,
                EndTime = row.End,
                Title = row.Title,
                SortOrder = row.Order
            });
        }

        foreach (var row in GetWeekendSaturdayRows())
        {
            db.PlanTemplateItems.Add(new PlanTemplateItem
            {
                TemplateKind = PlanTemplateKind.WeekendSaturday,
                StartTime = row.Start,
                EndTime = row.End,
                Title = row.Title,
                SortOrder = row.Order
            });
        }

        foreach (var row in GetWeekendSundayRows())
        {
            db.PlanTemplateItems.Add(new PlanTemplateItem
            {
                TemplateKind = PlanTemplateKind.WeekendSunday,
                StartTime = row.Start,
                EndTime = row.End,
                Title = row.Title,
                SortOrder = row.Order
            });
        }
    }

    private static IEnumerable<(TimeSpan Start, TimeSpan End, int Order, string Title)> GetWeekdayRows()
    {
        var specs = new (TimeSpan Start, TimeSpan End, int Order)[]
        {
            (new TimeSpan(6, 30, 0), new TimeSpan(7, 0, 0), 1),
            (new TimeSpan(7, 0, 0), new TimeSpan(8, 0, 0), 2),
            (new TimeSpan(8, 0, 0), new TimeSpan(9, 0, 0), 3),
            (new TimeSpan(9, 0, 0), new TimeSpan(9, 15, 0), 4),
            (new TimeSpan(9, 15, 0), new TimeSpan(10, 0, 0), 5),
            (new TimeSpan(10, 0, 0), new TimeSpan(13, 0, 0), 6),
            (new TimeSpan(13, 0, 0), new TimeSpan(14, 0, 0), 7),
            (new TimeSpan(14, 0, 0), new TimeSpan(17, 0, 0), 8),
            (new TimeSpan(17, 0, 0), new TimeSpan(18, 30, 0), 9),
            (new TimeSpan(18, 30, 0), new TimeSpan(19, 30, 0), 10),
            (new TimeSpan(19, 30, 0), new TimeSpan(19, 45, 0), 11),
            (new TimeSpan(19, 45, 0), new TimeSpan(20, 45, 0), 12),
            (new TimeSpan(20, 45, 0), new TimeSpan(22, 0, 0), 13),
            (new TimeSpan(22, 0, 0), new TimeSpan(22, 30, 0), 14),
            (new TimeSpan(22, 30, 0), new TimeSpan(23, 0, 0), 15)
        };

        var titles = new[]
        {
            "Uyanış",
            "Kahvaltı / hazırlanma",
            "Teknik çalışma",
            "Mola",
            "Teknik çalışma",
            "Şirket işleri",
            "Öğle yemeği / ev işleri",
            "Şirket işleri",
            "Yemek hazırlığı / yemek",
            "İngilizce speaking",
            "Mola",
            "İngilizce grammar",
            "Serbest zaman",
            "Uykuya hazırlık",
            "Uyku"
        };

        for (var i = 0; i < specs.Length; i++)
        {
            var s = specs[i];
            var title = i < titles.Length ? titles[i] : string.Empty;
            yield return (s.Start, s.End, s.Order, title);
        }
    }

    private static IEnumerable<(TimeSpan Start, TimeSpan End, int Order, string Title)> GetWeekendSaturdayRows()
    {
        var specs = new (TimeSpan Start, TimeSpan End, int Order)[]
        {
            (new TimeSpan(8, 0, 0), new TimeSpan(9, 0, 0), 1),
            (new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 2),
            (new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0), 3),
            (new TimeSpan(13, 0, 0), new TimeSpan(14, 30, 0), 4),
            (new TimeSpan(14, 30, 0), new TimeSpan(17, 30, 0), 5),
            (new TimeSpan(17, 30, 0), new TimeSpan(19, 0, 0), 6),
            (new TimeSpan(19, 0, 0), new TimeSpan(20, 0, 0), 7),
            (new TimeSpan(20, 0, 0), new TimeSpan(21, 0, 0), 8),
            (new TimeSpan(21, 0, 0), new TimeSpan(22, 0, 0), 9),
            (new TimeSpan(22, 0, 0), new TimeSpan(23, 0, 0), 10)
        };

        var titles = new[]
        {
            "Kahvaltı / hazırlanma",
            "Teknik çalışma (3 saat)",
            "Öğle yemeği",
            "Mola / ev işleri",
            "Serbest zaman",
            "Yemek hazırlığı / yemek",
            "İngilizce speaking",
            "İngilizce grammar",
            "İngilizce / tekrar",
            "Uykuya hazırlık / uyku"
        };

        for (var i = 0; i < specs.Length; i++)
        {
            var s = specs[i];
            var title = i < titles.Length ? titles[i] : string.Empty;
            yield return (s.Start, s.End, s.Order, title);
        }
    }

    private static IEnumerable<(TimeSpan Start, TimeSpan End, int Order, string Title)> GetWeekendSundayRows()
    {
        var specs = new (TimeSpan Start, TimeSpan End, int Order)[]
        {
            (new TimeSpan(8, 0, 0), new TimeSpan(9, 0, 0), 1),
            (new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 2),
            (new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0), 3),
            (new TimeSpan(13, 0, 0), new TimeSpan(14, 30, 0), 4),
            (new TimeSpan(14, 30, 0), new TimeSpan(17, 30, 0), 5),
            (new TimeSpan(17, 30, 0), new TimeSpan(19, 0, 0), 6),
            (new TimeSpan(19, 0, 0), new TimeSpan(20, 0, 0), 7),
            (new TimeSpan(20, 0, 0), new TimeSpan(21, 0, 0), 8),
            (new TimeSpan(21, 0, 0), new TimeSpan(22, 0, 0), 9),
            (new TimeSpan(22, 0, 0), new TimeSpan(23, 0, 0), 10)
        };

        var titles = new[]
        {
            "Kahvaltı / hazırlanma",
            "Teknik çalışma (3 saat)",
            "Öğle yemeği",
            "Mola / dinlenme",
            "Serbest zaman / planlama",
            "Akşam yemeği",
            "İngilizce speaking",
            "İngilizce grammar",
            "Hafif tekrar / serbest",
            "Uykuya hazırlık / uyku"
        };

        for (var i = 0; i < specs.Length; i++)
        {
            var s = specs[i];
            var title = i < titles.Length ? titles[i] : string.Empty;
            yield return (s.Start, s.End, s.Order, title);
        }
    }
}
