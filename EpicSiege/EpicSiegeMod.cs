using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace EpicSiege
{
    // Token: 0x02000007 RID: 7
    public class EpicSiegeMod : Mod
    {
        public EpicSiegeMod(ModContentPack content)
            : base(content)
        {
            EpicSiegeMod.settings = base.GetSettings<EpicSiegeModSettings>();
            new Harmony("EpicSiegeMod").PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory()
        {
            return "ESM_EpicSiegeMod".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.Label("ESM_AlterDaysBeforeRaid".Translate() + ": " + EpicSiegeMod.settings.alterDaysBeforeRaid.ToString("F1"), -1f, "ESM_AlterDaysBeforeRaidDescription".Translate());
            EpicSiegeMod.settings.alterDaysBeforeRaid = listing_Standard.Slider(EpicSiegeMod.settings.alterDaysBeforeRaid, 0.5f, 15f);
            EpicSiegeMod.settings.additionalMultiplier = (int)listing_Standard.SliderLabeled("ESM_AdditionalMultiplier".Translate() + EpicSiegeMod.settings.additionalMultiplier.ToString(), (float)EpicSiegeMod.settings.additionalMultiplier, 1f, 10f, 0.5f, "ESM_AdditionalMultiplierDescription".Translate());
            listing_Standard.End();
            base.DoSettingsWindowContents(inRect);
        }

        // Token: 0x04000005 RID: 5
        public static EpicSiegeModSettings settings;
    }
}

