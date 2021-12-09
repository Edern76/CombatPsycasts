using RimWorld;
using Verse;

namespace CombatPsycasts.Comps
{
    public class CompAbilityBase_CombatPsychic : CompAbilityEffect
    {
        public override bool GizmoDisabled(out string reason)
        {
            if (parent.pawn.WorkTagIsDisabled(WorkTags.Violent) ||
                StatDefOf.ShootingAccuracyPawn.Worker.IsDisabledFor(parent.pawn))
            {
                reason = "Selected pawn cannot take part in combat.";
                return true;
            }

            return base.GizmoDisabled(out reason);
        }
    }
}