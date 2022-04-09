using CombatPsycasts.Comps;
using CombatPsycasts.Utils;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CombatPsycasts.Harmony.Patches
{
    [HarmonyPatch(typeof(Faction))]
    [HarmonyPatch("Notify_MemberDied")]
    public class PatchFactionMemberDied
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn member,
            DamageInfo? dinfo,
            bool wasWorldPawn,
            bool wasGuilty,
            Map map,
            Faction __instance)
        {
            if (dinfo == null && member.FindLethalInstigatorHediff() is Hediff hediff)
            {
                if (!wasWorldPawn && !__instance.IsPlayer && !PawnGenerator.IsBeingGenerated(member) &&
                    Current.ProgramState == ProgramState.Playing && map != null && !member.IsSlaveOfColony &&
                    map.IsPlayerHome && !__instance.HostileTo(Faction.OfPlayer))
                {
                    Pawn instigator = hediff.TryGetComp<HediffComp_HasInstigator>().instigator;
                    if (instigator != null && instigator.Faction != null && instigator.Faction.IsPlayer && !wasGuilty)
                    {
                        int goodwillChange;
                        if (member.kindDef.factionHostileOnKill)
                        {
                            goodwillChange = Faction.OfPlayer.GoodwillToMakeHostile(__instance);
                        }
                        else
                        {
                            goodwillChange = member.RaceProps.Humanlike ? -25 : -15;
                        }
                        Faction.OfPlayer.TryAffectGoodwillWith(__instance, goodwillChange, canSendHostilityLetter: (!__instance.temporary), reason: HistoryEventDefOf.MemberKilled);
                    }
                }    
            }
            else
            {
                Log.Message($"DInfo : {dinfo}");
            }
        }
    }
}