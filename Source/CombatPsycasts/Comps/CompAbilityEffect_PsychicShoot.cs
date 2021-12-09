using System.Collections.Generic;
using CombatPsycasts.Interfaces;
using CombatPsycasts.Verbs;
using RimWorld;
using Verse;

namespace CombatPsycasts.Comps
{
    public class CompProperties_AbilityPsychicShoot : CompProperties_AbilityEffect
    {
        public List<VerbProperties> verbs;

        public CompProperties_AbilityPsychicShoot()
        {
            this.compClass = typeof(CompAbilityEffect_PsychicShoot);
        }
    }

    public class CompAbilityEffect_PsychicShoot : CompAbilityBase_CombatPsychic, IVerbOwner, IExposable, ILoadReferenceable,
        ITickable
    {
        public virtual int Id => 4271342;

        public CompProperties_AbilityPsychicShoot Props => (CompProperties_AbilityPsychicShoot)this.props;
        private VerbTracker _verbTracker;
        
        public Thing ConstantCaster => this.parent.pawn;
        public string UniqueVerbOwnerID() => this.GetUniqueLoadID();
        public bool VerbsStillUsableBy(Pawn p) => true;
        public List<Tool> Tools { get; private set; }
        public ImplementOwnerTypeDef ImplementOwnerTypeDef => ImplementOwnerTypeDefOf.NativeVerb;
        public List<VerbProperties> VerbProperties => Props.verbs;

        public VerbTracker VerbTracker
        {
            get
            {
                if (this._verbTracker == null)
                {
                    this._verbTracker = new VerbTracker((IVerbOwner)this);
                }

                return this._verbTracker;
            }
        }


        public string GetUniqueLoadID() => "CP_AbilityComp_" + (object) this.Id;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            BaseApply(target, dest);
            if (VerbTracker.PrimaryVerb is Verb_PsychicShoot verbShoot)
            {
                bool shot = verbShoot.TryStartCastOn(target);
            }
            else
            {
                Verb verb = VerbTracker.PrimaryVerb;
                Log.Error($"Primary verb is not shoot (verb : {verb} | type : {verb.GetType()})");
            }
        }

        protected void BaseApply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
        }

        public virtual void DoTick()
        {
            this.VerbTracker.VerbsTick();
        }


        public void ExposeData()
        {
            Scribe_Deep.Look<VerbTracker>(ref this._verbTracker, "verbTracker", (object)this);
        }
    }
    

}