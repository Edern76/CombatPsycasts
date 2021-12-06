using System.Reflection;
using CombatPsycasts.Comps;
using CombatPsycasts.Utils;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace CombatPsycasts.Harmony.Patches
{
    [HarmonyPatch(typeof(Projectile))]
    [HarmonyPatch("Tick")]
    public class PatchProjectileTick
    {
        /// <summary> Redirects homing projectile to target's current position if it's different from it's last known position.</summary>
        [HarmonyPrefix]
        public static bool Prefix(Projectile __instance)
        {
            LocalTargetInfo intendedTarget = __instance.intendedTarget;
            if (intendedTarget.HasThing && !intendedTarget.ThingDestroyed &&
                __instance.TryGetComp<Comp_HomingProjectile>() is Comp_HomingProjectile comp && !intendedTarget.Thing.Position.Equals(comp.lastCell))
            {
                FieldInfo ticksToImpactField = AccessTools.Field(typeof(Projectile), "ticksToImpact");
                FieldInfo destinationField = AccessTools.Field(typeof(Projectile), "destination");
                FieldInfo originField = AccessTools.Field(typeof(Projectile), "origin");
                if (comp.ShouldRecalculateNow())
                {
                    Vector3 oldDestination = (Vector3)destinationField.GetValue(__instance);
                    Vector3 newDestination = intendedTarget.Thing.Position.ToVector3Shifted();
                    Vector3 curPosition = __instance.ExactPosition;
                    int newTicksToImpact = Mathf.CeilToInt(ProjectileUtils.RecalculateNewTicksToImpact(__instance,newDestination));
                    float dist = (oldDestination - newDestination).magnitude;

                    originField.SetValue(__instance, curPosition);
                    destinationField.SetValue(__instance, newDestination);
                    ticksToImpactField.SetValue(__instance, newTicksToImpact);
                    
                    comp.SetLastCell(intendedTarget.Thing.Position);
                }    
            }    
            return true; //Run vanilla method
        }
    }
}