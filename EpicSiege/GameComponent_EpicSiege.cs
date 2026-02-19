using RimWorld;
using System.Diagnostics;
using UnityEngine;
using Verse;

namespace EpicSiege
{
    public class GameComponent_EpicSiege : GameComponent
    {
        public static GameComponent_EpicSiege Instance { get; private set; }

        public GameComponent_EpicSiege(Game game) : base()
        {
            //EpicSiegeDebugUtility.LogCallStack("GameComponent_EpicSiege Constructor");

            // compsの初期化より先に来るのはgame compのここくらいかなぁ・・・
            // 新しいゲームならRimWorld.Page_SelectScenario:BeginScenarioConfigurationから呼ばれる
            SiegeSiteManager.ClearAll();
            Instance = this;
        }

        public override void FinalizeInit()
        {
            ApplyIncidentDefOverrides();



            //EpicSiegeDebugUtility.DebugLogIncident_EpicSiege(ESDefOf.ESEpicSiegeIncident);
        }

        public void ApplyIncidentDefOverrides()
        {
            // Defsはアノテーションで初期化されているため、どこでもいいがゲーム開始後クロスリファレンス解決後に行うのが確実
            if (EpicSiegeMod.settings.enableCategoryOverride)
            {
                ESDefOf.ESEpicSiegeIncident.category = EpicSiegeCategoryUtility.GetCategory(EpicSiegeMod.settings.selectedCategoryIndex);
                if (EpicSiegeMod.settings.baseChance >= 0.0f)
                {
                    ESDefOf.ESEpicSiegeIncident.baseChance = EpicSiegeMod.settings.baseChance;
                }

                if (EpicSiegeMod.settings.minThreatPoints >= 0.0f)
                {
                    ESDefOf.ESEpicSiegeIncident.minThreatPoints = EpicSiegeMod.settings.minThreatPoints;
                }

                if (EpicSiegeMod.settings.minPopulation >= 0)
                {
                    ESDefOf.ESEpicSiegeIncident.minPopulation = EpicSiegeMod.settings.minPopulation;
                }

                if (EpicSiegeMod.settings.minRefireDays >= 0)
                {
                    ESDefOf.ESEpicSiegeIncident.minRefireDays = EpicSiegeMod.settings.minRefireDays;
                }

                if (EpicSiegeMod.settings.earliestDay >= 0)
                {
                    ESDefOf.ESEpicSiegeIncident.earliestDay = EpicSiegeMod.settings.earliestDay;
                }
            }
        }
    }
}
