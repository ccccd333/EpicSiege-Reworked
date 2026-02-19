using EpicSiege;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using static Verse.HediffCompProperties_RandomizeSeverityPhases;

namespace SiegeIncident
{
    internal class IncidentWorker_SetUpSiegeCamp : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!IncidentWorker_SetUpSiegeCamp.IsSuitableEnemy(parms.faction, out this.enemy) && 
                !IncidentWorker_SetUpSiegeCamp.EnemyFaction(out this.enemy))
            {
                Log.Warning("IncidentWorker_SetUpSiegeCamp: No suitable enemy faction found.");
                return false;
            }

            //Log.Message($"forced={parms.forced}, Target={parms.target}, StoryState.Target={parms.target.StoryState.Target}, points={parms.points}");

            List<Site> list = new List<Site>();
            for (int i = 0; i < 3; i++)
            {
                PlanetTile planetTile;
                if (!TileFinder.TryFindNewSiteTile(out planetTile, 3, 5, false, null, 0.5f, true, TileFinderMode.Random, false, true, null, null))
                {
                    Log.Warning(string.Format("IncidentWorker_SetUpSiegeCamp: Failed to find a valid tile for siege camp {0}.", i));
                }
                else
                {
                    // ここ元MODからforced=trueで記述していた。
                    // このタイミングのforcedはストーリーテラー側でインシデントが発生したことも記録しない
                    // これはインシデントそのものの発生条件としてはいい。
                    // ただ、インシデントの現状を考慮した発生条件としては不適切。
                    // そのためincidentDefの記述のMinRefireDays(インシデントが発生して30日経過したか)などを無視する。
                    // ここに書くべきではない
                    //parms.forced = true;


                    Site site = IncidentWorker_SetUpSiegeCamp.CreateSiegeCamp(i, planetTile, this.enemy, parms.points);
                    if (site == null)
                    {
                        Log.Warning(string.Format("IncidentWorker_SetUpSiegeCamp: Failed to create siege camp {0}.", i));
                    }
                    else
                    {
                        list.Add(site);
                        Find.WorldObjects.Add(site);
                    }
                }
            }
            if (list.Count == 0)
            {
                Log.Warning("IncidentWorker_SetUpSiegeCamp: No siege camps could be placed.");
                return false;
            }
            TaggedString taggedString = "EpicSiegeDescription1".Translate() + " " + this.enemy.NameColored + " " + "EpicSiegeDescription2".Translate(EpicSiegeMod.settings.alterDaysBeforeRaid.ToString("F1")) + "\n" + "EpicSiegeDescription3".Translate();
            Letter letter = LetterMaker.MakeLetter("EpicSiegeName".Translate(), taggedString, LetterDefOf.ThreatBig, this.enemy, null);
            letter.lookTargets = list.FirstOrDefault<Site>();
            Find.LetterStack.ReceiveLetter(letter, null, 0, true);
            //EpicSiegeDebugUtility.DebugLogCallStack("IncidentWorker_SetUpSiegeCamp: Successfully set up siege camps. Call stack:");
            //Log.Message($"IncidentWorker_SetUpSiegeCamp: Successfully set up {list.Count} siege camps for faction {this.enemy.Name}.");
            return true;
        }

        private static Site CreateSiegeCamp(int index, PlanetTile tile, Faction faction, float points)
        {
            SitePartDef sitePartDef = null;

            if (EpicSiegeMod.settings.customRaidPhases)
            {
                sitePartDef = GetCustomRaidSitePartDef(index);
            }
            else
            {
                sitePartDef = GetDefaultRaidSitePartDef(index);
            }

            if (sitePartDef == null)
            {
                Log.Error("IncidentWorker_SetUpSiegeCamp: Invalid site part definition for index " + index);
                return null;
            }

            Site site = SiteMaker.MakeSite(sitePartDef, tile, faction, true, new float?(points), ESDefOf.EpicSiegeCampSite);
            site.sitePartsKnown = true;
            int num = Mathf.RoundToInt(60000f * EpicSiegeMod.settings.alterDaysBeforeRaid);
            if (num > 0)
            {
                TimeoutComp component = site.GetComponent<TimeoutComp>();
                if (component != null)
                {
                    component.StartTimeout(num);
                }
            }
            else
            {
                Log.Warning("IncidentWorker_SetUpSiegeCamp: Invalid timeout duration. Using default.");
                TimeoutComp component2 = site.GetComponent<TimeoutComp>();
                if (component2 != null)
                {
                    component2.StartTimeout(120000);
                }
            }
            
            return site;
        }

        private static bool EnemyFaction(out Faction enemy)
        {
            return Find.FactionManager.AllFactions.Where((Faction x) => !x.def.hidden && !x.defeated && x.HostileTo(Faction.OfPlayer) && x.def.canSiege).TryRandomElement(out enemy);
        }

        private static bool IsSuitableEnemy(Faction faction, out Faction enemy)
        {
            enemy = faction;
            return faction != null && !faction.def.hidden && !faction.defeated && faction.HostileTo(Faction.OfPlayer) && faction.def.canSiege;
        }

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && Find.AnyPlayerHomeMap != null;
        }

        private static SitePartDef GetCustomRaidSitePartDef(int phase) 
        {
            switch (phase)
            {
                case 0:
                    return MapPhaseTypeToSitePartDef(EpicSiegeMod.settings.raidPhase1);
                case 1:
                    return MapPhaseTypeToSitePartDef(EpicSiegeMod.settings.raidPhase2);
                case 2:
                    return MapPhaseTypeToSitePartDef(EpicSiegeMod.settings.raidPhase3);
                default:
                    return null;
            }
        }

        private static SitePartDef GetDefaultRaidSitePartDef(int phase)
        {
            switch (phase)
            {
                case 0:
                    return ESDefOf.ESMortarCamp;
                case 1:
                    return ESDefOf.ESBreachCamp;
                case 2:
                    return ESDefOf.ESRaidCamp;
                default:
                    return null;
            }
        }

        private static SitePartDef MapPhaseTypeToSitePartDef(int phaseType)
        {
            switch (phaseType)
            {
                case 0:
                    return ESDefOf.ESRaidCamp;
                case 1:
                    return ESDefOf.ESBreachCamp;
                case 2:
                    return ESDefOf.ESMortarCamp;
                default:
                    return null;
            }
        }

        private const int MaxDist = 5;

        private const int MinDist = 3;

        private Faction enemy;
    }

}
