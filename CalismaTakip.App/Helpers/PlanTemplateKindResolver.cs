using CalismaTakip.Models;

namespace CalismaTakip.Helpers;

public static class PlanTemplateKindResolver
{
    public static PlanTemplateKind ResolveForDate(DateOnly date) => date.DayOfWeek switch
    {
        DayOfWeek.Saturday => PlanTemplateKind.WeekendSaturday,
        DayOfWeek.Sunday => PlanTemplateKind.WeekendSunday,
        _ => PlanTemplateKind.Weekday
    };

    public static string ToTurkishPlanLabel(PlanTemplateKind kind) => kind switch
    {
        PlanTemplateKind.Weekday => "Hafta İçi",
        PlanTemplateKind.WeekendSaturday => "Hafta Sonu (Cumartesi)",
        PlanTemplateKind.WeekendSunday => "Hafta Sonu (Pazar)",
        _ => "Plan"
    };
}
