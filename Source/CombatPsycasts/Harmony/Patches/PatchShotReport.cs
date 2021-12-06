using System.Reflection;
using CombatPsycasts.Verbs;
using HarmonyLib;
using Verse;

namespace CombatPsycasts.Harmony.Patches
{
    [HarmonyPatch(typeof(ShotReport))]
    [HarmonyPatch("HitReportFor")]
    public class PatchShotReport
    {
        /// <summary> Makes it so the chance of a psychic projectile hitting the intended target is independant from the pawn shooting skill </summary>
        [HarmonyPostfix]
        public static void Postfix(ref ShotReport __result, Thing caster, Verb verb, LocalTargetInfo target)
        {
            if (verb is Verb_PsychicShoot)
            {
                AccessTools.Field(typeof(ShotReport), "factorFromShooterAndDist")
                    .SetValue(__result, 1f);
            }    
        }
    }
}