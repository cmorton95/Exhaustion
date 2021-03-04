using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exhaustion.StatusEffects
{
    class SE_Pushing : StatusEffect
    {
        public override void Setup(Character character)
        {
            m_name = "Pushing";
            name = "Pushing";

            var vfxWet = ZNetScene.instance.GetPrefab("vfx_Wet");

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

        public void UpdateTTL(float ttl)
        {
            m_ttl = ttl;
        }

        public override void ModifySpeed(ref float speed)
        {
            speed *= 0.85f;
        }
    }
}
