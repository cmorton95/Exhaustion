﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Config = Exhaustion.Utility.RebalanceConfig;

namespace Exhaustion.Patches
{
    [HarmonyPatch(typeof(ItemDrop), "Awake")]
    class ItemDropAwakePatch
    {

        static void Prefix(ref ItemDrop __instance)
        {
            if (__instance.m_itemData.m_shared.m_food > 0)
            {
                var foodHealthBase = __instance.m_itemData.m_shared.m_food;
                var foodHealth = (float)Math.Max(Config.FoodHealthMin.Value, Math.Round(foodHealthBase * Config.FoodHealthMultiplier.Value));
                var foodStamBase = __instance.m_itemData.m_shared.m_foodStamina;
                var foodStam = (float)Math.Max(Config.FoodHealthMin.Value, Math.Round(foodStamBase * Config.FoodHealthMultiplier.Value));

                __instance.m_itemData.m_shared.m_food = foodHealth;
                __instance.m_itemData.m_shared.m_foodStamina = foodStam;
                __instance.m_itemData.m_shared.m_foodBurnTime = (float)Math.Round(__instance.m_itemData.m_shared.m_foodBurnTime * Config.FoodBurnTimeMultiplier.Value);
            }
        }
    }
}
