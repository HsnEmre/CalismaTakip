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

    public DbSet<DailyCheckDefinition> DailyCheckDefinitions => Set<DailyCheckDefinition>();

    public DbSet<DailyCheckRecord> DailyCheckRecords => Set<DailyCheckRecord>();

    public DbSet<DailyCheckCompletion> DailyCheckCompletions => Set<DailyCheckCompletion>();

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

        modelBuilder.Entity<DailyCheckDefinition>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Key).HasMaxLength(64).IsRequired();
            e.Property(x => x.DisplayName).HasMaxLength(256).IsRequired();
            e.HasIndex(x => x.Key).IsUnique();
        });

        modelBuilder.Entity<DailyCheckRecord>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Note).HasMaxLength(8000);
            e.HasIndex(x => x.Date).IsUnique();
        });

        modelBuilder.Entity<DailyCheckCompletion>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.DailyCheckRecord)
                .WithMany(x => x.Completions)
                .HasForeignKey(x => x.DailyCheckRecordId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.DailyCheckDefinition)
                .WithMany(x => x.Completions)
                .HasForeignKey(x => x.DailyCheckDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.DailyCheckRecordId, x.DailyCheckDefinitionId }).IsUnique();
        });
    }
}
