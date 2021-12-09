using System.Linq;
using CombatPsycasts.Utils;
using RimWorld;
using Verse;
using Verse.AI;
using HediffDefOf = CombatPsycasts.DefOfs.HediffDefOf;

namespace CombatPsycasts.Comps
{
    public class CompProperties_AbilityPsychicChoke : CompProperties_AbilityEffect
    {
        public JobDef sustainedJobDef;
        public int maxSustainTicks = 600;
        
        public CompProperties_AbilityPsychicChoke()
        {
            this.compClass = typeof(CompAbilityEffect_PsychicChoke);
        }    
    }    
    
    public class CompAbilityEffect_PsychicChoke : CompAbilityBase_CombatPsychic
    {
        public int TicksSinceLastSecond = 0;
        public bool ShouldBeChoking = false;
        public LocalTargetInfo curTarget;

        public CompProperties_AbilityPsychicChoke Props => (CompProperties_AbilityPsychicChoke)this.props;
        
        private bool ThingIsStillAlive()
        {
            return this.curTarget.HasThing && !this.curTarget.ThingDestroyed && this.curTarget.Pawn is Pawn pawn && pawn.health.State != PawnHealthState.Dead;
        }

        public bool ShouldContinueChoking() =>
            this.parent.CanCast && this.parent.pawn.drafter.Drafted && ThingIsStillAlive();

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest) =>
            base.CanApplyOn(target, dest) && target.Pawn is Pawn pawn 
                                          && pawn.health.hediffSet
                                                        .GetNotMissingParts()
                                                        .Any(p => p.def == BodyPartDefOf.Neck);

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (!ShouldBeChoking)
            {
                this.ShouldBeChoking = true;
                this.curTarget = target;
                if (ThingIsStillAlive())
                {
                    if (this.curTarget.Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.CP_Hediff_PsychicChoke) is
                        Hediff hediff)
                    {
                        hediff.TryGetComp<HediffComp_ReducesOverTime>().ShouldReduce = false;
                    }    
                    HediffUtils.AddOrUpdateHediffWithSeverity(this.curTarget.Pawn, HediffDefOf.CP_Hediff_PsychicChoke, this.curTarget.Pawn.def.race.body.GetPartsWithDef(BodyPartDefOf.Neck).FirstOrDefault(), 0.05f);
                    Job toStart = JobMaker.MakeJob(Props.sustainedJobDef, this.parent.pawn, curTarget);
                    this.parent.pawn.jobs.TryTakeOrderedJob(toStart);
                }
            }    
        }

        public void SustainedTick()
        {
            this.TicksSinceLastSecond++;
            if (ShouldBeChoking && ShouldContinueChoking() && this.TicksSinceLastSecond.TicksToSeconds() > 1.0)
            {
                ShouldBeChoking = this.parent.pawn.psychicEntropy.TryAddEntropy(this.parent.def.EntropyGain);

                if (ShouldBeChoking)
                {
                    HediffUtils.AddOrUpdateHediffWithSeverity(this.curTarget.Pawn, HediffDefOf.CP_Hediff_PsychicChoke, this.curTarget.Pawn.def.race.body.GetPartsWithDef(BodyPartDefOf.Neck).FirstOrDefault(), 0.10f);
                }

                this.TicksSinceLastSecond = 0;
            }    
        }
    }
}