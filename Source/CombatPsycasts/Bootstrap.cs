using CombatPsycasts.Harmony;
using Verse;

namespace CombatPsycasts
{
    [StaticConstructorOnStartup]
    public static class Bootstrap
    {
        static Bootstrap()
        {
            HarmonyBase.ApplyPatches();
            Log.Message("[CombatPsycasts] Done intialization");
        }    
    }
}