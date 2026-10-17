using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static RimWorld.ColonistBar;

namespace EpicSiege
{
    public static class EpicSiegeLetterUtility
    {
        public static void SendCampDestroyedLetter()
        {
            Letter letter = LetterMaker.MakeLetter("CampDestroyedName".Translate(), "CampDestroyedDescription".Translate(), LetterDefOf.PositiveEvent, null, null);
            Find.LetterStack.ReceiveLetter(letter, null, 0, true);
        }

        public static void SendRetreatLetter(Faction enemy)
        {
            Letter letter2 = LetterMaker.MakeLetter("RetreatName".Translate(), enemy.NameColored + "RetreatText".Translate(), LetterDefOf.PositiveEvent, null, null);
            Find.LetterStack.ReceiveLetter(letter2, null, 0, true);
        }

        public static void SendPeaceOutcomeLetter(Faction enemy, Quest quest)
        {
            Letter letter3 = LetterMaker.MakeLetter("OutcomeName".Translate(), enemy.NameColored + "OutcomeText".Translate(), LetterDefOf.PositiveEvent, enemy, quest);
            Find.LetterStack.ReceiveLetter(letter3, null, 0, true);
        }

        public static void SendLeaderExecutionLetter(Faction enemy)
        {
            TaggedString taggedString = "executeLeaderLabel".Translate();
            Name name = enemy.leader.Name;
            Letter letter4 = LetterMaker.MakeLetter(taggedString, ((name != null) ? name.ToString() : null) + "executeLeaderText1".Translate() + enemy.NameColored + "executeLeaderText2".Translate(), LetterDefOf.PositiveEvent, enemy, null);
            Find.LetterStack.ReceiveLetter(letter4, null, 0, true);
        }
    }
}
