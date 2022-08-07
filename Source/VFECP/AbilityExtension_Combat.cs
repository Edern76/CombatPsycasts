using RimWorld;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace VFECP
{
    public class AbilityExtension_Combat : AbilityExtension_AbilityMod
    {
        public override bool IsEnabledForPawn(Ability ability, out string reason)
        {
            if (ability.pawn.WorkTagIsDisabled(WorkTags.Violent) ||
                StatDefOf.ShootingAccuracyPawn.Worker.IsDisabledFor(ability.pawn))
            {
                reason = "IsIncapableOfViolence".Translate(ability.pawn);
                return false;
            }

            return base.IsEnabledForPawn(ability, out reason);
        }
    }
}