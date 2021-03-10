﻿using HarmonyLib;
using UnityEngine;
using ValheimLib;
using Config = Exhaustion.Utility.RebalanceConfig;

namespace Exhaustion.StatusEffects
{
    class SE_Pushing : StatusEffect
    {
        public void Awake()
        {
            m_name = "Pushing";
            name = "Pushing";
        }

        public override void Setup(Character character)
        {
            var vfxWet = Prefab.Cache.GetPrefab<GameObject>("vfx_Wet");

            m_startEffects = new EffectList();
            m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { Utility.Utilities.WetEffect };

            base.Setup(character);
        }

        public override void UpdateStatusEffect(float dt)
        {
            if (Config.PushingWarms.Value)
            {
                var seman = m_character.GetSEMan();
                if (seman.HaveStatusEffect("Wet"))
                {
                    var wet = seman.GetStatusEffect("Wet");
                    var time = Traverse.Create(wet).Field("m_time");
                    time.SetValue((float)time.GetValue() + (Config.PushingWarmRate.Value * dt));
                }
                if (seman.HaveStatusEffect("Cold"))
                {
                    seman.RemoveStatusEffect("Cold");
                }
            }
        }

        public override void ModifySpeed(ref float speed)
        {
            speed *= Config.PushingSpeedMultiplier.Value;
        }
    }
}
