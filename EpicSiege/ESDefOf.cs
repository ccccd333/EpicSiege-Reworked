using System;
using RimWorld;

namespace EpicSiege
{
    [DefOf]
    public static class ESDefOf
    {
        static ESDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ESDefOf));
        }

        public static SitePartDef ESSiegeCamp;

        public static SitePartDef ESMortarCamp;

        public static SitePartDef ESBreachCamp;

        public static SitePartDef ESRaidCamp;

        public static IncidentDef ESEpicSiegeIncident;

        public static RaidStrategyDef Siege;

        public static RaidStrategyDef ImmediateAttackBreaching;

        public static RaidStrategyDef ImmediateAttack;

        public static WorldObjectDef EpicSiegeCampSite;
    }
}
