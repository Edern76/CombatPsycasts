using System.Linq;
using CombatPsycasts.Comps;
using CombatPsycasts.Utils;
using RimWorld;
using RimWorld.Planet;
using VanillaPsycastsExpanded;
using Verse;
using HediffDefOf = CombatPsycasts.DefOfs.HediffDefOf;

namespace VFECP
{
    public class Ability_PsychicChoke : Ability_Continuous
    {
        private bool ThingIsStillAlive() =>
            curTarget.HasThing && !curTarget.ThingDestroyed && curTarget.Pawn is Pawn pawn && pawn.health.State != PawnHealthState.Dead;

        public bool ShouldContinueChoking() => pawn.drafter.Drafted && ThingIsStillAlive();

        public override bool ShouldContinueCasting() => base.ShouldContinueCasting() && ShouldContinueChoking();

        public override bool CanHitTarget(LocalTargetInfo target) => base.CanHitTarget(target) && target.Pawn is Pawn pawn &&
                                                                     pawn.health.hediffSet.GetNotMissingParts().Any(p => p.def == BodyPartDefOf.Neck);

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            if (ThingIsStillAlive())
            {
                if (curTarget.Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.CP_Hediff_PsychicChoke) is
                    Hediff hediff)
                    hediff.TryGetComp<HediffComp_ReducesOverTime>().ShouldReduce = false;

                HediffUtils.AddOrUpdateHediffWithSeverity(curTarget.Pawn, HediffDefOf.CP_Hediff_PsychicChoke,
                    curTarget.Pawn.def.race.body.GetPartsWithDef(BodyPartDefOf.Neck).FirstOrDefault(), 0.05f);
            }
        }

        public override void CastingTick()
        {
            base.CastingTick();
            if (ShouldContinueChoking() && pawn.IsHashIntervalTick(60))
            {
                currentlyCasting = pawn.psychicEntropy.TryAddEntropy(def.Psycast().entropyGain);

                if (currentlyCasting)
                {
                    HediffUtils.AddOrUpdateHediffWithSeverity(curTarget.Pawn, HediffDefOf.CP_Hediff_PsychicChoke,
                        curTarget.Pawn.def.race.body.GetPartsWithDef(BodyPartDefOf.Neck).FirstOrDefault(), 0.10f);
                    curTarget.Pawn?.FindLethalInstigatorHediff()?.TryGetComp<HediffComp_HasInstigator>()?.SetInstigator(pawn);
                }
            }
        }

        public override void EndCasting()
        {
            base.EndCasting();

            if (curTarget.Pawn?.health?.hediffSet != null && !curTarget.Pawn.health.Dead)
            {
                var compReduce = curTarget.Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.CP_Hediff_PsychicChoke)
                    ?.TryGetComp<HediffComp_ReducesOverTime>();
                if (compReduce != null) compReduce.ShouldReduce = true;
            }
        }
    }
}