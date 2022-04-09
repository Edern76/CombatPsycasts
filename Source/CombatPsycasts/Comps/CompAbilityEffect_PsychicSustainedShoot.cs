using CombatPsycasts.Verbs;
using Verse;
using Verse.AI;

namespace CombatPsycasts.Comps
{
    public class CompProperties_PsychicSustainedShoot : CompProperties_AbilityPsychicShoot
    {
        public JobDef sustainedJobDef;
        public int maxSustainTicks = 600;
        
        public CompProperties_PsychicSustainedShoot()
        {
            this.compClass = typeof(CompAbilityEffect_PsychicSustainedShoot);
        }    
    }    
    
    public class CompAbilityEffect_PsychicSustainedShoot : CompAbilityEffect_PsychicShoot
    {
        public override int Id => 9212392;
        public int TicksSinceLastSecond = 0;
        public bool ShouldBeFiring = false;
        public LocalTargetInfo curTarget;

        
        private bool shootCanReach = true;
        private bool forceFirstShot = false;


        public CompProperties_PsychicSustainedShoot SustainedProps => (CompProperties_PsychicSustainedShoot)this.props;

        private bool ThingIsStillStanding()
        {
            if (!this.curTarget.HasThing)
            {
                return true;
            }
            else
            {
                if (this.curTarget.ThingDestroyed)
                {
                    return false;
                }

                if (this.curTarget.Pawn is Pawn pawn &&
                    (pawn.health.State == PawnHealthState.Down || pawn.health.State == PawnHealthState.Dead))
                {
                    return false;
                }

                return true;
            }
        }
        public bool ShouldContinueFiring() => shootCanReach && this.parent.CanCast && this.parent.pawn.drafter.Drafted && ThingIsStillStanding();

        
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.BaseApply(target, dest);
            if (!ShouldBeFiring)
            {
                this.ShouldBeFiring = true;
                this.forceFirstShot = true;
                curTarget = target;
                Job toStart = JobMaker.MakeJob(SustainedProps.sustainedJobDef, this.parent.pawn, curTarget);
                this.parent.pawn.jobs.TryTakeOrderedJob(toStart);
            }
        }

        public void SustainedTick()
        {
            this.TicksSinceLastSecond++;
            if (ShouldBeFiring && (forceFirstShot || ( ShouldContinueFiring() && this.TicksSinceLastSecond.TicksToSeconds() > this.parent.verb.verbProps.warmupTime)))
            {
                if (VerbTracker.PrimaryVerb is Verb_PsychicShoot verbShoot)
                {
                    this.TicksSinceLastSecond = 0;
                    if (!this.forceFirstShot)
                    {
                        ShouldBeFiring = this.parent.pawn.psychicEntropy.TryAddEntropy(this.parent.def.EntropyGain);
                    }

                    if (ShouldBeFiring)
                    {
                        this.shootCanReach = verbShoot.TryStartCastOn(curTarget);
                    }

                    this.forceFirstShot = false;
                }
                else
                {
                    Verb verb = VerbTracker.PrimaryVerb;
                    Log.Error($"Primary verb is not shoot (verb : {verb} | type : {verb.GetType()})");
                }
            }    
        }
    }
}