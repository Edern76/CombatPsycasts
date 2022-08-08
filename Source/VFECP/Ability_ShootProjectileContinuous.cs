using System.Collections.Generic;
using CombatPsycasts.Verbs;
using RimWorld;
using RimWorld.Planet;
using VanillaPsycastsExpanded;
using Verse;

namespace VFECP
{
    public class Ability_ShootProjectileContinuous : Ability_Continuous, IVerbOwner
    {
        private bool forceNextShot;

        private VerbTracker verbTracker;
        private int warmupTicks;

        public string UniqueVerbOwnerID() => GetUniqueLoadID();

        public bool VerbsStillUsableBy(Pawn p) => pawn == p;

        public VerbTracker VerbTracker => verbTracker;
        public List<VerbProperties> VerbProperties => def.GetModExtension<AbilityExtension_Verbs>().verbs;
        public List<Tool> Tools => new List<Tool>();
        public ImplementOwnerTypeDef ImplementOwnerTypeDef => ImplementOwnerTypeDefOf.Weapon;
        public Thing ConstantCaster => pawn;

        public override void Init()
        {
            base.Init();
            verbTracker = new VerbTracker(this);
        }

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            forceNextShot = true;
        }

        private bool ThingIsStillStanding()
        {
            if (!curTarget.HasThing) return true;

            if (curTarget.ThingDestroyed) return false;

            if (curTarget.Pawn is Pawn pawn && (pawn.health.State == PawnHealthState.Down || pawn.health.State == PawnHealthState.Dead)) return false;

            return true;
        }

        public bool ShouldContinueFiring() => pawn.drafter.Drafted && ThingIsStillStanding();

        public override bool ShouldContinueCasting() => base.ShouldContinueCasting() && ShouldContinueFiring();

        public override void CastingTick()
        {
            base.CastingTick();
            warmupTicks++;
            if (forceNextShot || (ShouldContinueFiring() && warmupTicks > def.castTime))
            {
                if (VerbTracker.PrimaryVerb is Verb_PsychicShoot verbShoot)
                {
                    warmupTicks = 0;

                    if (!forceNextShot) currentlyCasting = pawn.psychicEntropy.TryAddEntropy(def.Psycast().entropyGain);

                    if (currentlyCasting) currentlyCasting = verbShoot.TryStartCastOn(curTarget);
                    forceNextShot = false;
                }
                else
                {
                    var verb = VerbTracker.PrimaryVerb;
                    Log.Error($"Primary verb is not shoot (verb : {verb} | type : {verb.GetType()})");
                }
            }
        }

        public override void Tick()
        {
            base.Tick();
            verbTracker.VerbsTick();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref verbTracker, nameof(verbTracker), this);
            Scribe_Values.Look(ref forceNextShot, nameof(forceNextShot));
            Scribe_Values.Look(ref warmupTicks, nameof(warmupTicks));
        }
    }

    public class AbilityExtension_Verbs : AbilityExtension_Continuous
    {
        public List<VerbProperties> verbs;
    }
}