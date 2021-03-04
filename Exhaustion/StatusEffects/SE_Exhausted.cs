using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Config = Exhaustion.Utility.RebalanceConfig;

namespace Exhaustion.StatusEffects
{
    public class SE_Exhausted : StatusEffect
    {
        public override void Setup(Character character)
        {
            m_name = "Exhausted";
            name = "Exhausted";
            m_ttl = -1;

            var vfxWet = ZNetScene.instance.GetPrefab("vfx_Wet");

            m_icon = Utility.Utilities.SweatSprite;
            m_startEffects = new EffectList();
            m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData()
                {
                    m_prefab = vfxWet,
                    m_enabled = true,
                    m_attach = true,
                    m_inheritParentRotation = false,
                    m_inheritParentScale = false,
                    m_randomRotation = false,
                    m_scale = true
                }
            };

            base.Setup(character);
        }

        public override void ModifySpeed(ref float speed)
        {
            speed *= Config.ExhaustionSpeedMultiplier.Value;
        }
    }
}
