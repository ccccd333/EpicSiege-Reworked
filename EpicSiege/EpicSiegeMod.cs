using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime;
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

        private void SetPhaseType(int phaseType, string phase)
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
            Rect viewRect = new Rect(0, 0, inRect.width - 17f, scrollHeight);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing_Standard = new Listing_Standard(inRect.AtZero(), () => scrollPosition)
            {
                maxOneColumn = true,
                ColumnWidth = viewRect.width
            };
            listing_Standard.Begin(viewRect);

            listing_Standard.Label("ESM_AlterDaysBeforeRaid".Translate() + ": " + EpicSiegeMod.settings.alterDaysBeforeRaid.ToString("F1"), -1f, "ESM_AlterDaysBeforeRaidDescription".Translate());
            EpicSiegeMod.settings.alterDaysBeforeRaid = listing_Standard.Slider(EpicSiegeMod.settings.alterDaysBeforeRaid, 0.5f, 15f);
            EpicSiegeMod.settings.additionalMultiplier = (int)listing_Standard.SliderLabeled("ESM_AdditionalMultiplier".Translate() + EpicSiegeMod.settings.additionalMultiplier.ToString(), (float)EpicSiegeMod.settings.additionalMultiplier, 1f, 10f, 0.5f, "ESM_AdditionalMultiplierDescription".Translate());

            listing_Standard.GapLine();

            // CategoryOverride用ボタン
            listing_Standard.CheckboxLabeled(
                "ESM_IncidentDefCategoryOverride".Translate(),
                ref EpicSiegeMod.settings.enableCategoryOverride);
            if (EpicSiegeMod.settings.enableCategoryOverride)
            {
                string label = EpicSiegeCategoryUtility.GetCategoryLabel(EpicSiegeMod.settings.selectedCategoryIndex).Translate();

                if (listing_Standard.ButtonText($"{"ESM_SelectedIncidentCategory".Translate()} {label}"))
                {
                    var options = new List<FloatMenuOption>{
                    new FloatMenuOption(
                        "ESM_IncidentCategoryThreatSmall".Translate(),
                        () => EpicSiegeMod.settings.selectedCategoryIndex = 0),
                    new FloatMenuOption(
                        "ESM_IncidentCategoryThreatBig".Translate(),
                        () => EpicSiegeMod.settings.selectedCategoryIndex = 1),
                    new FloatMenuOption(
                        "ESM_IncidentCategoryMisc".Translate(),
                        () => EpicSiegeMod.settings.selectedCategoryIndex = 2)};

                    Find.WindowStack.Add(new FloatMenu(options));
                }

                listing_Standard.Gap();


                LabeledFloatField(listing_Standard, "ESM_IncidentBaseChance", ref EpicSiegeMod.settings.baseChance);

                LabeledFloatField(listing_Standard, "ESM_IncidentMinThreatPoints", ref EpicSiegeMod.settings.minThreatPoints);

                LabeledIntField(listing_Standard, "ESM_MinPopulation", ref EpicSiegeMod.settings.minPopulation);

                LabeledIntField(listing_Standard, "ESM_MinRefireDays", ref EpicSiegeMod.settings.minRefireDays);

                LabeledIntField(listing_Standard, "ESM_EarliestDay", ref EpicSiegeMod.settings.earliestDay);

                listing_Standard.GapLine();
            }
            else
            {
                listing_Standard.GapLine();
            }

            // CustomRaidPhases用ボタン
            listing_Standard.CheckboxLabeled(
                "ESM_CustomRaidPhases".Translate(),
                ref EpicSiegeMod.settings.customRaidPhases);

            if (EpicSiegeMod.settings.customRaidPhases)
            {
                DropDownList_Phase(listing_Standard, EpicSiegeMod.settings.raidPhase1, "ESM_RaidPhase1", "ESM_RaidPhase1Description");
                DropDownList_Phase(listing_Standard, EpicSiegeMod.settings.raidPhase2, "ESM_RaidPhase2", "ESM_RaidPhase2Description");
                DropDownList_Phase(listing_Standard, EpicSiegeMod.settings.raidPhase3, "ESM_RaidPhase3", "ESM_RaidPhase3Description");
                listing_Standard.GapLine();
            }
            else
            {
                listing_Standard.GapLine();
            }

            scrollHeight = listing_Standard.CurHeight;

            listing_Standard.End();
            Widgets.EndScrollView();
            base.DoSettingsWindowContents(inRect);
        }

        private void DropDownList_Phase(Listing_Standard listing_Standard, int phase, string PhaseLabel, string PhaseDescLabel)
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

        private static void LabeledFloatField(Listing_Standard listing, string labelKey, ref float value, float labelWidthPercent = 0.4f)
        {
            Rect rect = listing.GetRect(Text.LineHeight);

            Rect labelRect = new Rect(
                rect.x,
                rect.y,
                rect.width * labelWidthPercent,
                rect.height);

            Rect textRect = new Rect(
                rect.x + rect.width * labelWidthPercent,
                rect.y,
                rect.width * (1f - labelWidthPercent),
                rect.height);

            Widgets.Label(labelRect, labelKey.Translate());

            string buffer = value.ToString();
            buffer = Widgets.TextField(textRect, buffer);

            if (float.TryParse(buffer, out float result))
            {
                value = result;
            }
        }

        private static void LabeledIntField(Listing_Standard listing, string labelKey, ref int value, float labelWidthPercent = 0.4f)
        {
            Rect rect = listing.GetRect(Text.LineHeight);

            Rect labelRect = new Rect(
                rect.x,
                rect.y,
                rect.width * labelWidthPercent,
                rect.height);

            Rect textRect = new Rect(
                rect.x + rect.width * labelWidthPercent,
                rect.y,
                rect.width * (1f - labelWidthPercent),
                rect.height);

            Widgets.Label(labelRect, labelKey.Translate());

            string buffer = value.ToString();
            buffer = Widgets.TextField(textRect, buffer);

            if (int.TryParse(buffer, out int result))
            {
                value = result;
            }
        }

        // Token: 0x04000005 RID: 5
        public static EpicSiegeModSettings settings;
        private Vector2 scrollPosition;
        private float scrollHeight = 0;
    }
}

