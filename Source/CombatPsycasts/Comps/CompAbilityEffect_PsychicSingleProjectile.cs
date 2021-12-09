using CombatPsycasts.DefOfs;
using RimWorld;
using Verse;

namespace CombatPsycasts.Comps
{
    public class CompProperties_AbilityPsychicSingleProjectile : CompProperties_AbilityEffect
    {
        public ThingDef projectileDef = ProjectileDefOf.CP_Proj_PsychicGrenade;
        
        public CompProperties_AbilityPsychicSingleProjectile()
        {
            this.compClass = typeof(CompAbilityEffect_PsychicSingleProjectile);
        }
    }    
    public class CompAbilityEffect_PsychicSingleProjectile : CompAbilityBase_CombatPsychic
    {
        public CompProperties_AbilityPsychicSingleProjectile Props =>
            (CompProperties_AbilityPsychicSingleProjectile)this.props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (target != null && this.parent.pawn is Pawn caster)
            {
                bool shootLineFromTo =
                    this.parent.verb.TryFindShootLineFromTo(caster.Position, target, out ShootLine resultingLine);
                if (!shootLineFromTo) return;
                Projectile projectile = (Projectile)GenSpawn.Spawn(Props.projectileDef, resultingLine.Source, caster.Map);
                ProjectileHitFlags hitFlags = ProjectileHitFlags.All;

                projectile.Launch(caster, caster.DrawPos, target, target, hitFlags);
            }    

        }
    }
}