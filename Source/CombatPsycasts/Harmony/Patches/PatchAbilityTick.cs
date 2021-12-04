using System.Linq;
using CombatPsycasts.Comps;
using CombatPsycasts.Interfaces;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatPsycasts.Harmony.Patches
{
    [HarmonyPatch(typeof(Ability))]
    [HarmonyPatch("AbilityTick")]
    public class PatchAbilityTick
    {
        [HarmonyPostfix]
        public static void Postfix(Ability __instance)
        {
            foreach (ITickable tickable in __instance.comps.Where(c => c is ITickable).Cast<ITickable>())
            {
                tickable.DoTick();
            }
        }
    }
}