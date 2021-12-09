using CombatPsycasts.Interfaces;
using HarmonyLib;
using Verse.AI;

namespace CombatPsycasts.Harmony.Patches
{
    [HarmonyPatch(typeof(JobDriver))]
    [HarmonyPatch("EndJobWith")]
    public class PatchJobDriverEndJob
    {
        [HarmonyPrefix]
        public static bool Prefix(JobDriver __instance)
        {
            if (__instance is IJobWithEndAction job)
            {
                job.OnEnd();
            }

            return true;
        }
    }
}