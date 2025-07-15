using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;
using VEF.Abilities;

namespace VFECP
{
    public class Ability_Continuous : Ability
    {
        protected int castingTime;
        protected bool currentlyCasting;
        protected LocalTargetInfo curTarget;
        protected List<GlobalTargetInfo> curTargets;
        protected AbilityExtension_Continuous Ext => def.GetModExtension<AbilityExtension_Continuous>();

        public virtual void CastingTick()
        {
            castingTime++;
        }

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            curTargets = targets.ToList();
            curTarget = targets[0].HasThing ? new LocalTargetInfo(targets[0].Thing) : new LocalTargetInfo(targets[0].Cell);
            currentlyCasting = true;
            castingTime = 0;
        }

        public virtual bool ShouldContinueCasting() => currentlyCasting && curTargets != null && curTarget.IsValid && castingTime <= Ext.maxSustainTicks;

        public virtual void EndCasting()
        {
            curTargets = null;
            currentlyCasting = false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref castingTime, nameof(castingTime));
            Scribe_Values.Look(ref currentlyCasting, nameof(currentlyCasting));
            Scribe_Collections.Look(ref curTargets, nameof(curTargets), LookMode.GlobalTargetInfo);
            Scribe_TargetInfo.Look(ref curTarget, nameof(curTarget));
        }
    }

    public class AbilityExtension_Continuous : AbilityExtension_AbilityMod
    {
        public int maxSustainTicks = 600;
    }
}