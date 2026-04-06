using CalismaTakip.Data;
using CalismaTakip.Models;
using CalismaTakip.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CalismaTakip.Services;

public class DailyTrackingService : IDailyTrackingService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public DailyTrackingService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task LoadIntoAsync(DailyTrackingViewModel viewModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var date = viewModel.SelectedDate;
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var definitions = await db.DailyCheckDefinitions
            .AsNoTracking()
            .OrderBy(d => d.SortOrder)
            .ToListAsync(cancellationToken);

        viewModel.CheckItems.Clear();

        foreach (var def in definitions)
        {
            viewModel.CheckItems.Add(new DailyCheckItemViewModel(def.Id, def.DisplayName));
        }

        var record = await db.DailyCheckRecords
            .AsNoTracking()
            .Include(r => r.Completions)
            .FirstOrDefaultAsync(r => r.Date == date, cancellationToken);

        viewModel.Note = record?.Note ?? string.Empty;

        if (record == null)
            return;

        foreach (var item in viewModel.CheckItems)
        {
            var match = record.Completions.FirstOrDefault(c => c.DailyCheckDefinitionId == item.DefinitionId);
            item.IsCompleted = match?.IsCompleted ?? false;
        }
    }

    public async Task SaveAsync(DailyTrackingViewModel viewModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var date = viewModel.SelectedDate;
        var note = viewModel.Note ?? string.Empty;

        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var record = await db.DailyCheckRecords
            .Include(r => r.Completions)
            .FirstOrDefaultAsync(r => r.Date == date, cancellationToken);

        if (record == null)
        {
            record = new DailyCheckRecord
            {
                Date = date,
                Note = note
            };
            db.DailyCheckRecords.Add(record);
        }
        else
        {
            record.Note = note;
        }

        var definitionIds = viewModel.CheckItems.Select(i => i.DefinitionId).ToHashSet();

        foreach (var item in viewModel.CheckItems)
        {
            var existing = record.Completions.FirstOrDefault(c => c.DailyCheckDefinitionId == item.DefinitionId);
            if (existing == null)
            {
                record.Completions.Add(new DailyCheckCompletion
                {
                    DailyCheckDefinitionId = item.DefinitionId,
                    IsCompleted = item.IsCompleted
                });
            }
            else
            {
                existing.IsCompleted = item.IsCompleted;
            }
        }

        var stale = record.Completions.Where(c => !definitionIds.Contains(c.DailyCheckDefinitionId)).ToList();
        foreach (var c in stale)
            db.DailyCheckCompletions.Remove(c);

        await db.SaveChangesAsync(cancellationToken);
    }
}
