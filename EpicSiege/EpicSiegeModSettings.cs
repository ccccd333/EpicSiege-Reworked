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
            base.ExposeData();
        }

        public void SetDefault()
        {
            this.alterDaysBeforeRaid = 1.5f;
            this.additionalMultiplier = 1;
        }

        public float alterDaysBeforeRaid = 1.5f;

        public int additionalMultiplier = 1;
    }
}
