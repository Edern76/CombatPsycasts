using System;
using System.Reflection;
using CombatPsycasts.Defs.DefModExtensions;
using HarmonyLib;
using Verse;

namespace CombatPsycasts.Harmony.Patches
{
    [HarmonyPatch(typeof(BattleLogEntry_RangedImpact))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] {typeof(Thing), typeof(Thing), typeof(Thing), typeof(ThingDef), typeof(ThingDef), typeof(ThingDef)})]
    public class PatchBattleLogRangedImpact
    {
        [HarmonyPostfix]
        public static void Postfix(BattleLogEntry_RangedImpact __instance)
        {
            FieldInfo weaponDefField = AccessTools.Field(typeof(BattleLogEntry_RangedImpact), "weaponDef");
            FieldInfo projectileDefField = AccessTools.Field(typeof(BattleLogEntry_RangedImpact), "projectileDef");

            ThingDef weaponDef = (ThingDef)weaponDefField.GetValue(__instance);
            ThingDef projectileDef = (ThingDef)projectileDefField.GetValue(__instance);

            if (weaponDef == null && projectileDef != null && projectileDef.HasModExtension<FallbackBattleLogWeapon>())
            {
                ThingDef fallbackWeaponDef = projectileDef.GetModExtension<FallbackBattleLogWeapon>().fallbackWeapon;
                weaponDefField.SetValue(__instance, fallbackWeaponDef);
            }    
        }
    }
}