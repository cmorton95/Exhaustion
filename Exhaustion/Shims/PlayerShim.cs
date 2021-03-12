using Exhaustion.StatusEffects;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Config = Exhaustion.Utility.RebalanceConfig;

namespace Exhaustion.Managers
{
    public class PlayerShim
    {
        private Player Player { get; set; }
        private SEMan SEMan { get; set; }
        private ZDOID ZDOID { get; }

        #region Traversal
        private Traverse _stamina;
        private Traverse _maxStamina;
        private Traverse _runKey;
        private Traverse _nview;
        private Traverse _regenTimer;
        private Traverse _blockTimer;
        private Traverse _blocker;

        private float Stamina
        {
            get => (float)_stamina.GetValue();
            set => _stamina.SetValue(value);
        }

        private float MaxStamina
        {
            get => (float)_maxStamina.GetValue();
            set => _maxStamina.SetValue(value);
        }

        private float StaminaRegenTimer
        {
            get => (float)_regenTimer.GetValue();
            set => _regenTimer.SetValue(value);
        }

        private bool RunKey
        {
            get => (bool)_runKey.GetValue();
            set => _runKey.SetValue(value);
        }

        private float ParryTimer
        {
            get => (float)_blockTimer.GetValue();
            set => _blockTimer.SetValue(value);
        }
        private ZNetView NView 
        { 
            get => (ZNetView)_nview.GetValue(); 
        }

        private ItemDrop.ItemData CurrentBlocker
        {
            get => (ItemDrop.ItemData)_blocker.GetValue();
        }
        #endregion

        #region Field accessor properties
        private float Acceleration
        {
            get => Player.m_acceleration;
            set => Player.m_acceleration = value;
        }

        private float StaminaRegenDelay
        {
            get => Player.m_staminaRegenDelay;
            set => Player.m_staminaRegenDelay = value;
        }
        #endregion

        #region Properties
        private float ParryRefundAmount { get; set; }
        private SE_Warmed WarmedStatusEffect { get; set; }
        #endregion

        #region Shortcut getter properties
        private float StaminaPercentage => Player.GetStaminaPercentage();
        private bool IsPushing => SEMan.HaveStatusEffect("Pushing");
        private bool IsExhausted => SEMan.HaveStatusEffect("Exhausted");
        private bool IsWarmedUp => SEMan.HaveStatusEffect("Warmed");
        private bool IsFreezing => SEMan.HaveStatusEffect("Freezing") || SEMan.HaveStatusEffect("Frost");
        #endregion

        public PlayerShim(Player player)
        {
            Player = player;
            SEMan = Player.GetSEMan();
            ZDOID = Player.GetZDOID();

            var traverse = Traverse.Create(Player);
            _stamina = traverse.Field("m_stamina");
            _maxStamina = traverse.Field("m_maxStamina");
            _runKey = traverse.Field("m_run");
            _nview = traverse.Field("m_nview");
            _regenTimer = traverse.Field("m_staminaRegenTimer");
            _blockTimer = traverse.Field("m_blockTimer");
            _blocker = traverse.Method("GetCurrentBlocker");

            ConfigurePlayer();

            Debug.Log($"Create player shim: {ZDOID}");
        }

        public bool CheckStamina(float amount)
        {
            if (NView.IsValid() && !NView.IsOwner())
            {
                return NView.GetZDO().GetFloat("stamina", MaxStamina) > (RunKey && amount == 0.0f ? Config.StaminaMinimum.Value : 0.0f);
            }
            else
            {
                return Stamina > (RunKey && amount == 0.0f ? Config.StaminaMinimum.Value : 0.0f);
            }
        }

        public void CheckAndAddExhaustion()
        {
            if (Stamina <= Config.PushingThreshold.Value && Stamina > Config.StaminaMinimum.Value && (RunKey || StaminaPercentage > Config.PushingThreshold.Value))
            {
                AddPushing();
                if (Config.PushingWarms.Value)
                    AddWarmedUp();
            }

            //If stamina falls below the exhaustion threshold apply acceleration debuff and "Exhausted" status effect
            if (Stamina <= Config.ExhaustionThreshold.Value)
            {
                if ((!RunKey || Stamina <= Config.StaminaMinimum.Value))
                {
                    AddExhausted();
                }
            }
        }

        public void CheckAndRemoveExhaustion(float dt)
        {
            if (Config.PushingWarms.Value && IsWarmedUp && !IsFreezing && Stamina < Config.PushingThreshold.Value && RunKey)
            {
                UpdateWarmedUp(dt);
            }
            if (Player.GetStaminaPercentage() >= Config.PushingThreshold.Value && IsPushing)
            {
                RemovePushing();
            }
            if (Player.GetStaminaPercentage() >= Config.ExhaustionRecoveryThreshold.Value && IsExhausted)
            {
                RemoveExhausted();
            }
        }

        public bool CheckEncumbered()
        {
            return Player.GetInventory().GetTotalWeight() > Config.EncumberanceAltThreshold.Value + (Player.GetMaxCarryWeight() - Config.BaseCarryWeight.Value);
        }

        public float GetBaseHp()
        {
            return Config.BaseHealth.Value;
        }

        public float GetNewStaminaUsage(float amount)
        {
            var placeMode = Player.InPlaceMode();

            if (placeMode && amount == 5.0f && !Config.BuildUseStamina.Value)
            {
                return 0.0f;
            }

             return amount * Config.StaminaUseMultiplier.Value;
        }

        public void UpdateParry()
        {
            var timerVal = ParryTimer;
            var blocker = CurrentBlocker;

            //Use configured parry time
            if (timerVal > Config.ParryTime.Value && timerVal != -1.0f)
            {
                ParryTimer = 0.26f; //skip parry timing @0.25
            }
            else if (timerVal <= Config.ParryTime.Value && blocker.m_shared.m_timedBlockBonus > 1.0f)
            {
                ParryTimer = 0.01f;
                ParryRefundAmount = blocker.m_shared.m_timedBlockBonus * Player.m_blockStaminaDrain * Config.ParryRefundMultiplier.Value;
            }
        }

        public void UpdateParryRefund(bool blockSuccess)
        {
            if (blockSuccess && ParryRefundAmount > 0.0f)
                Player.AddStamina(ParryRefundAmount);
        }

        public void UpdateStamina(float amount)
        {
            Stamina = Mathf.Clamp(Stamina - amount, Config.StaminaMinimum.Value, MaxStamina);
            StaminaRegenTimer = StaminaRegenDelay;
        }

        public void UpdateMaxStamina(float max, bool flashBar)
        {
            if (max > MaxStamina && flashBar)
                Hud.instance?.StaminaBarUppgradeFlash();

            MaxStamina = max;
            Stamina = Mathf.Clamp(Stamina, Config.StaminaMinimum.Value, MaxStamina);
        }

        public void Destroy(Player player)
        {
            if (Player != player)
                return;

            Player = null;
            SEMan = null;
            Patches.PlayerPatches.Unassign(this);
            Debug.Log($"Destroyed player shim: {ZDOID}");
        }

        private void ConfigurePlayer()
        {
            Player.m_staminaRegen = Config.StaminaRegen.Value;
            Player.m_staminaRegenDelay = Config.StaminaDelay.Value;
            Player.m_dodgeStaminaUsage = Config.DodgeStamina.Value;
            Player.m_maxCarryWeight = Config.BaseCarryWeight.Value;
            Player.m_jumpStaminaUsage = Config.JumpStamina.Value;
            Player.m_encumberedStaminaDrain = Config.EncumberedDrain.Value;
            Player.m_acceleration = Config.Acceleration.Value;
        }

        private void AddPushing()
        {
            if (!IsPushing)
                SEMan.AddStatusEffect("Pushing");
        }

        private void AddExhausted() 
        {
            if (!IsExhausted)
            {
                if (IsPushing)
                    SEMan.RemoveStatusEffect("Pushing");

                SEMan.AddStatusEffect("Exhausted");
                Acceleration = Config.Acceleration.Value;
            }
        }

        private void AddWarmedUp()
        {
            if (!IsWarmedUp)
                WarmedStatusEffect = (SE_Warmed)SEMan.AddStatusEffect("Warmed");
        }

        private void RemovePushing() =>
            SEMan.RemoveStatusEffect("Pushing");

        private void RemoveExhausted()
        {
            SEMan.RemoveStatusEffect("Exhausted");
            Acceleration = Config.Acceleration.Value;
        }

        private void UpdateWarmedUp(float dt)
        {
            if (IsWarmedUp)
            {
                WarmedStatusEffect.TTL += Config.PushingWarmTimeRate.Value * dt;

                if (SEMan.HaveStatusEffect("Cold"))
                {
                    SEMan.RemoveStatusEffect("Cold");
                }
            }
        }
    }
}
