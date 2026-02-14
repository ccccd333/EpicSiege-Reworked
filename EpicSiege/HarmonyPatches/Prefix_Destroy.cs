//using System;
//using System.Collections.Generic;
//using System.Linq;
//using EpicSiege;
//using HarmonyLib;
//using RimWorld;
//using RimWorld.Planet;
//using Verse;

//namespace HarmonyPatches
//{
//    // Token: 0x02000005 RID: 5
//    [HarmonyPatch(typeof(Site), "Destroy")]
//    internal class Prefix_Destroy
//    {
//        // Token: 0x06000009 RID: 9 RVA: 0x00002314 File Offset: 0x00000514
//        private static void Prefix(ref Site __instance)
//        {
//            Site site = __instance;
//            string text;
//            if (site == null)
//            {
//                text = null;
//            }
//            else
//            {
//                List<SitePart> parts = site.parts;
//                if (parts == null)
//                {
//                    text = null;
//                }
//                else
//                {
//                    SitePart sitePart = parts.FirstOrDefault<SitePart>();
//                    if (sitePart == null)
//                    {
//                        text = null;
//                    }
//                    else
//                    {
//                        SitePartDef def = sitePart.def;
//                        if (def == null)
//                        {
//                            text = null;
//                        }
//                        else
//                        {
//                            List<string> tags = def.tags;
//                            text = ((tags != null) ? tags.FirstOrDefault<string>() : null);
//                        }
//                    }
//                }
//            }
//            if (text != "SiegeCamp")
//            {
//                return;
//            }
//            if (__instance.WorldObjectTimeoutTicksLeft >= 1)
//            {
//                return;
//            }
//            TimeoutComp component = __instance.GetComponent<TimeoutComp>();
//            if (component != null && component.TicksLeft > 0)
//            {
//                return;
//            }
//            Map anyPlayerHomeMap = Find.AnyPlayerHomeMap;
//            if (anyPlayerHomeMap == null)
//            {
//                return;
//            }
//            IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, anyPlayerHomeMap);
//            incidentParms.forced = true;
//            incidentParms.faction = __instance.Faction;
//            incidentParms.target = anyPlayerHomeMap;
//            incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
//            float num = incidentParms.points * (float)EpicSiegeMod.settings.additionalMultiplier;
//            incidentParms.points = num;
//            int num2 = 21600;
//            SitePart sitePart2 = __instance.parts.FirstOrDefault<SitePart>();
//            SitePartDef sitePartDef = ((sitePart2 != null) ? sitePart2.def : null);
//            if (sitePartDef == ESDefOf.ESMortarCamp)
//            {
//                incidentParms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("Siege", true);
//                num2 = 1000;
//                Log.Message("Prefix_Destroy: Triggering Siege Raid.");
//                Log.Message(string.Format("Raid strategy: {0}", incidentParms.raidStrategy));
//            }
//            else if (sitePartDef == ESDefOf.ESBreachCamp)
//            {
//                incidentParms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("ImmediateAttackBreaching", true);
//                num2 = 10800;
//                Log.Message("Prefix_Destroy: Triggering Breach Raid.");
//                Log.Message(string.Format("Raid strategy: {0}", incidentParms.raidStrategy));
//            }
//            else
//            {
//                incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
//                Log.Message("Prefix_Destroy: Triggering Standard Raid.");
//                Log.Message(string.Format("Raid strategy: {0}", incidentParms.raidStrategy));
//            }
//            Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + num2, incidentParms, 0);
//        }
//    }
//}
