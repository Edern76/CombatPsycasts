using System.Collections.Generic;
using System.Linq;
using CombatPsycasts.Comps;
using CombatPsycasts.Interfaces;
using Verse;
using Verse.AI;

namespace CombatPsycasts.Jobs
{
    public class JobDriver_SustainPsychicShoot : JobDriver, IJobWithEndAction
    {
        private TargetIndex indCaster => TargetIndex.A;
        private Pawn caster => TargetThingA as Pawn;

        private TargetIndex indTarget => TargetIndex.B;
        private LocalTargetInfo _target => TargetB;

        private CompAbilityEffect_PsychicSustainedShoot comp;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            comp = caster.abilities.abilities
                .SelectMany(a => a.comps.Where(c => c is CompAbilityEffect_PsychicSustainedShoot))
                .Cast<CompAbilityEffect_PsychicSustainedShoot>()
                .FirstOrDefault(c => c.SustainedProps.sustainedJobDef == this.job.def);
            return comp != null;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(indCaster);
            this.FailOnDownedOrDead(indCaster);
            this.FailOnMentalState(indCaster);
            this.FailOn(() => comp == null);
            this.FailOn(() => !comp.ShouldBeFiring);
            this.FailOn(() => !comp.ShouldContinueFiring());


            yield return new Toil()
            {
                actor = caster,
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = comp.SustainedProps.maxSustainTicks,
                tickAction = (() => comp.SustainedTick()),
            };
              
        }

        public void OnEnd()
        {
            comp.ShouldBeFiring = false;
        }
    }
}