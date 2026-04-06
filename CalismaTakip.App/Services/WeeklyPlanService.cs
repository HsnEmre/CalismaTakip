using System.Collections.ObjectModel;
using CalismaTakip.Data;
using CalismaTakip.Helpers;
using CalismaTakip.Models;
using CalismaTakip.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CalismaTakip.Services;

public class WeeklyPlanService : IWeeklyPlanService
{
    private static readonly DayOfWeek[] Weekdays =
    {
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday
    };

    private readonly IDbContextFactory<AppDbContext> _factory;

    public WeeklyPlanService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<ObservableCollection<WeekdayPlanRowViewModel>> GetWeekdayRowsAsync(
        CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var slots = await db.TimeSlots
            .AsNoTracking()
            .Where(t => t.PlanKind == PlanKind.Weekday)
            .OrderBy(t => t.SortOrder)
            .ToListAsync(cancellationToken);

        var items = await db.WeeklyPlanItems
            .AsNoTracking()
            .Where(w => w.PlanKind == PlanKind.Weekday)
            .ToListAsync(cancellationToken);

        var bySlot = items.GroupBy(i => i.TimeSlotId).ToDictionary(g => g.Key, g => g.ToList());

        var rows = new ObservableCollection<WeekdayPlanRowViewModel>();
        foreach (var slot in slots)
        {
            var label = TimeRangeFormatter.Format(slot.StartTime, slot.EndTime);
            var row = new WeekdayPlanRowViewModel(slot.Id, label);

            if (bySlot.TryGetValue(slot.Id, out var list))
            {
                foreach (var it in list)
                {
                    var text = it.ActivityText ?? string.Empty;
                    switch (it.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            row.MondayActivity = text;
                            break;
                        case DayOfWeek.Tuesday:
                            row.TuesdayActivity = text;
                            break;
                        case DayOfWeek.Wednesday:
                            row.WednesdayActivity = text;
                            break;
                        case DayOfWeek.Thursday:
                            row.ThursdayActivity = text;
                            break;
                        case DayOfWeek.Friday:
                            row.FridayActivity = text;
                            break;
                    }
                }
            }

            rows.Add(row);
        }

        return rows;
    }

    public async Task SaveWeekdayAsync(IReadOnlyList<WeekdayPlanRowViewModel> rows, CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        foreach (var row in rows)
        {
            foreach (var day in Weekdays)
            {
                var text = row.GetActivity(day) ?? string.Empty;
                var item = await db.WeeklyPlanItems.FirstOrDefaultAsync(
                    w => w.PlanKind == PlanKind.Weekday
                         && w.TimeSlotId == row.TimeSlotId
                         && w.DayOfWeek == day,
                    cancellationToken);

                if (item == null)
                {
                    db.WeeklyPlanItems.Add(new WeeklyPlanItem
                    {
                        TimeSlotId = row.TimeSlotId,
                        PlanKind = PlanKind.Weekday,
                        DayOfWeek = day,
                        ActivityText = text
                    });
                }
                else
                {
                    item.ActivityText = text;
                }
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<ObservableCollection<WeekendPlanRowViewModel>> GetWeekendRowsAsync(
        CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var slots = await db.TimeSlots
            .AsNoTracking()
            .Where(t => t.PlanKind == PlanKind.Weekend)
            .OrderBy(t => t.SortOrder)
            .ToListAsync(cancellationToken);

        var items = await db.WeeklyPlanItems
            .AsNoTracking()
            .Where(w => w.PlanKind == PlanKind.Weekend)
            .ToListAsync(cancellationToken);

        var bySlot = items.GroupBy(i => i.TimeSlotId).ToDictionary(g => g.Key, g => g.ToList());

        var rows = new ObservableCollection<WeekendPlanRowViewModel>();
        foreach (var slot in slots)
        {
            var label = TimeRangeFormatter.Format(slot.StartTime, slot.EndTime);
            var row = new WeekendPlanRowViewModel(slot.Id, label);

            if (bySlot.TryGetValue(slot.Id, out var list))
            {
                foreach (var it in list)
                {
                    var text = it.ActivityText ?? string.Empty;
                    if (it.DayOfWeek == DayOfWeek.Saturday)
                        row.SaturdayActivity = text;
                    else if (it.DayOfWeek == DayOfWeek.Sunday)
                        row.SundayActivity = text;
                }
            }

            rows.Add(row);
        }

        return rows;
    }

    public async Task SaveWeekendAsync(IReadOnlyList<WeekendPlanRowViewModel> rows, CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        foreach (var row in rows)
        {
            await UpsertWeekendCellAsync(db, row.TimeSlotId, DayOfWeek.Saturday, row.SaturdayActivity, cancellationToken);
            await UpsertWeekendCellAsync(db, row.TimeSlotId, DayOfWeek.Sunday, row.SundayActivity, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task UpsertWeekendCellAsync(
        AppDbContext db,
        int timeSlotId,
        DayOfWeek day,
        string? text,
        CancellationToken cancellationToken)
    {
        var safe = text ?? string.Empty;
        var item = await db.WeeklyPlanItems.FirstOrDefaultAsync(
            w => w.PlanKind == PlanKind.Weekend
                 && w.TimeSlotId == timeSlotId
                 && w.DayOfWeek == day,
            cancellationToken);

        if (item == null)
        {
            db.WeeklyPlanItems.Add(new WeeklyPlanItem
            {
                TimeSlotId = timeSlotId,
                PlanKind = PlanKind.Weekend,
                DayOfWeek = day,
                ActivityText = safe
            });
        }
        else
        {
            item.ActivityText = safe;
        }
    }
}
