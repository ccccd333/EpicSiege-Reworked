using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EpicSiege
{
    public static class EpicSiegeDebugUtility
    {
        public static void DebugLogCallStack(string context = null)
        {
            Log.Warning(
                $"[DebugTrace] {context ?? "CallStack"}\n" +
                new StackTrace(true)
            );
        }

        public static void DebugLogRegisteredSites()
        {
            foreach (var comp in SiegeSiteManager.RegisteredSites.Keys)
            {
                Log.Message($"[EpicSiege] Registered siege site ==>{comp}");
            }
        }

        public static void DebugLogIncident_EpicSiege(IncidentDef def)
        {
            if (def == null)
            {
                Log.Message("[EpicSiege] IncidentDef is null");
                return;
            }

            Log.Message($"[EpicSiege] ===== IncidentDef: {def.defName} =====");

            var fields = typeof(IncidentDef).GetFields(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance
            );

            foreach (var field in fields)
            {
                object value = field.GetValue(def);
                Log.Message($"[EpicSiege] {field.Name} = {value}");
            }
        }
    }
}
