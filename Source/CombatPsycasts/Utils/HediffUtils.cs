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
    }
}