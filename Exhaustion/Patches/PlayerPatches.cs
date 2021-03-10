using HarmonyLib;
using Exhaustion.StatusEffects;
using UnityEngine;
using Config = Exhaustion.Utility.RebalanceConfig;
using System;
using ValheimLib.ODB;
using ValheimLib;

namespace Exhaustion.Patches
{
    public static class PlayerPatches
    {
        /// <summary>
        ///     Patch Awake to inject our configured values and apply the Encumberance status effect if enabled
        /// </summary>
        [HarmonyPatch(typeof(Player), "Awake")]
        class PlayerAwakePatch
        {
            static void Postfix(Player __instance)
            {
                //Prevent NREs from main menu fake player
                //TODO: Find a better solution, this function seems unreliable as it just returns "..." for any Player object *except* the main menu player
                if (string.IsNullOrEmpty(__instance.GetPlayerName()))
                    return;

                __instance.m_staminaRegen = Config.StaminaRegen.Value;
                __instance.m_staminaRegenDelay = Config.StaminaDelay.Value;
                __instance.m_dodgeStaminaUsage = Config.DodgeStamina.Value;
                __instance.m_maxCarryWeight = Config.BaseCarryWeight.Value;
                __instance.m_jumpStaminaUsage = Config.JumpStamina.Value;
                __instance.m_encumberedStaminaDrain = Config.EncumberedDrain.Value;
                __instance.m_acceleration = Config.Acceleration.Value;

                var seman = __instance.GetSEMan();
                if (!seman.HaveStatusEffect("Encumberance") && Config.EncumberanceAltEnable.Value)
                {
                    seman.AddStatusEffect("Encumberance");
                }
            }
        }

        /// <summary>
        ///     Patch HaveStamina result to allow going into negative stamina
        /// </summary>
        [HarmonyPatch(typeof(Player), "HaveStamina")]
        class PlayerHaveStaminaPatch
        {
            static void Postfix(ref bool __result, Player __instance, ZNetView ___m_nview, float ___m_maxStamina)
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

        /// <summary>
        ///     Patch CheckRun to allow the player to continue sprinting into negative stamina values and to apply the "Pushing" and "Exhausted" status effects when thresholds are met
        /// </summary>
        [HarmonyPatch(typeof(Player), "CheckRun")]
        class PlayerCheckRunPatch
        {
            static void Postfix(float dt, ref bool __result, Player __instance, bool ___m_run)
            {
                if (Config.ExhaustionEnable.Value)
                {
                    var seman = __instance.GetSEMan();

                    //We need to check ___m_run (Player.m_run) to see if the player is holding the run key
                    if (__instance.GetStamina() <= 0f && __instance.GetStamina() > Config.StaminaMinimum.Value && ___m_run)
                    {
                        if (!seman.HaveStatusEffect("Pushing"))
                        {
                            seman.AddStatusEffect("Pushing");
                        }

                        if (Config.PushingWarms.Value && !seman.HaveStatusEffect("Freezing") && !seman.HaveStatusEffect("Frost"))
                        {
                            if (!seman.HaveStatusEffect("Warmed"))
                            {
                                seman.AddStatusEffect("Warmed");
                            }
                            else
                            {
                                var warm = seman.GetStatusEffect("Warmed") as SE_Warmed;
                                warm.TTL += Config.PushingWarmTimeRate.Value * dt;
                            }
                        }
                        __result = !seman.HaveStatusEffect("Exhausted");
                    }

                    //If stamina falls below the exhaustion threshold apply acceleration debuff and "Exhausted" status effect
                    if (__instance.GetStamina() <= Config.ExhaustionThreshold.Value)
                    {
                        __instance.m_acceleration = Config.STAM_EXH_ACCEL;
                        if ((!__result || __instance.GetStamina() <= Config.StaminaMinimum.Value) && !seman.HaveStatusEffect("Exhausted"))
                        {
                            seman.RemoveStatusEffect("Pushing");
                            seman.AddStatusEffect("Exhausted");
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Patch UseStamina to apply usage multiplier and prevent stamina usage in place-mode
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

        /// <summary>
        ///     Patch RPC_UseStamina to prevent it from clamping stamina to positive values
        /// </summary>
        [HarmonyPatch(typeof(Player), "RPC_UseStamina")]
        class PlayerRPCUseStaminaPatch
        {
            static void Prefix(Player __instance, out float __state)
            {
                //Store current stamina in state before RPC_UseStamina is executed
                __state = __instance.GetStamina();
            }

            static void Postfix(float v, Player __instance, ref float ___m_stamina, float __state)
            {
                //If in negative stamina values and the original state - v (the amount of stamina used) is also negative, clamp stamina to the configured minimum value instead 
                if (__instance.GetStamina() <= 0f && __state - v < 0f)
                {
                    ___m_stamina = Mathf.Clamp(__state - v, Config.StaminaMinimum.Value, __state);
                }
            }
        }

        /// <summary>
        ///     Patch SetMaxStamina to prevent it from resetting stamina to positive values similarly to RPC_UseStamina
        /// </summary>
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

        /// <summary>
        ///     Patch UpdateStats to remove the "Pushing" and "Exhausted" status effects when appropriate
        /// </summary>
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

        /// <summary>
        ///     Patch IsEncumbered to use configured values, accounting for modifications to the players max carry weight
        /// </summary>
        [HarmonyPatch(typeof(Player), "IsEncumbered")]
        class PlayerIsEncumberedPatch
        {
            static bool Prefix(ref bool __result, Player __instance)
            {
                if (!Config.EncumberanceAltEnable.Value)
                    return true;

                __result = __instance.GetInventory().GetTotalWeight() > Config.EncumberanceAltThreshold.Value + (__instance.GetMaxCarryWeight() - Config.BaseCarryWeight.Value);
                return false;
            }
        }

        /// <summary>
        ///     Patch GetTotalFoodValue base HP and stamina, unfortunately requires reimplementation of method as hp and stamina values were inlined
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
        ///     Patch GetBaseFoodHP to have UI appear properly
        /// </summary>
        [HarmonyPatch(typeof(Player), "GetBaseFoodHP")]
        class PlayerBaseFoodHPPatch
        {
            static void Postfix(ref float __result)
            {
                __result = Config.BaseHealth.Value;
            }
        }

        /* TECHNICALLY NOT PLAYER PATCHES BUT THEY FEEL LIKE THEY BELONG HERE */

        /// <summary>
        ///     Patch BlockAttack to allow customisation of parry timing
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

                //Use configured parry time
                if (timerVal > Config.ParryTime.Value && timerVal != -1.0f)
                {
                    blockTimer.SetValue(0.26f); //skip parry timing @0.25
                }
                else if (timerVal <= Config.ParryTime.Value && currentBlocker.m_shared.m_timedBlockBonus > 1.0f)
                {
                    blockTimer.SetValue(0.01f);
                    __state = currentBlocker.m_shared.m_timedBlockBonus * __instance.m_blockStaminaDrain * Config.ParryRefundMultiplier.Value;
                }
            }

            static void Postfix(bool __result, Player __instance, float __state)
            {
                //Refund stamina if enabled using stored value in state
                if (__result && __state > 0.0f && Config.ParryRefundEnable.Value)
                    __instance.AddStamina(__state);
            }
        }

        /// <summary>
        ///     Patch Attack GetStaminaUsage to use configured weapon weight scaling
        ///     
        ///     TODO: Consider allowing configuration of lerp values
        /// </summary>
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

        [HarmonyPatch(typeof(SEMan), "AddStatusEffect")]
        [HarmonyPatch(new Type[] { typeof(string), typeof(bool) })]
        class SEManAddStatusEffectPatch
        {
            static bool Prefix(ref StatusEffect __result, string name, SEMan __instance)
            {
                if (string.Equals(name, "Cold") && Config.PushingWarms.Value)
                {
                    if (__instance.HaveStatusEffect("Warmed"))
                    {
                        __result = null;
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
