using RimWorld;
using Verse;

namespace CombatPsycasts.Comps
{
    public class CompProperties_AbilityBrainExploder : CompProperties_AbilityEffect
    {
        public CompProperties_AbilityBrainExploder()
        {
            this.compClass = typeof(CompAbilityEffect_BrainExploder);
        }

        public float headExplodeChance = 0.5f;
    }
    
    public class CompAbilityEffect_BrainExploder : CompAbilityEffect
    {
        public CompProperties_AbilityBrainExploder Props => (CompProperties_AbilityBrainExploder)this.props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            this.DoBrainExplosion(this.parent.pawn, target.Pawn);
        }

        protected void DoBrainExplosion(Pawn caster, Pawn target)
        {
            if (target?.health.hediffSet.GetBrain() is BodyPartRecord brain)
            {
                DamageInfo toApply = new DamageInfo(DamageDefOf.Bomb, target.health.hediffSet.GetPartHealth(brain), 1f,
                    -1f, caster, brain);
                if (Rand.Chance(this.Props.headExplodeChance))
                {
                    toApply.SetAmount(999);
                    toApply.SetAllowDamagePropagation(true);
                }
                else
                {
                    toApply.SetAllowDamagePropagation(false);
                }
                target.TakeDamage(toApply);
                
            }    
        }
    }
}