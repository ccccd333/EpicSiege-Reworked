using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
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

        private string PhaseTypeToString(int phaseType)
        {
            switch (phaseType)
            {
                case 2:
                    return "ESM_SiegeType0";
                case 1:
                    return "ESM_SiegeType1";
                case 0:
                    return "ESM_SiegeType2";
                default:
                    return "Unknown";
            }
        }

        private void SetPhaseType(int phaseType,string phase)
        {
            switch (phase)
            {
                case "ESM_RaidPhase1":
                    EpicSiegeMod.settings.raidPhase1 = phaseType;
                    break;
                case "ESM_RaidPhase2":
                    EpicSiegeMod.settings.raidPhase2 = phaseType;
                    break;
                case "ESM_RaidPhase3":
                    EpicSiegeMod.settings.raidPhase3 = phaseType;
                    break;
            }
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

            // CustomRaidPhases用ボタン
            listing_Standard.CheckboxLabeled(
                "ESM_CustomRaidPhases".Translate(),
                ref EpicSiegeMod.settings.customRaidPhases);

            if (EpicSiegeMod.settings.customRaidPhases)
            {
                DropDownList_Phase(listing_Standard, EpicSiegeMod.settings.raidPhase1, "ESM_RaidPhase1", "ESM_RaidPhase1Description");
                DropDownList_Phase(listing_Standard, EpicSiegeMod.settings.raidPhase2, "ESM_RaidPhase2", "ESM_RaidPhase2Description");
                DropDownList_Phase(listing_Standard, EpicSiegeMod.settings.raidPhase3, "ESM_RaidPhase3", "ESM_RaidPhase3Description");
            }
            listing_Standard.End();
            base.DoSettingsWindowContents(inRect);
        }

        private void DropDownList_Phase(Listing_Standard listing_Standard,int phase, string PhaseLabel, string PhaseDescLabel)
        {
            string PhaseType = PhaseTypeToString(phase).Translate();

            // Phase1ドロップダウン
            listing_Standard.Label(PhaseLabel.Translate() + " : " + PhaseType, -1f, PhaseDescLabel.Translate());

            Rect dropdownRect = listing_Standard.GetRect(30f); // ドロップダウン用の矩形
            if (Widgets.ButtonText(dropdownRect, PhaseType))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                for (int i = 0; i <= 2; i++)
                {
                    int captured = i; // クロージャ用にキャプチャ
                    options.Add(new FloatMenuOption(PhaseTypeToString(i).Translate(), () => SetPhaseType(captured, PhaseLabel)));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }
        }

        // Token: 0x04000005 RID: 5
        public static EpicSiegeModSettings settings;
    }
}

