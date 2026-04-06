using CalismaTakip.Models;
using Microsoft.EntityFrameworkCore;

namespace CalismaTakip.Data;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public DatabaseInitializer(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public void Initialize()
    {
        using var db = _factory.CreateDbContext();
        db.Database.EnsureCreated();
        PlanTrackSchemaInstaller.EnsureInstalled(db);

        if (!db.TimeSlots.Any())
        {
            SeedWeeklyPlan(db);
            db.SaveChanges();
        }

        PlanTemplateSeed.SeedIfEmpty(db);
        db.SaveChanges();
    }

    private static void SeedWeeklyPlan(AppDbContext db)
    {
        var weekdaySlots = SeedWeekdaySlots(db);
        SeedWeekdayPlanItems(db, weekdaySlots);
        var weekendSlots = SeedWeekendSlots(db);
        SeedWeekendPlanItems(db, weekendSlots);
    }

    private static List<TimeSlot> SeedWeekdaySlots(AppDbContext db)
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

        var list = new List<TimeSlot>();
        foreach (var (start, end, order) in specs)
        {
            var slot = new TimeSlot
            {
                PlanKind = PlanKind.Weekday,
                StartTime = start,
                EndTime = end,
                SortOrder = order
            };
            db.TimeSlots.Add(slot);
            list.Add(slot);
        }

        return list;
    }

    private static readonly string[] WeekdayDefaultRowTexts =
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

    private static void SeedWeekdayPlanItems(AppDbContext db, IReadOnlyList<TimeSlot> slots)
    {
        var days = new[]
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };

        for (var i = 0; i < slots.Count; i++)
        {
            var text = i < WeekdayDefaultRowTexts.Length ? WeekdayDefaultRowTexts[i] : string.Empty;
            foreach (var day in days)
            {
                db.WeeklyPlanItems.Add(new WeeklyPlanItem
                {
                    TimeSlot = slots[i],
                    PlanKind = PlanKind.Weekday,
                    DayOfWeek = day,
                    ActivityText = text
                });
            }
        }
    }

    private static List<TimeSlot> SeedWeekendSlots(AppDbContext db)
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

        var list = new List<TimeSlot>();
        foreach (var (start, end, order) in specs)
        {
            var slot = new TimeSlot
            {
                PlanKind = PlanKind.Weekend,
                StartTime = start,
                EndTime = end,
                SortOrder = order
            };
            db.TimeSlots.Add(slot);
            list.Add(slot);
        }

        return list;
    }

    private static readonly string[] WeekendSaturdayTexts =
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

    private static readonly string[] WeekendSundayTexts =
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

    private static void SeedWeekendPlanItems(AppDbContext db, IReadOnlyList<TimeSlot> slots)
    {
        for (var i = 0; i < slots.Count; i++)
        {
            var sat = i < WeekendSaturdayTexts.Length ? WeekendSaturdayTexts[i] : string.Empty;
            var sun = i < WeekendSundayTexts.Length ? WeekendSundayTexts[i] : string.Empty;

            db.WeeklyPlanItems.Add(new WeeklyPlanItem
            {
                TimeSlot = slots[i],
                PlanKind = PlanKind.Weekend,
                DayOfWeek = DayOfWeek.Saturday,
                ActivityText = sat
            });
            db.WeeklyPlanItems.Add(new WeeklyPlanItem
            {
                TimeSlot = slots[i],
                PlanKind = PlanKind.Weekend,
                DayOfWeek = DayOfWeek.Sunday,
                ActivityText = sun
            });
        }
    }
}
