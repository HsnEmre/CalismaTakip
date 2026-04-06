using CalismaTakip.Data;
using CalismaTakip.Helpers;
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
        var kind = PlanTemplateKindResolver.ResolveForDate(date);

        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var header = await db.DailyPlanTrackHeaders
            .AsNoTracking()
            .Include(h => h.Items)
            .FirstOrDefaultAsync(h => h.TrackDate == date, cancellationToken);

        viewModel.ApplyShellFromDate(kind);
        viewModel.PlanRows.Clear();

        if (header != null)
        {
            viewModel.Note = header.Note ?? string.Empty;
            foreach (var it in header.Items.OrderBy(i => i.SortOrder))
                viewModel.PlanRows.Add(new DailyPlanTrackRowViewModel(it, viewModel.NotifyRowChanged));
        }
        else
        {
            viewModel.Note = string.Empty;
            var templates = await db.PlanTemplateItems
                .AsNoTracking()
                .Where(t => t.TemplateKind == kind)
                .OrderBy(t => t.SortOrder)
                .ToListAsync(cancellationToken);

            foreach (var t in templates)
                viewModel.PlanRows.Add(new DailyPlanTrackRowViewModel(t, viewModel.NotifyRowChanged));
        }

        viewModel.RefreshSummary();
    }

    public async Task SaveAsync(DailyTrackingViewModel viewModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var date = viewModel.SelectedDate;
        var kind = PlanTemplateKindResolver.ResolveForDate(date);
        var note = viewModel.Note ?? string.Empty;

        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var header = await db.DailyPlanTrackHeaders
            .Include(h => h.Items)
            .FirstOrDefaultAsync(h => h.TrackDate == date, cancellationToken);

        if (header == null)
        {
            header = new DailyPlanTrackHeader
            {
                TrackDate = date,
                TemplateKind = kind,
                Note = note
            };
            db.DailyPlanTrackHeaders.Add(header);
        }
        else
        {
            header.Note = note;
            header.TemplateKind = kind;
            var existing = header.Items.ToList();
            db.DailyPlanTrackItems.RemoveRange(existing);
        }

        await db.SaveChangesAsync(cancellationToken);

        foreach (var row in viewModel.PlanRows.OrderBy(r => r.SortOrder))
        {
            db.DailyPlanTrackItems.Add(new DailyPlanTrackItem
            {
                HeaderId = header.Id,
                StartTime = row.StartTime,
                EndTime = row.EndTime,
                Title = row.Title,
                SortOrder = row.SortOrder,
                IsCompleted = row.IsCompleted
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTrackForDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);
        var header = await db.DailyPlanTrackHeaders
            .Include(h => h.Items)
            .FirstOrDefaultAsync(h => h.TrackDate == date, cancellationToken);

        if (header == null)
            return;

        db.DailyPlanTrackHeaders.Remove(header);
        await db.SaveChangesAsync(cancellationToken);
    }
}
