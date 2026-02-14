//using System;
//using System.Collections.Generic;
//using System.Linq;
//using EpicSiege;
//using HarmonyLib;
//using RimWorld;
//using RimWorld.Planet;
//using RimWorld.QuestGen;
//using Verse;

//namespace HarmonyPatches
//{
//    [HarmonyPatch(typeof(Site), "Notify_MyMapAboutToBeRemoved")]
//    internal class Prefix_Notify_MyMapAboutToBeRemoved
//    {
//        public static List<Site> pendingDestroy = new List<Site>();

//        public static int count = 0;

//        private static void Prefix(ref Site __instance)
//        {
//            SitePart sitePart = __instance.parts.FirstOrDefault<SitePart>();
//            if (__instance.HasMap && sitePart != null)
//            {
//                SitePartDef def = sitePart.def;
//                string text;
//                if (def == null)
//                {
//                    text = null;
//                }
//                else
//                {
//                    List<string> tags = def.tags;
//                    text = ((tags != null) ? tags.FirstOrDefault<string>() : null);
//                }
//                if (!(text != "SiegeCamp"))
//                {
//                    Log.Message($"--------Prefix_Notify_MyMapAboutToBeRemoved.Prefix---------{count}");
//                    count++;
//                    if (GenHostility.AnyHostileActiveThreatToPlayer(__instance.Map, true, false))
//                    {
//                        Prefix_Notify_MyMapAboutToBeRemoved.GenerateRaid(__instance);
//                        return;
//                    }
//                    Faction enemy = __instance.Faction;
//                    List<Site> list = Find.WorldObjects.Sites.FindAll(delegate (Site x)
//                    {
//                        SitePart sitePart2 = x.parts.FirstOrDefault<SitePart>();
//                        return ((sitePart2 != null) ? sitePart2.def.tags.FirstOrDefault<string>() : null) == "SiegeCamp" && x.Faction == enemy;
//                    });
//                    if (list.Count > 1)
//                    {
//                        Log.Message($"AAAA");
//                        if (list.Count > 2)
//                        {
//                            Log.Message($"AAAA1");
//                            Letter letter = LetterMaker.MakeLetter("CampDestroyedName".Translate(), "CampDestroyedDescription".Translate(), LetterDefOf.PositiveEvent, null, null);
//                            Find.LetterStack.ReceiveLetter(letter, null, 0, true);
//                            return;
//                        }
//                        else
//                        {
//                            Log.Message($"AAAA2");
//                            Site site = list.FirstOrDefault<Site>();
//                            if (site != null)
//                            {
//                                pendingDestroy.Add(site);

//                            }
//                        }
//                        //if (list.Count > 1)
//                        //{
//                        //    Log.Message($"AAAA3");
//                        //    return;
//                        //}
//                    }
//                    Log.Message($"AAAA4");
//                    Site site2 = list.Where(s => !pendingDestroy.Contains(s)).FirstOrDefault();
//                    if (site2 != null)
//                    {
//                        Log.Message($"AAAA5");
//                        pendingDestroy.Add(site2);
                            
//                    }
//                    Log.Message($"AAAA6");
//                    Letter letter2 = LetterMaker.MakeLetter("RetreatName".Translate(), __instance.Faction.NameColored + "RetreatText".Translate(), LetterDefOf.PositiveEvent, null, null);
//                    Find.LetterStack.ReceiveLetter(letter2, null, 0, true);
//                    if (!enemy.def.permanentEnemy)
//                    {
//                        Slate slate = new Slate();
//                        slate.Set<Faction>("faction", enemy, false);
//                        Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(DefDatabase<QuestScriptDef>.GetNamed("OpportunitySite_PeaceTalks", true), slate);
//                        Letter letter3 = LetterMaker.MakeLetter("OutcomeName".Translate(), __instance.Faction.NameColored + "OutcomeText".Translate(), LetterDefOf.PositiveEvent, __instance.Faction, quest);
//                        Find.LetterStack.ReceiveLetter(letter3, null, 0, true);
//                        return;
//                    }
//                    if (enemy.leader != null && enemy.leader.Map == null)
//                    {
//                        TaggedString taggedString = "executeLeaderLabel".Translate();
//                        Name name = enemy.leader.Name;
//                        Letter letter4 = LetterMaker.MakeLetter(taggedString, ((name != null) ? name.ToString() : null) + "executeLeaderText1".Translate() + enemy.NameColored + "executeLeaderText2".Translate(), LetterDefOf.PositiveEvent, enemy, null);
//                        Find.LetterStack.ReceiveLetter(letter4, null, 0, true);
//                        foreach (Pawn pawn2 in Find.WorldPawns.AllPawnsAlive.FindAll((Pawn pawn) => pawn.IsColonist))
//                        {
//                            Pawn_NeedsTracker needs = pawn2.needs;
//                            if (needs != null)
//                            {
//                                Need_Mood mood = needs.mood;
//                                if (mood != null)
//                                {
//                                    MemoryThoughtHandler memories = mood.thoughts.memories;
//                                    if (memories != null)
//                                    {
//                                        memories.TryGainMemory(ThoughtDefOf.DefeatedHostileFactionLeader, enemy.leader, null);
//                                    }
//                                }
//                            }
//                        }
//                        LongEventHandler.ExecuteWhenFinished(() =>
//                        { enemy.leader.Kill(null, null); });
                            
//                        return;
//                    }
//                    return;
//                }
//            }
//        }

//        //private static void Postfix(ref Site __instance)
//        //{
//        //    // MapDeiniter.NotifyEverythingWhichUsesMapReference



//        //    // site削除時、ArgumentOutOfRangeが出てた。
//        //    // メインスレッド外からメインスレッドを触ろうとすると怒られるので
//        //    // その際にそこにアクションを渡すとメインスレッドへのinvokeキューが空いた
//        //    // タイミングで遅延実行してくれる
//        //    if (pendingDestroy.Count > 0)
//        //    {
//        //        var toDestroy = pendingDestroy.ToList();
//        //        pendingDestroy.Clear();

//        //        LongEventHandler.ExecuteWhenFinished(() =>
//        //        {
//        //            foreach (var s in toDestroy)
//        //            {
//        //                if (s != null && !s.Destroyed)
//        //                    s.Destroy();
//        //            }
//        //        });
//        //    }
//        //}
//        public static void GenerateRaid(Site __instance)
//        {
//            Log.Message("GenerateRaid");
//            IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, Find.AnyPlayerHomeMap);
//            incidentParms.forced = true;
//            incidentParms.faction = __instance.Faction;
//            incidentParms.target = Find.AnyPlayerHomeMap;
//            incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
//            float num = incidentParms.points * (float)EpicSiegeMod.settings.additionalMultiplier;
//            incidentParms.points = num;
//            int num2 = 0;
//            SitePart sitePart = __instance.parts.FirstOrDefault<SitePart>();
//            if (sitePart != null)
//            {
//                if (sitePart.def == ESDefOf.ESMortarCamp)
//                {
//                    incidentParms.raidStrategy = ESDefOf.Siege;
//                    num2 = 1000;
//                }
//                else if (sitePart.def == ESDefOf.ESBreachCamp)
//                {
//                    incidentParms.raidStrategy = ESDefOf.ImmediateAttackBreaching;
//                    num2 = 10800;
//                }
//                else if (sitePart.def == ESDefOf.ESRaidCamp)
//                {
//                    incidentParms.raidStrategy = ESDefOf.ImmediateAttack;
//                    num2 = 21600;
//                }
//            }
//            Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + num2, incidentParms, 0);
//        }
//    }
//}
