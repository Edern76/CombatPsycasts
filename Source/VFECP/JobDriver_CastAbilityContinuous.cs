using System.Collections.Generic;
using Verse.AI;
using VFECore.Abilities;

namespace VFECP
{
    public class JobDriver_CastAbilityContinuous : JobDriver_CastAbilityOnce
    {
        public Ability_Continuous Ability => CompAbilities.currentlyCasting as Ability_Continuous;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            foreach (var toil in base.MakeNewToils()) yield return toil;
            var castToil = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Never,
                tickAction = () =>
                {
                    Ability.CastingTick();
                    pawn.rotationTracker.FaceTarget(job.GetTarget(TargetIndex.A));
                },
                handlingFacing = true
            };
            castToil.AddFailCondition(() => Ability is null);
            castToil.AddEndCondition(() => Ability.ShouldContinueCasting() ? JobCondition.Ongoing : JobCondition.Succeeded);
            castToil.AddFinishAction(() => Ability.EndCasting());
            yield return castToil;
        }
    }
}