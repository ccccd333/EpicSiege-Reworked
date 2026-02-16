using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace EpicSiege
{
    public static class SiegeSiteManager
    {
        private static readonly Dictionary<Comp_SiegeSiteTracker, Site> registeredSites 
            = new Dictionary<Comp_SiegeSiteTracker, Site>();

        private static bool shouldClearCache = true;

        public static bool ShouldClearCache
        {
            get => shouldClearCache;
            set => shouldClearCache = value;
        }

        public static void Debug()
        {
            foreach (var comp in registeredSites.Keys)
            {
                Log.Message($"[EpicSiege] Registered siege site ==>{comp}");
            }
        }

        public static void ClearAll()
        {
            registeredSites.Clear();
        }

        public static void Initialize(Comp_SiegeSiteTracker my)
        {
            // 初期化時はsiteに情報がないので
            registeredSites[my] = null;
        }

        public static void RegisterSite(Comp_SiegeSiteTracker my)
        {
            if (my.parent is Site site)
            {
                registeredSites[my] = site;
            }
        }

        public static void NotifySiteCleared(EpicSiegeCampSite clearedSite, out int clearObjective)
        {
            Comp_SiegeSiteTracker foundKey = null;

            foreach (var comp in registeredSites.Keys)
            {
                if (comp.parent is EpicSiegeCampSite site && site == clearedSite)
                {
                    foundKey = comp;
                    break;
                }
            }

            if (foundKey != null)
            {
                registeredSites.Remove(foundKey);
            }

            clearObjective = CountRemainingSitesForFaction(clearedSite.Faction);

        }

        public static void RemoveSite(Site site)
        {
            Comp_SiegeSiteTracker keyToRemove = null;

            foreach (var pair in registeredSites)
            {
                if (pair.Key.parent is Site s && s == site)
                {
                    keyToRemove = pair.Key;
                    break;
                }
            }

            if (keyToRemove != null)
            {
                registeredSites.Remove(keyToRemove);
            }
        }

        public static int CountRemainingSitesForFaction(Faction faction)
        {
            int count = 0;

            foreach (var comp in registeredSites.Keys)
            {
                // factionはロード時は一意となるものしか生成しない。
                // 狼ファクションならサイトごとにFactionを生成しないのでそのまま比較
                if (comp.parent is Site site && site.Faction == faction)
                {
                    count++;
                }
            }

            return count;
        }

        public static void DestroyAliveSigeSiteFaction(Site triggerSite)
        {
            var faction = triggerSite.Faction;

            var toRemove = registeredSites.Keys
                .Where(c => c.parent is Site s && s.Faction == faction)
                .ToList();

            foreach (var comp in toRemove)
            {
                var site = comp.parent as Site;

                if (site != null &&
                    site != triggerSite &&
                    !site.Destroyed)
                {
                    site.Destroy();
                }

                registeredSites.Remove(comp);
            }
        }
    }
}
