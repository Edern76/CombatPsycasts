using CombatPsycasts.Comps;
using Verse;

namespace CombatPsycasts.Utils
{
    public static class HediffUtils
    {
        public static void AddOrUpdateHediffWithSeverity(Pawn pawn, HediffDef hediff, BodyPartRecord part, float severity)
        {
            Pawn_HealthTracker health = pawn.health;
            HediffSet hediffSet = health.hediffSet;
            if (hediffSet.GetFirstHediffOfDef(hediff) == null)
            {
                health.AddHediff(hediff, part);
            }

            hediffSet.GetFirstHediffOfDef(hediff).Severity += severity;

        }

        public static Hediff FindLethalInstigatorHediff(this Pawn pawn)
        {
            return pawn?.health?.hediffSet?.hediffs.Find(
                hediff => hediff.TryGetComp<HediffComp_HasInstigator>() is HediffComp_HasInstigator comp && comp.Props.isLethal);
        }
    }
}