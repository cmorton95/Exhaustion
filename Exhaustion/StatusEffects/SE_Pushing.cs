using HarmonyLib;
using Config = Exhaustion.Utility.RebalanceConfig;

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
