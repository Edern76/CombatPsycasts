using CombatPsycasts.Verbs;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatPsycasts.Harmony.Patches
{
    [HarmonyPatch(typeof(ShieldBelt))]
    [HarmonyPatch("AllowVerbCast")]
    public class PatchShieldBelt
    {
        [HarmonyPostfix]
        public static void Postfix(Verb verb, ref bool __result)
        {
            if (verb is Verb_PsychicShoot)
            {
                __result = true;
            }
        }
    }
}