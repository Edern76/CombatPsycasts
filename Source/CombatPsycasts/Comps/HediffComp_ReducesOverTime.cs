using Verse;

namespace CombatPsycasts.Comps
{
    public class HediffCompProperties_ReducesOverTime : HediffCompProperties
    {
        public int ticksToReduction = 6;
        
        public HediffCompProperties_ReducesOverTime()
        {
            this.compClass = typeof(HediffComp_ReducesOverTime);
        }
    }
    
    public class HediffComp_ReducesOverTime : HediffComp
    {
        public HediffCompProperties_ReducesOverTime Props => (HediffCompProperties_ReducesOverTime)this.props;
        public bool ShouldReduce = false;

        private int ticksSinceLastReduce = 0;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            ticksSinceLastReduce++;
            if (ShouldReduce)
            {
                if (ticksSinceLastReduce > Props.ticksToReduction)
                {
                    severityAdjustment -= 0.01f;
                    ticksSinceLastReduce = 0;
                }    
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref ShouldReduce, "shouldReduce", false);
            Scribe_Values.Look(ref ticksSinceLastReduce, "ticksSinceLastReduce", 0);
        }
    }
}