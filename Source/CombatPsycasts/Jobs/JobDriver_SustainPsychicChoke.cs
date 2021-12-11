using System.Collections.Generic;
using System.Linq;
using CombatPsycasts.Comps;
using CombatPsycasts.DefOfs;
using CombatPsycasts.Interfaces;
using Verse;
using Verse.AI;

namespace CombatPsycasts.Jobs
{
    public class JobDriver_SustainPsychicChoke : JobDriver, IJobWithEndAction
    {
        private TargetIndex indCaster => TargetIndex.A;
        private Pawn caster => TargetThingA as Pawn;

        private TargetIndex indTarget => TargetIndex.B;
        private LocalTargetInfo _target => TargetB;
        private Pawn victim => TargetThingB as Pawn;

        private CompAbilityEffect_PsychicChoke comp;

        public override string GetReport()
        {
            string replaceWith;
            if (victim != null && victim.LabelShortCap != null)
            {
                replaceWith = victim.LabelShortCap;
            }
            else
            {
                replaceWith = "something";
            }

            if (this.job != null)
            {
                string text = this.job.def.reportString.Replace("TargetB", replaceWith);
                return text;
            }
            else
            {
                return "Job error";
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            comp = caster.abilities.abilities
                .SelectMany(a => a.comps.Where(c => c is CompAbilityEffect_PsychicChoke))
                .Cast<CompAbilityEffect_PsychicChoke>()
                .FirstOrDefault(c => c.Props.sustainedJobDef == this.job.def);
            return comp != null;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(indCaster);
            this.FailOnDespawnedOrNull(indTarget);
            this.FailOnDownedOrDead(indCaster);
            this.FailOnMentalState(indCaster);
            this.FailOn(() => comp == null);
            this.FailOn(() => !comp.ShouldBeChoking);
            this.FailOn(() => !comp.ShouldContinueChoking());
            


            yield return new Toil()
            {
                actor = caster,
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = comp.Props.maxSustainTicks,
                tickAction = (() => comp.SustainedTick()),
            };
              
        }

        public void OnEnd()
        {
            if (comp != null && comp?.ShouldBeChoking != null)
            {
                comp.ShouldBeChoking = false;
            }

            if (victim != null && victim?.health != null && victim?.health?.hediffSet != null && !victim.health.Dead)
            {
                HediffDef psyChokeHediff = CombatPsycasts.DefOfs.HediffDefOf.CP_Hediff_PsychicChoke;
                HediffComp_ReducesOverTime compReduce = victim.health.hediffSet
                    .GetFirstHediffOfDef(psyChokeHediff)?.TryGetComp<HediffComp_ReducesOverTime>();
                if (compReduce != null)
                {
                    compReduce.ShouldReduce = true;
                }
            }
        }
    }
}