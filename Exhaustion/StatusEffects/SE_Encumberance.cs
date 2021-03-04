using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Config = Exhaustion.Utility.RebalanceConfig;

namespace Exhaustion.StatusEffects
{
    public class SE_Encumberance : StatusEffect
    {
        public override void Setup(Character character)
        {
            m_name = "Encumberance";
            name = "Encumberance";
            m_ttl = -1f;

            base.Setup(character);
        }

        public override void ModifySpeed(ref float speed)
        {
            var player = (Player)m_character;

            if (player.IsEncumbered())
                return;

            var threshold = Config.EncumberanceAltThreshold.Value;
            if (player.GetMaxCarryWeight() > Config.BaseCarryWeight.Value)
            {
                threshold += player.GetMaxCarryWeight() - Config.BaseCarryWeight.Value;
            }

            var weight = player.GetInventory().GetTotalWeight() / threshold;

            var mult = Mathf.Lerp(Config.EncumberanceAltMaxSpeed.Value, Config.EncumberanceAltMinSpeed.Value, weight * weight );
            speed *= mult;
        }
    }
}
