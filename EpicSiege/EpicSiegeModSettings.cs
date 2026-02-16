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
    }
}
