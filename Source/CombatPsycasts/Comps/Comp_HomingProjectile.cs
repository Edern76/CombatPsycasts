using Verse;

namespace CombatPsycasts.Comps
{
    public class CompProperties_HomingProjectile : CompProperties
    {
        public int homingRecalculationInterval = 1;

        public CompProperties_HomingProjectile()
        {
            this.compClass = typeof(Comp_HomingProjectile);
        }    
    }    
    
    public class Comp_HomingProjectile : ThingComp
    {
        public CompProperties_HomingProjectile Props => (CompProperties_HomingProjectile)this.props;

        public bool isHoming = false;
        public int ticksSinceLastRecalculation = 0;
        public IntVec3 lastCell;

        public bool ShouldRecalculateNow()
        {
            this.ticksSinceLastRecalculation++;
            if (this.ticksSinceLastRecalculation >= this.Props.homingRecalculationInterval)
            {
                this.ticksSinceLastRecalculation = 0;
                return true;
            }

            return false;
        }

        public void SetHoming(bool value)
        {
            isHoming = value;
        }

        public void SetLastCell(IntVec3 value)
        {
            lastCell = value;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.isHoming, "isHoming", false);
            Scribe_Values.Look(ref this.ticksSinceLastRecalculation, "ticksSinceLastRecalculation", 0);
        }
    }
}