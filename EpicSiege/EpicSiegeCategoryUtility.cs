using RimWorld;

public static class EpicSiegeCategoryUtility
{
    public static IncidentCategoryDef GetCategory(int index)
    {
        switch (index)
        {
            case 0: return IncidentCategoryDefOf.ThreatSmall;
            case 1: return IncidentCategoryDefOf.ThreatBig;
            case 2: return IncidentCategoryDefOf.Misc;
            default: return IncidentCategoryDefOf.ThreatBig;
        }
    }

    public static string GetCategoryLabel(int index)
    {
        switch(index)
        {
            case 0: return "ESM_CategoryThreatSmall";
            case 1: return "ESM_CategoryThreatBig";
            case 2: return "ESM_CategoryMisc";
            default: return "ESM_CategoryThreatBig";
        }
    }
}