using BepInEx.Configuration;
using HarmonyLib;
using Exhaustion.StatusEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Config = Exhaustion.Utility.RebalanceConfig;

namespace Exhaustion.Patches
{
    public static class PlayerPatches
    {
        

        [HarmonyPatch(typeof(Player), "Awake")]
        class PlayerAwakePatch
        {
            static void Postfix(Player __instance)
            {
                __instance.m_staminaRegen = Config.StaminaRegen.Value;
                __instance.m_staminaRegenDelay = Config.StaminaDelay.Value;
                __instance.m_dodgeStaminaUsage = Config.DodgeStamina.Value;
                __instance.m_maxCarryWeight = Config.BaseCarryWeight.Value;
                __instance.m_jumpStaminaUsage = Config.JumpStamina.Value;
                __instance.m_encumberedStaminaDrain = Config.EncumberedDrain.Value;
                __instance.m_acceleration = Config.Acceleration.Value;

                if (!__instance.GetSEMan().HaveStatusEffect("Encumberance") && Config.EncumberanceAltEnable.Value)
                {
                    var encumberance = ScriptableObject.CreateInstance<SE_Encumberance>();
                    __instance.GetSEMan().AddStatusEffect(encumberance);
                }
            }
        }

        /// <summary>
        ///     Patch have stamina result to allow going into negative stamina to allow regen delay punish to fire
        /// </summary>
        [HarmonyPatch(typeof(Player), "HaveStamina")]
        class PlayerHaveStaminaPatch
        {
            static void Postfix(float amount, ref bool __result, Player __instance, ZNetView ___m_nview, float ___m_maxStamina)
            {
                if (___m_nview.IsValid())
                {
                    if (___m_nview.IsOwner())
                    {
                        if (Config.ExhaustionEnable.Value && __instance.GetSEMan().HaveStatusEffect("Exhausted"))
                            __result = false;
                        __result = __instance.GetStamina() > 0.0f;
                    }
                    else if (!___m_nview.IsOwner())
                    {
                        __result = ___m_nview.GetZDO().GetFloat("stamina", ___m_maxStamina) > 0.0f;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), "CheckRun")]
        class PlayerCheckRunPatch
        {
            static void Postfix(ref bool __result, Player __instance, bool ___m_run)
            {
                if (Config.ExhaustionEnable.Value)
                {
                    if (__instance.GetStamina() <= 0f && __instance.GetStamina() > Config.StaminaMinimum.Value && ___m_run)
                    {
                        var seman = __instance.GetSEMan();
                        if (!seman.HaveStatusEffect("Pushing"))
                        {
                            var pushing = ScriptableObject.CreateInstance<SE_Pushing>();
                            seman.AddStatusEffect(pushing);
                        }
                        __result = !seman.HaveStatusEffect("Exhausted");
                    }

                    if (__instance.GetStamina() <= Config.ExhaustionThreshold.Value)
                    {
                        __instance.m_acceleration = Config.STAM_EXH_ACCEL;
                        if ((!__result || __instance.GetStamina() <= Config.StaminaMinimum.Value) && !__instance.GetSEMan().HaveStatusEffect("Exhausted"))
                        {
                            var exhausted = ScriptableObject.CreateInstance<SE_Exhausted>();
                            __instance.GetSEMan().AddStatusEffect(exhausted);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Patch stamina usage to increase regen delay when reaching 0 stamina to punish stamina mis-management
        /// </summary>
        [HarmonyPatch(typeof(Player), "UseStamina")]
        class PlayerUseStaminaPatch
        {
            static void Prefix(ref float v, Player __instance)
            {
                var placeMode = __instance.InPlaceMode();

                if (placeMode && v == 5.0f && !Config.BuildUseStamina.Value)
                {
                    v = 0.0f;
                }

                v *= Config.StaminaUseMultiplier.Value;
            }
        }

        [HarmonyPatch(typeof(Player), "RPC_UseStamina")]
        class PlayerRPCUseStaminaPatch
        {
            static void Prefix(Player __instance, out float __state)
            {
                __state = __instance.GetStamina();
            }

            static void Postfix(float v, Player __instance, ref float ___m_stamina, float __state)
            {
                if (__instance.GetStamina() <= 0f)
                {
                    if (__state - v < 0f)
                    {
                        ___m_stamina = Mathf.Clamp(__state - v, Config.StaminaMinimum.Value, __state);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), "SetMaxStamina")]
        class PlayerSetMaxStaminaPatch
        {
            static void Prefix(float ___m_stamina, out float __state)
            {
                __state = ___m_stamina;
            }

            static void Postfix(ref float ___m_stamina, float __state)
            {
                if (__state < 0f && ___m_stamina == 0f)
                {
                    ___m_stamina = __state;
                }
            }
        }

        [HarmonyPatch(typeof(Player), "UpdateStats")]
        class PlayerUpdateStatsPatch
        {
            static void Postfix(Player __instance)
            {
                var seman = __instance.GetSEMan();
                if (__instance.GetStaminaPercentage() >= 0f && seman.HaveStatusEffect("Pushing"))
                {
                    seman.RemoveStatusEffect("Pushing");
                }
                if (__instance.GetStaminaPercentage() >= Config.ExhaustionRecoveryThreshold.Value && seman.HaveStatusEffect("Exhausted"))
                {
                    seman.RemoveStatusEffect("Exhausted");
                    __instance.m_acceleration = Config.Acceleration.Value;
                }
            }
        }

        [HarmonyPatch(typeof(Player), "IsEncumbered")]
        class PlayerIsEncumberedPatch
        {
            static void Postfix(ref bool __result, Player __instance)
            {
                __result = __instance.GetInventory().GetTotalWeight() > Config.EncumberanceAltThreshold.Value + (__instance.GetMaxCarryWeight() - Config.BaseCarryWeight.Value);
            }
        }

        /// <summary>
        ///     Patch base HP, stamina, stamina regen and stamina regen delay
        /// </summary>
        [HarmonyPatch(typeof(Player), "GetTotalFoodValue")]
        class PlayerTotalFoodValuePatch
        {
            static void Postfix(ref float hp, ref float stamina, Player __instance)
            {
                hp = Config.BaseHealth.Value;
                stamina = Config.BaseStamina.Value;

                var foods = __instance.GetFoods();

                foreach (Player.Food food in foods)
                {
                    hp += food.m_health;
                    stamina += food.m_stamina;
                }
            }
        }

        /// <summary>
        ///     Patch base food hp function to have UI appear properly
        /// </summary>
        [HarmonyPatch(typeof(Player), "GetBaseFoodHP")]
        class PlayerBaseFoodHPPatch
        {
            static void Postfix(ref float __result)
            {
                __result = Config.BaseHealth.Value;
            }
        }

        /// <summary>
        ///     Patch blocktimer to make parrying more rewarding but harder to pull off
        ///     
        ///     ~halve time available to parry
        /// </summary>
        [HarmonyPatch(typeof(Humanoid), "BlockAttack")]
        class HumanoidBlockAttackPatch
        {
            static void Prefix(Player __instance, out float __state)
            {
                var traverse = Traverse.Create(__instance);
                var blockTimer = traverse.Field("m_blockTimer");
                var timerVal = (float)blockTimer.GetValue();
                __state = 0f;

                var currentBlocker = (ItemDrop.ItemData)traverse.Method("GetCurrentBlocker").GetValue();

                if (timerVal > Config.ParryTime.Value && timerVal < 0.25f && timerVal != -1.0f)
                {
                    blockTimer.SetValue(0.25f); //skip parry after 0.13
                }
                else if (timerVal <= Config.ParryTime.Value && currentBlocker.m_shared.m_timedBlockBonus > 1.0f)
                {
                    __state = currentBlocker.m_shared.m_timedBlockBonus * __instance.m_blockStaminaDrain * Config.ParryRefundMultiplier.Value;
                }
            }

            static void Postfix(bool __result, Player __instance, float __state)
            {
                if (__result && __state > 0.0f && Config.ParryRefundEnable.Value)
                    __instance.AddStamina(__state);
            }
        }

        [HarmonyPatch(typeof(Attack), "GetStaminaUsage")]
        class AttackGetStaminaUsagePatch
        {
            static void Postfix(ref float __result, Attack __instance)
            {
                if (Config.WeaponWeightStaminaScalingEnable.Value)
                {
                    var weight = __instance.GetWeapon().GetWeight();

                    var lerp = Mathf.LerpUnclamped(3.0f, 8.0f, weight / 3.0f);

                    __result += lerp;
                }
            }
        }
    }
}
