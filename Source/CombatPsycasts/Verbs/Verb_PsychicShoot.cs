using Verse;

namespace CombatPsycasts.Verbs
{
    public class Verb_PsychicShoot : Verb_Shoot
    {
        public override bool Available()
        {
            return true;
        }
    }
}