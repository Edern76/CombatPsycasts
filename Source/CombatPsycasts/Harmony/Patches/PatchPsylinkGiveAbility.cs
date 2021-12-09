using System.Linq;
using CombatPsycasts.Comps;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatPsycasts.Harmony.Patches
{
    [HarmonyPatch(typeof(Hediff_Psylink))]
    [HarmonyPatch("TryGiveAbilityOfLevel")]
    public class PatchPsylinkGiveAbility
    {
        /// <summary> Prevents pacifists from learning combat psycasts.</summary>
        [HarmonyPrefix]
        public static bool Prefix(Hediff_Psylink __instance, int abilityLevel, bool sendLetter)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.WorkTagIsDisabled(WorkTags.Violent) ||
                StatDefOf.ShootingAccuracyPawn.Worker.IsDisabledFor(pawn))
            {
                if (!pawn.abilities.abilities.Any<Ability>(a => a.def.level == abilityLevel))
                {
                    string letterContent;
                    AbilityDef abilityDef = DefDatabase<AbilityDef>.AllDefs
                        .Where(a => a.level == abilityLevel && !(a.comps.Any(comp => comp.compClass.IsAssignableFrom(typeof(CompAbilityBase_CombatPsychic)))))
                        .RandomElementWithFallback();
                    if (abilityDef != null)
                    {
                        pawn.abilities.GainAbility(abilityDef);
                        letterContent = Hediff_Psylink.MakeLetterTextNewPsylinkLevel(pawn, abilityLevel,
                            Gen.YieldSingle<AbilityDef>(abilityDef));
                    }
                    else
                    {
                        letterContent = Hediff_Psylink.MakeLetterTextNewPsylinkLevel(pawn, abilityLevel);
                    }

                    if (sendLetter && PawnUtility.ShouldSendNotificationAbout(pawn))
                    {
                        string letterTitle = (string) ("LetterLabelPsylinkLevelGained".Translate() + ": " + pawn.LabelShortCap);
                        Find.LetterStack.ReceiveLetter((TaggedString)letterTitle, (TaggedString)letterContent, LetterDefOf.PositiveEvent, (LookTargets) (Thing) pawn);
                    }

                    return false; // Skips Vanilla method
                }    
            }

            return true;
        }
    }
}