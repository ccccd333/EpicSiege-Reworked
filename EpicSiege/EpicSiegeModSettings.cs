using System;
using Verse;

namespace EpicSiege
{
    public class EpicSiegeModSettings : ModSettings
    {
        public override void ExposeData()
        {
            Scribe_Values.Look<float>(ref this.alterDaysBeforeRaid, "alterDaysBeforeRaid", 1.5f, false);
            Scribe_Values.Look<int>(ref this.additionalMultiplier, "additionalMultiplier", 1, false);

            Scribe_Values.Look<int>(ref this.raidPhase1, "raidPhase1", 2, false);
            Scribe_Values.Look<int>(ref this.raidPhase2, "raidPhase2", 1, false);
            Scribe_Values.Look<int>(ref this.raidPhase3, "raidPhase3", 0, false);

            Scribe_Values.Look<bool>(ref this.customRaidPhases, "customRaidPhases", false, false);

            Scribe_Values.Look<bool>(ref enableCategoryOverride, "enableCategoryOverride", false);
            Scribe_Values.Look<int>(ref selectedCategoryIndex, "selectedCategoryIndex", 1, false);

            Scribe_Values.Look<float>(ref baseChance, "baseChance", 1f, false);
            Scribe_Values.Look<float>(ref minThreatPoints, "minThreatPoints", 1000f, false);
            Scribe_Values.Look<int>(ref this.minPopulation, "minPopulation", 4, false);
            Scribe_Values.Look<int>(ref this.minRefireDays, "minRefireDays", 30, false);
            Scribe_Values.Look<int>(ref this.earliestDay, "earliestDay", 20, false);
            base.ExposeData();
        }

        public void SetDefault()
        {
            this.alterDaysBeforeRaid = 1.5f;
            this.additionalMultiplier = 1;
        }

        public float alterDaysBeforeRaid = 1.5f;

        public int additionalMultiplier = 1;

        public int raidPhase1 = 2;
        public int raidPhase2 = 1;
        public int raidPhase3 = 0;

        public bool customRaidPhases = false;

        public bool enableCategoryOverride = false;

        // 0=ThreatSmall, 1=ThreatBig, 2=Misc
        public int selectedCategoryIndex = 1;
        public float baseChance = 1f;
        public float minThreatPoints = 1000f;
        public int minPopulation = 4;
        public int minRefireDays = 30;
        public int earliestDay = 20;
    }
}
