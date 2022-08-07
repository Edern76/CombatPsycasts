using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;
using Ability = VFECore.Abilities.Ability;
using SoundDefOf = CombatPsycasts.DefOfs.SoundDefOf;

namespace VFECP
{
    public class Ability_BrainExploder : Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            foreach (var target in targets)
                if (target.Thing is Pawn p)
                    DoBrainExplosion(pawn, p);
        }

        protected void DoBrainExplosion(Pawn caster, Pawn target)
        {
            if (target?.health.hediffSet.GetBrain() is BodyPartRecord brain)
            {
                var toApply = new DamageInfo(DamageDefOf.Bomb, target.health.hediffSet.GetPartHealth(brain), 1f,
                    -1f, caster, brain);
                if (Rand.Chance(0.5f))
                {
                    toApply.SetAmount(999);
                    toApply.SetAllowDamagePropagation(true);
                    SoundDefOf.CP_HeadExplosion.PlayOneShot(SoundInfo.InMap((TargetInfo)(Thing)target));
                    target.health.DropBloodFilth();
                }
                else
                    toApply.SetAllowDamagePropagation(false);

                target.TakeDamage(toApply);
            }
        }
    }
}