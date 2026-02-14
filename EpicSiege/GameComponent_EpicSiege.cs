using UnityEngine;
using Verse;

namespace EpicSiege
{
    public class GameComponent_EpicSiege : GameComponent
    {
        public static GameComponent_EpicSiege Instance { get; private set; }

        public GameComponent_EpicSiege(Game game) : base()
        {
            // compsの初期化より先に来るのはgame compのここくらいかなぁ・・・
            SiegeSiteManager.ClearAll();
            Instance = this;
        }

        public override void FinalizeInit()
        {
            SiegeSiteManager.Debug();
        }
    }
}
