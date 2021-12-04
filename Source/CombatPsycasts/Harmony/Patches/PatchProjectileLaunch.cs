using System;
using CombatPsycasts.Comps;
using CombatPsycasts.Defs.DefModExtensions;
using CombatPsycasts.Utils;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace CombatPsycasts.Harmony.Patches
{
    [HarmonyPatch(typeof(Projectile))]
    [HarmonyPatch("Launch")]
    [HarmonyPatch(new Type[] {typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(bool), typeof(Thing), typeof(ThingDef)})]
    public class PatchProjectileLaunch
    {
        [HarmonyPostfix]
        public static void Postfix(Projectile __instance,
            Thing launcher,
            Vector3 origin,
            LocalTargetInfo usedTarget,
            LocalTargetInfo intendedTarget,
            bool preventFriendlyFire,
            Thing equipment,
            ThingDef targetCoverDef)
        {
            if (__instance.def.HasModExtension<PreciseProjectile>())
            {
                float chanceToRedirect = __instance.def.GetModExtension<PreciseProjectile>().forceHitTargetChance;
                if (Rand.Chance(chanceToRedirect))
                {
                    if (usedTarget != intendedTarget)
                    {
                        Vector3 newDest = intendedTarget.Cell.ToVector3Shifted();
                        AccessTools.Field(typeof(Projectile), "destination")
                            .SetValue(__instance, newDest);
                        AccessTools.Field(typeof(Projectile), "ticksToImpact").SetValue(__instance, Mathf.CeilToInt(ProjectileUtils.RecalculateNewTicksToImpact(__instance, newDest)));
                        __instance.HitFlags = ProjectileHitFlags.IntendedTarget | ProjectileHitFlags.NonTargetPawns;
                        __instance.usedTarget = intendedTarget;
                    }

                    if (intendedTarget.HasThing && !intendedTarget.ThingDestroyed)
                    {
                        Comp_HomingProjectile comp = __instance.TryGetComp<Comp_HomingProjectile>();
                        comp?.SetHoming(true);
                        comp?.SetLastCell(intendedTarget.Thing.Position);
                    }

                }    
            }    
        }
    }
}