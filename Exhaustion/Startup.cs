using BepInEx;
using HarmonyLib;
using Exhaustion.Utility;
using ValheimLib;
using ValheimLib.ODB;
using UnityEngine;

namespace Exhaustion
{
    /// <summary>
    ///     Load harmony patches
    /// </summary>
    [BepInPlugin("dev.7dd.exhaustion", "Exhaustion", "1.4.0")]
    public class ExhaustionPlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Log.Init(Logger);

            RebalanceConfig.Bind(Config);

            SetupStatusEffects();

            ObjectDBHelper.OnAfterInit += SetupIcons;
            ObjectDBHelper.OnAfterInit += SetupEffects;

            DoPatching();
        }

        public static void DoPatching()
        {
            var harmony = new Harmony("dev.7dd.exhaustion");
            harmony.PatchAll();
            Log.LogInfo("Patching complete");
        }

        private void SetupIcons()
        {
            Utilities.WarmSprite = Prefab.Cache.GetPrefab<Sprite>("Warm");
            Utilities.SweatSprite = Prefab.Cache.GetPrefab<Sprite>("Wet");
            Log.LogInfo("Sprites retrieved");
        }

        private void SetupEffects()
        {
            var vfxWet = Prefab.Cache.GetPrefab<GameObject>("vfx_Wet");
            Utilities.WetEffect = new EffectList.EffectData()
            {
                m_prefab = vfxWet,
                m_enabled = true,
                m_attach = true,
                m_inheritParentRotation = false,
                m_inheritParentScale = false,
                m_randomRotation = false,
                m_scale = true
            };
            Log.LogInfo("VFX retrieved");
        }

        private void SetupStatusEffects()
        {
            ObjectDBHelper.Add(new CustomStatusEffect(ScriptableObject.CreateInstance<StatusEffects.SE_Encumbrance>(), true));
            ObjectDBHelper.Add(new CustomStatusEffect(ScriptableObject.CreateInstance<StatusEffects.SE_Exhausted>(), true));
            ObjectDBHelper.Add(new CustomStatusEffect(ScriptableObject.CreateInstance<StatusEffects.SE_Pushing>(), true));
            ObjectDBHelper.Add(new CustomStatusEffect(ScriptableObject.CreateInstance<StatusEffects.SE_Warmed>(), true));
            Log.LogInfo("Status effects injected");
        }
    }
}
