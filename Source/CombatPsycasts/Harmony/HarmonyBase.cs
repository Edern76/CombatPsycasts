namespace CombatPsycasts.Harmony
{
    public class HarmonyBase
    {
        private static HarmonyLib.Harmony harmonyInstance = new HarmonyLib.Harmony("com.combatpsycats.patch");

        public static void ApplyPatches()
        {
            harmonyInstance.PatchAll();
        }
    }
}