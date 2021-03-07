using BepInEx;
using HarmonyLib;
using Exhaustion.Utility;

namespace Exhaustion
{
    /// <summary>
    ///     Load harmony patches
    /// </summary>
    [BepInPlugin("dev.7dd.exhaustion", "Exhaustion", "1.1.0")]
    public class ExhaustionPlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            RebalanceConfig.Bind(Config);

            DoPatching();
        }

        public static void DoPatching()
        {
            var harmony = new Harmony("dev.7dd.exhaustion");
            harmony.PatchAll();
        }
    }
}
