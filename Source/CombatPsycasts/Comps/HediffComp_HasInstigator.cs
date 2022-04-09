using Verse;

namespace CombatPsycasts.Comps
{
    public class HediffCompProperties_HasInstigator : HediffCompProperties
    {
        public bool isLethal = true;
        
        public HediffCompProperties_HasInstigator()
        {
            this.compClass = typeof(HediffComp_HasInstigator);
        }
    }    
    
    public class HediffComp_HasInstigator : HediffComp
    {
        public HediffCompProperties_HasInstigator Props => (HediffCompProperties_HasInstigator)this.props;

        public Pawn instigator;

        public void SetInstigator(Pawn instigator)
        {
            this.instigator = instigator;
        }
        
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_References.Look(ref this.instigator, "instigator", true);
        }
    }
}