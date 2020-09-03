using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.BetterSliders
{
    [UsedImplicitly]
    public class BetterSliders : Mod
    {
        public BetterSliders(ModContentPack content) : base(content)
        {
        }
    }

    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public static class HarmonyPatcher
    {
        static HarmonyPatcher()
        {
            new Harmony("com.sirrandoo.bettersliders").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
