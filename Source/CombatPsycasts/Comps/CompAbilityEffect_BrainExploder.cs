using RimWorld;
using Verse;
using Verse.Sound;
using SoundDefOf = CombatPsycasts.DefOfs.SoundDefOf;

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
    
    public class CompAbilityEffect_BrainExploder : CompAbilityBase_CombatPsychic
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
                    SoundDefOf.CP_HeadExplosion.PlayOneShot(SoundInfo.InMap((TargetInfo)(Thing)target));
                    target.health.DropBloodFilth();
                    target.TakeDamage(toApply);
                }
                
            }    
        }
    }
}