using Verse;

namespace CombatPsycasts
{
    [StaticConstructorOnStartup]
    public static class Bootstrap
    {
        static Bootstrap()
        {
            Log.Message("[CombatPsycasts] Done intialization");
        }    
    }
}