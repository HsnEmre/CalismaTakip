using CalismaTakip.Models;
using Microsoft.EntityFrameworkCore;

namespace CalismaTakip.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();

    public DbSet<WeeklyPlanItem> WeeklyPlanItems => Set<WeeklyPlanItem>();

    public DbSet<PlanTemplateItem> PlanTemplateItems => Set<PlanTemplateItem>();

    public DbSet<DailyPlanTrackHeader> DailyPlanTrackHeaders => Set<DailyPlanTrackHeader>();

    public DbSet<DailyPlanTrackItem> DailyPlanTrackItems => Set<DailyPlanTrackItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TimeSlot>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.PlanKind).HasConversion<int>();
            e.HasIndex(x => new { x.PlanKind, x.SortOrder });
        });

        modelBuilder.Entity<WeeklyPlanItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.PlanKind).HasConversion<int>();
            e.Property(x => x.DayOfWeek).HasConversion<int>();
            e.Property(x => x.ActivityText).HasMaxLength(2000);
            e.HasOne(x => x.TimeSlot)
                .WithMany(x => x.WeeklyPlanItems)
                .HasForeignKey(x => x.TimeSlotId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.PlanKind, x.TimeSlotId, x.DayOfWeek }).IsUnique();
        });

        modelBuilder.Entity<PlanTemplateItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.TemplateKind).HasConversion<int>();
            e.Property(x => x.Title).HasMaxLength(2000);
            e.HasIndex(x => new { x.TemplateKind, x.SortOrder });
        });

        modelBuilder.Entity<DailyPlanTrackHeader>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.TemplateKind).HasConversion<int>();
            e.Property(x => x.Note).HasMaxLength(8000);
            e.HasIndex(x => x.TrackDate).IsUnique();
            e.HasMany(x => x.Items)
                .WithOne(x => x.Header)
                .HasForeignKey(x => x.HeaderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DailyPlanTrackItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(2000);
            e.HasIndex(x => new { x.HeaderId, x.SortOrder });
        });
    }
}
