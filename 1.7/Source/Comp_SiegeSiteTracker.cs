using RimWorld;
using RimWorld.Planet;
using Verse;

namespace EpicSiege
{
    public class Comp_SiegeSiteTracker : WorldObjectComp
    {
        public override void Initialize(WorldObjectCompProperties props)
        {
            base.Initialize(props);
            // Game.LoadGame
            //   Scribe_Collections.Look<Map>(ref this.maps, "maps", LookMode.Deep, Array.Empty<object>());
            //   Scribe.loader.FinalizeLoading();
            //   this.initer.DoAllPostLoadInits();
            //   ここにくる。これはGame.LoadedGameやFinalizeInitより早い段階

            // 後は紐づくWorldObject生成時
            SiegeSiteManager.Initialize(this);
        }

        public override void PostMapGenerate()
        {
            if (parent is Site s)
            {
                SiegeSiteManager.RegisterSite(this);

                // debug用
                //s.Faction;
                //        var m = s.Map;

                //        Log.Message("Hostile active threat: " +
                //GenHostility.AnyHostileActiveThreatToPlayer(m, true));

                //        foreach (var pawn in m.mapPawns.AllPawns)
                //        {
                //            Log.Message($"{pawn} hostile={pawn.HostileTo(Faction.OfPlayer)}");
                //        }
            }
        }

        // Map.Destroyの後のタイミングでよいもの

        //public override void PostDestroy()
        //{
        //    base.PostDestroy();
        //}

        // Map.Destroyされる前でやるべきもの
        //public override void PostMyMapRemoved()
        //{

        //}
    }

}
