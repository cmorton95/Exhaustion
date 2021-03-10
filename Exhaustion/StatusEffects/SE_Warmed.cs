﻿using HarmonyLib;
using Config = Exhaustion.Utility.RebalanceConfig;

namespace Exhaustion.StatusEffects
{
    public class SE_Warmed : StatusEffect
    {
        public float TTL
        {
            get { return m_ttl; }
            set { m_ttl = value; }
        }

        public void Awake()
        {
            m_name = "Warmed Up";
            name = "Warmed";
            m_tooltip = "You are warmed up from pushing yourself. Removes <color=yellow>Cold</color> and reduces <color=yellow>Wet</color>.";
        }

        public override void Setup(Character character)
        {
            m_icon = Utility.Utilities.WarmSprite;
            TTL = Config.PushingWarmInitialTime.Value;

            base.Setup(character);
        }

        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);
            var seman = m_character.GetSEMan();
            var wet = seman.GetStatusEffect("Wet");
            if (wet != null)
            {
                var time = Traverse.Create(wet).Field("m_time");
                time.SetValue((float)time.GetValue() + (Config.PushingWarmRate.Value * dt));
            }
            if (seman.HaveStatusEffect("Cold"))
            {
                seman.RemoveStatusEffect("Cold");
            }
        }
    }
}
