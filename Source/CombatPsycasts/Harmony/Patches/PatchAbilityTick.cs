using System.Linq;
using CombatPsycasts.Interfaces;
using HarmonyLib;
using RimWorld;

namespace CombatPsycasts.Harmony.Patches
{
    [HarmonyPatch(typeof(Ability))]
    [HarmonyPatch("AbilityTick")]
    public class PatchAbilityTick
    {
        [HarmonyPostfix]
        public static void Postfix(Ability __instance)
        {
            if (__instance?.comps == null) return;
            foreach (var tickable in __instance.comps.OfType<ITickable>()) tickable.DoTick();
        }
    }
}