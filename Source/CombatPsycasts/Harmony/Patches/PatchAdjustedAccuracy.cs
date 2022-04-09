using System;
using HarmonyLib;
using Verse;

namespace CombatPsycasts.Harmony.Patches
{
    [HarmonyPatch(typeof(VerbProperties))]
    [HarmonyPatch("AdjustedAccuracy")]
    public class PatchAdjustedAccuracy
    {
        [HarmonyFinalizer]
        public static Exception Finalizer(Thing equipment, Exception __exception)
        {
            if (equipment == null && __exception is NullReferenceException)
            {
                return null; //Fix for conflict with mods not null-checking equipment in pre/postfixes
            }
            return __exception;
        }
    }
}