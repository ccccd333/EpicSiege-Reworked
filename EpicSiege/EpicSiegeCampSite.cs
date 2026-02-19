using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Verse;

namespace EpicSiege
{
    public class EpicSiegeCampSite : Site
    {
        public override void Notify_MyMapAboutToBeRemoved()
        {
            base.Notify_MyMapAboutToBeRemoved();

            // MapParent.CheckRemoveMapNow->this.ShouldRemoveMapNow(out flag)成立時
            // Current.Game.DeinitAndRemoveMap(map, true);が呼ばれてこのメソッドが呼ばれる。
            // ShouldRemoveMapNowの成立判定でSitePartDefのworkerClassにSitePartWorker_RaidSourceを指定してさえすれば、
            // 敵を全滅させたときtrueを返し、ここに到達する。
            // ただし、これはプレイヤーがそのまま敵を倒さずマップを離れたとき、
            // (①return !TransporterUtility.IncomingTransporterPreventingMapRemoval(base.Map);←このマップに来る、飛んでくる旅客機などが居るかの否定形(出ていく))
            // (②けど、(x.def.Worker is SitePartWorker_RaidSource && GenHostility.AnyHostileActiveThreatToPlayer(base.Map, true, false))←レイド中で敵対的な人まだいる)
            // (①②の時マップ消すけどSite(拠点)は残すとなる)
            // マップは初期化するけど、Site(拠点)は残すよとなる。
            //
            // 敵がいる状態でそのまま敵拠点から出ていきましたの場合もここに来る
            // そのため敵対的なポーンがいないか判定する必要がある

            // これならDestroy()のタイミングでもいいが、
            // 将来マップ内容を消す前に何かする場合を見てここに書いておく

            //Log.Message($"[EpicSiege] Notify_MyMapAboutToBeRemoved called for {this}. Checking for hostile threats.");
            if (!GenHostility.AnyHostileActiveThreatToPlayer(Map, true, false))
            {
                HandleCampCleared();
            }

        }

        public override void Destroy()
        {
            //Log.Message($"[EpicSiege] Destroying {this}. Checking for retaliation raid conditions.");
            try
            {
                TriggerRetaliationRaidIfFailed();
            }
            catch(Exception e)
            {
                Log.Error($"[EpicSiege]An error occurred after the timeout or when destroying the base. Please report it. wt?{e}");
            }
            SiegeSiteManager.RemoveSite(this);
            //SiegeSiteManager.Debug();

            base.Destroy();
        }

        private void HandleCampCleared()
        {
            if (!IsClearedOnTime() || !HasPlayerHome())
            {
                // プレイヤーポーンが時間切れで拠点破棄タイミングで、拠点へ入り
                // 時間切れで拠点破棄が起きた際、clearResultが0で敵ポーンの処刑か和平交渉が始まってしまう
                // そのため、タイム切れもしくはプレイヤー拠点が無くなった場合はここで処理を止める。
                return;
            }

            int clearResult;

            SiegeSiteManager.NotifySiteCleared(this, out clearResult);

            if (clearResult == 2)
            {
                EpicSiegeLetterUtility.SendCampDestroyedLetter();
                return;
            }
            else if (clearResult <= 1)
            {
                HandlePartialClear();
            }
        }

        private void HandlePartialClear()
        {
            Faction enemy = Faction;

            EpicSiegeLetterUtility.SendRetreatLetter(enemy);

            if (!enemy.def.permanentEnemy)
            {
                GeneratePeaceTalkQuest(enemy);
            }
            else if (enemy.leader != null && enemy.leader.Map == null)
            {
                HandleLeaderExecution(enemy);
            }

            // 元MODより3つ中2つを攻略した場合3つ目も消える処理
            SiegeSiteManager.DestroyAliveSigeSiteFaction(this);
        }

        private void GeneratePeaceTalkQuest(Faction enemy)
        {
            Slate slate = new Slate();
            slate.Set("faction", enemy);

            Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(
                DefDatabase<QuestScriptDef>.GetNamed("OpportunitySite_PeaceTalks", true),
                slate);

            EpicSiegeLetterUtility.SendPeaceOutcomeLetter(enemy, quest);
        }

        private void HandleLeaderExecution(Faction enemy)
        {
            EpicSiegeLetterUtility.SendLeaderExecutionLetter(enemy);

            foreach (Pawn colonist in Find.WorldPawns.AllPawnsAlive
                         .FindAll(p => p.IsColonist))
            {
                colonist.needs?.mood?.thoughts?.memories?
                    .TryGainMemory(ThoughtDefOf.DefeatedHostileFactionLeader, enemy.leader);
            }

            LongEventHandler.ExecuteWhenFinished(() =>
            {
                enemy.leader.Kill(null);
            });

        }

        private bool IsClearedOnTime()
        {
            TimeoutComp timeout = GetComponent<TimeoutComp>();
            return timeout != null && timeout.TicksLeft > 0;
        }

        private bool HasPlayerHome()
        {
            //Log.Message($"[EpicSiege] Checking for player home map. Found: {Find.AnyPlayerHomeMap != null}");
            return Find.AnyPlayerHomeMap != null;
        }

        private void TriggerRetaliationRaidIfFailed()
        {
            // 敵拠点を時間内にクリアした
            if (IsClearedOnTime())
            {
                //Log.Message($"[EpicSiege] Camp cleared on time. No retaliation raid will be triggered.");
                return;
            }

            Map playerHome = Find.AnyPlayerHomeMap;

            // 襲撃するプレイヤーホームがない
            if (!HasPlayerHome())
            {
                SiegeSiteManager.DestroyAliveSigeSiteFaction(this);
                return;
            }

            IncidentParms parms = StorytellerUtility.DefaultParmsNow(
                IncidentCategoryDefOf.ThreatBig,
                playerHome);

            parms.forced = true;
            parms.faction = Faction;
            parms.target = playerHome;
            parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            parms.points *= (float)EpicSiegeMod.settings.additionalMultiplier;

            ConfigureRaidStrategy(parms);

            Find.Storyteller.incidentQueue.Add(
                IncidentDefOf.RaidEnemy,
                Find.TickManager.TicksGame + GetRaidDelay(),
                parms);
        }

        private void ConfigureRaidStrategy(IncidentParms parms)
        {
            SitePartDef partDef = parts.FirstOrDefault()?.def;

            if (partDef == ESDefOf.ESMortarCamp)
            {
                parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("Siege");
            }
            else if (partDef == ESDefOf.ESBreachCamp)
            {
                parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("ImmediateAttackBreaching");
            }
            else
            {
                parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            }
        }

        private int GetRaidDelay()
        {
            if (EpicSiegeMod.settings.customRaidPhases)
            {
                const int MaxSites = 3;
                // remainingSites = 1～3 
                int remainingSites = SiegeSiteManager.CountRemainingSitesForFaction(this.Faction);

                // 3を超えたら1～3にランダム化
                // これは、現在Site(this)をキーとして、Factionが一緒かでカウントしている
                // ThreatBigのRaidは被ることはないので最大3でいいが、
                // 被った際にはランダム化しておく
                if (remainingSites > MaxSites)
                {
                    remainingSites = Rand.RangeInclusive(1, MaxSites);
                }

                int destroyedSiteCount = (MaxSites + 1) - remainingSites;
                destroyedSiteCount = Mathf.Clamp(destroyedSiteCount, 1, MaxSites);

                int ticksPerPhase = 21600 / MaxSites; // 7200 ticks
                int ticks = ticksPerPhase * destroyedSiteCount;

                //Log.Message($"[EpicSiege] Custom raid phases enabled. Remaining sites for faction {this.Faction.Name}: {remainingSites} destroyedSiteCount: {destroyedSiteCount} ticks: {ticks}");

                return ticks;
            }
            else
            {
                SitePartDef partDef = parts.FirstOrDefault()?.def;

                if (partDef == ESDefOf.ESMortarCamp) return 1000;
                if (partDef == ESDefOf.ESBreachCamp) return 10800;

                return 21600;
            }

            
        }


        //private void TriggerRetaliationRaidIfFailed()
        //{
        //    // これ多分だけど廃棄物クエストの名残だと思われ。廃棄物によるクエストは消去したため今は不要
        //    //if (this.WorldObjectTimeoutTicksLeft >= 1)
        //    //{
        //    //    SigeManager.DestroyAliveSigeSiteFaction(this);
        //    //    return;
        //    //}
        //    TimeoutComp component = this.GetComponent<TimeoutComp>();
        //    if (component != null && component.TicksLeft > 0)
        //    {
        //        // 敵拠点を時間内にクリアした
        //        return;
        //    }
        //    Map anyPlayerHomeMap = Find.AnyPlayerHomeMap;
        //    if (anyPlayerHomeMap == null)
        //    {
        //        // 襲撃するプレイヤーホームがない
        //        SigeManager.DestroyAliveSigeSiteFaction(this);
        //        return;
        //    }
        //    IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, anyPlayerHomeMap);
        //    incidentParms.forced = true;
        //    incidentParms.faction = this.Faction;
        //    incidentParms.target = anyPlayerHomeMap;
        //    incidentParms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
        //    float num = incidentParms.points * (float)EpicSiegeMod.settings.additionalMultiplier;
        //    incidentParms.points = num;
        //    int num2 = 21600;
        //    SitePart sitePart2 = this.parts[0];
        //    SitePartDef sitePartDef = (sitePart2 != null) ? sitePart2.def : null;
        //    if (sitePartDef == ESDefOf.ESMortarCamp)
        //    {
        //        incidentParms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("Siege", true);
        //        num2 = 1000;
        //        Log.Message("Prefix_Destroy: Triggering Siege Raid.");
        //        Log.Message(string.Format("Raid strategy: {0}", incidentParms.raidStrategy));
        //    }
        //    else if (sitePartDef == ESDefOf.ESBreachCamp)
        //    {
        //        incidentParms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("ImmediateAttackBreaching", true);
        //        num2 = 10800;
        //        Log.Message("Prefix_Destroy: Triggering Breach Raid.");
        //        Log.Message(string.Format("Raid strategy: {0}", incidentParms.raidStrategy));
        //    }
        //    else
        //    {
        //        incidentParms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
        //        Log.Message("Prefix_Destroy: Triggering Standard Raid.");
        //        Log.Message(string.Format("Raid strategy: {0}", incidentParms.raidStrategy));
        //    }
        //    Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + num2, incidentParms, 0);
        //}

    }
}
