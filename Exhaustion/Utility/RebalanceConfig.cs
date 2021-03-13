using BepInEx.Configuration;
using System.Text;

namespace Exhaustion.Utility
{
    public static class RebalanceConfig
    {
        //Stamina
        public static ConfigEntry<float> BaseStamina { get; private set; }
        public static ConfigEntry<float> StaminaMinimum { get; private set; }
        public static ConfigEntry<float> StaminaRegen { get; private set; }
        public static ConfigEntry<float> StaminaDelay { get; private set; }
        public static ConfigEntry<float> DodgeStamina { get; private set; }
        public static ConfigEntry<float> JumpStamina { get; private set; }
        public static ConfigEntry<bool> BuildUseStamina { get; private set; }
        public static ConfigEntry<float> StaminaUseMultiplier { get; private set; }

        //Player
        public static ConfigEntry<float> BaseHealth { get; private set; }
        public static ConfigEntry<float> Acceleration { get; private set; }
        public static ConfigEntry<float> ParryTime { get; private set; }
        public static ConfigEntry<bool> ParryRefundEnable { get; private set; }
        public static ConfigEntry<float> ParryRefundMultiplier { get; private set; }
        public static ConfigEntry<bool> WeaponWeightStaminaScalingEnable { get; private set; }

        //Food
        public static ConfigEntry<float> FoodHealthMin { get; private set; }
        public static ConfigEntry<float> FoodHealthMultiplier { get; private set; }
        public static ConfigEntry<float> FoodStaminaMin { get; private set; }
        public static ConfigEntry<float> FoodStaminaMultiplier { get; private set; }
        public static ConfigEntry<float> FoodBurnTimeMultiplier { get; private set; }

        //Exhaustion
        public static ConfigEntry<bool> ExhaustionEnable { get; private set; }
        public static ConfigEntry<float> ExhaustionThreshold { get; private set; }
        public static ConfigEntry<float> ExhaustionRecoveryThreshold { get; private set; }
        public static ConfigEntry<float> ExhaustionSpeedMultiplier { get; private set; }
        public static ConfigEntry<float> PushingThreshold { get; private set; }
        public static ConfigEntry<float> PushingSpeedMultiplier { get; private set; }
        public static ConfigEntry<bool> PushingWarms { get; private set; }
        public static ConfigEntry<float> PushingWarmRate { get; private set; }
        public static ConfigEntry<float> PushingWarmTimeRate { get; private set; }
        public static ConfigEntry<float> PushingWarmInitialTime { get; private set; }
        public static float ExhaustedAcceleration => 0.02f;

        //Encumbrance
        public static ConfigEntry<float> BaseCarryWeight { get; private set; }
        public static ConfigEntry<bool> EncumberanceAltEnable { get; private set; }
        public static ConfigEntry<float> EncumberanceAltMinSpeed { get; private set; }
        public static ConfigEntry<float> EncumberanceAltMaxSpeed { get; private set; }
        public static ConfigEntry<float> EncumberanceAltThreshold { get; private set; }
        public static ConfigEntry<float> EncumberedDrain { get; private set; }

        //Utility
        public static ConfigEntry<int> NexusID { get; private set; }
        public static ConfigEntry<bool> BaseHealthStaminaEnable { get; private set; }

        public static void Bind(ConfigFile config)
        {
            //Stamina
            BaseStamina = config.Bind("Stamina", "BaseStamina", 75f, new ConfigDescription("Base player stamina; vanilla: 75", new AcceptableValueRange<float>(10f, 150f)));
            StaminaMinimum = config.Bind("Stamina", "StaminaMinimum", -50f, new ConfigDescription("Base stamina minimum, stamina is not usable in negative values but can be reached by using more stamina than you have; vanilla: 0", new AcceptableValueRange<float>(-150f, 0f)));
            StaminaRegen = config.Bind("Stamina", "StaminaRegen", 12f, new ConfigDescription("Base stamina regen; vanilla: 6", new AcceptableValueRange<float>(0f, 30f)));
            StaminaDelay = config.Bind("Stamina", "StaminaDelay", 1.75f, new ConfigDescription("Base stamina regen delay; vanilla: 1", new AcceptableValueRange<float>(0f, 20f)));
            DodgeStamina = config.Bind("Stamina", "DodgeStamina", 12f, new ConfigDescription("Base dodge stamina usage; vanilla: 10", new AcceptableValueRange<float>(0f, 40f)));
            JumpStamina = config.Bind("Stamina", "JumpStamina", 5f, new ConfigDescription("Base jump stamina usage; vanilla: 10", new AcceptableValueRange<float>(0f, 40f)));
            BuildUseStamina = config.Bind("Stamina", "BuildUseStamina", false, new ConfigDescription("Enable or disable stamina usage when building, cultivating or uh.. hoeing"));
            StaminaUseMultiplier = config.Bind("Stamina", "StaminaUseMultiplier", 1.5f, new ConfigDescription("Final stamina usage multiplier for any action; vanilla: 1", new AcceptableValueRange<float>(0f, 10f)));


            //Player
            BaseHealth = config.Bind("Player", "BaseHealth", 50f, new ConfigDescription("Base player health; vanilla: 25", new AcceptableValueRange<float>(1f, 150f)));
            Acceleration = config.Bind("Player", "Acceleration", 0.25f, new ConfigDescription("Base player movement acceleration; vanilla: 1", new AcceptableValueRange<float>(0.01f, 5f)));
            ParryTime = config.Bind("Player", "ParryTime", 0.13f, new ConfigDescription("Base parry time in seconds; vanilla: 0.25", new AcceptableValueRange<float>(0f, 5f)));
            ParryRefundEnable = config.Bind("Player", "ParryRefundEnable", true, new ConfigDescription("Enable or disable parry stamina refunds"));
            ParryRefundMultiplier = config.Bind("Player", "ParryRefundMultiplier", 1f, new ConfigDescription("Final stamina refund multiplier applied for a successful parry", new AcceptableValueRange<float>(0f, 10f)));
            WeaponWeightStaminaScalingEnable = config.Bind("Player", "WeaponWeightStaminaScalingEnable", true, new ConfigDescription("Enable or disable stamina usage increase based on weapon weight (note that this applies before stamina use multiplier)"));

            //Food
            FoodHealthMin = config.Bind("Food", "FoodHealthMin", 10f, new ConfigDescription("Minimum health a food item can give after multipliers", new AcceptableValueRange<float>(0f, 100f)));
            FoodHealthMultiplier = config.Bind("Food", "FoodHealthMultiplier", 0.8f, new ConfigDescription("Multiplier applied to food health", new AcceptableValueRange<float>(0f, 10f)));
            FoodStaminaMin = config.Bind("Food", "FoodStaminaMin", 20f, new ConfigDescription("Minimum stamina a food item can give after multipliers", new AcceptableValueRange<float>(0f, 100f)));
            FoodStaminaMultiplier = config.Bind("Food", "FoodStaminaMultiplier", 0.6f, new ConfigDescription("Multiplier applied to food stamina", new AcceptableValueRange<float>(0f, 10f)));
            FoodBurnTimeMultiplier = config.Bind("Food", "FoodBurnTimeMultiplier", 1.5f, new ConfigDescription("Multiplier applied to food burn time; vanilla: 1", new AcceptableValueRange<float>(0.01f, 10f)));

            //Exhaustion
            ExhaustionEnable = config.Bind("Exhaustion", "ExhaustionEnable", true, new ConfigDescription("Enable or disable exhaustion sprinting system, player will enter 'pushing' state when sprinting at the configured pushing threshold, and 'exhausted' state at the configured exhaustion threshold"));
            ExhaustionThreshold = config.Bind("Exhaustion", "ExhaustionThreshold", -40f, new ConfigDescription("Stamina threshold to activate exhaustion debuff", new AcceptableValueRange<float>(-150f, 0f)));
            ExhaustionRecoveryThreshold = config.Bind("Exhaustion", "ExhaustionRecoveryThreshold", 0.8f, new ConfigDescription("Stamina percentage (where 0.0 = 0%, 1.0 = 100%) threshold to deactivate exhaustion debuff", new AcceptableValueRange<float>(0f, 1f)));
            ExhaustionSpeedMultiplier = config.Bind("Exhaustion", "ExhaustionSpeedModifier", 0.25f, new ConfigDescription("Movement speed multiplier applied when exhausted (note this stacks with the pushing speed modifier)", new AcceptableValueRange<float>(0f, 1f)));
            PushingThreshold = config.Bind("Exhaustion", "PushingThreshold", 0f, new ConfigDescription("Stamina threshold to apply pushing debuff (speed modifier and sweating effect) at", new AcceptableValueRange<float>(-150f, 100f)));
            PushingSpeedMultiplier = config.Bind("Exhaustion", "PushingSpeedModifier", 0.85f, new ConfigDescription("Movement speed multiplier applied when pushing", new AcceptableValueRange<float>(0f, 1f)));
            PushingWarms = config.Bind("Exhaustion", "PushingWarms", true, new ConfigDescription("Enable or disable the pushing debuff 'warming' the player (applies 'warm' debuff; reduces time remaining on 'wet' debuff and temporarily removes 'cold' debuff)"));
            PushingWarmRate = config.Bind("Exhaustion", "PushingWarmRate", 4f, new ConfigDescription("The rate at which pushing warms the player, reducing time on the 'wet' debuff", new AcceptableValueRange<float>(0f, 20f)));
            PushingWarmTimeRate = config.Bind("Exhaustion", "PushingWarmTimeRate", 5f, new ConfigDescription("The rate at which more time is generated for the 'warm' debuff", new AcceptableValueRange<float>(0f, 20f)));
            PushingWarmInitialTime = config.Bind("Exhaustion", "PushingWarmInitialTime", 2f, new ConfigDescription("The initial amount of time the player gains the 'warm' debuff for", new AcceptableValueRange<float>(1f, 30f)));

            //Encumberance
            BaseCarryWeight = config.Bind("Encumberance", "BaseCarryWeight", 200f, new ConfigDescription("Base carry weight; vanilla: 300", new AcceptableValueRange<float>(0f, 1000f)));
            EncumberanceAltEnable = config.Bind("Encumberance", "EncumberanceAltEnable", true, new ConfigDescription("Enable or disable alternative encumberance, scales movement speed on carry weight"));
            EncumberanceAltMinSpeed = config.Bind("Encumberance", "EncumberanceAltMinSpeed", 0.6f, new ConfigDescription("The minimum speed multiplier applied when reaching the alt encumberance threshold", new AcceptableValueRange<float>(0.6f, 1f)));
            EncumberanceAltMaxSpeed = config.Bind("Encumberance", "EncumberanceAltMaxSpeed", 1.1f, new ConfigDescription("The maximum speed multiplier applied when unencumbered", new AcceptableValueRange<float>(0.6f, 2f)));
            EncumberanceAltThreshold = config.Bind("Encumberance", "EncumberanceAltThreshold", 400f, new ConfigDescription("The carry weight threshold at which to apply the encumbered status", new AcceptableValueRange<float>(0f, 1000f)));
            EncumberedDrain = config.Bind("Encumberance", "EncumberanceDrain", 2f, new ConfigDescription("Base stamina drain when encumbered, applies regardless of alternative encumberance; vanilla: 10", new AcceptableValueRange<float>(0f, 20f)));

            //NexusID
            NexusID = config.Bind("Utility", "NexusID", 297, "Nexus Mod ID for updates, do not change");
            BaseHealthStaminaEnable = config.Bind("Utility", "BaseHealthStaminaEnable", true, "Enables or disables base health and stamina adjustments (note other mods may disable this functionality by nature). " +
                "The method of modification used is somewhat fragile and could break with any update to the game, or not play ball with another mod that touches the same values, as such " +
                "I'm giving you the option to disable the patching process here should anything break.");

            var sanity = IsConfigSane();
            if (!sanity.sane)
                UnityEngine.Debug.LogError($"Configuration invalid: {sanity.reason}");
        }

        private static (bool sane, string reason) IsConfigSane()
        {
            var reason = new StringBuilder();

            //Exhaustion
            if (ExhaustionThreshold.Value > BaseStamina.Value)
            {
                ExhaustionThreshold.Value = (float)ExhaustionThreshold.DefaultValue;
                AppendReason(reason, nameof(ExhaustionThreshold), $"may not be greater than {nameof(BaseStamina)}");
            }

            if (ExhaustionRecoveryThreshold.Value < ExhaustionThreshold.Value)
            {
                ExhaustionRecoveryThreshold.Value = (float)ExhaustionRecoveryThreshold.DefaultValue;
                AppendReason(reason, nameof(ExhaustionRecoveryThreshold), $"may not be less than {nameof(ExhaustionThreshold)}");
            }

            if (PushingThreshold.Value < ExhaustionThreshold.Value || PushingThreshold.Value > BaseStamina.Value)
            {
                PushingThreshold.Value = (float)PushingThreshold.DefaultValue;
                AppendReason(reason, nameof(PushingThreshold), $"may not be less than {nameof(ExhaustionThreshold)} or greater than {nameof(BaseStamina)}");
            }

            if (PushingSpeedMultiplier.Value < ExhaustionSpeedMultiplier.Value)
            {
                PushingSpeedMultiplier.Value = (float)PushingSpeedMultiplier.DefaultValue;
                AppendReason(reason, nameof(PushingSpeedMultiplier), $"may not be less than {nameof(ExhaustionSpeedMultiplier)}");
            }

            if (EncumberanceAltMaxSpeed.Value < EncumberanceAltMinSpeed.Value)
            {
                EncumberanceAltMaxSpeed.Value = (float)EncumberanceAltMaxSpeed.DefaultValue;
                AppendReason(reason, nameof(EncumberanceAltMaxSpeed), $"may not be less than {nameof(EncumberanceAltMinSpeed)}");
            }

            if (EncumberanceAltThreshold.Value < BaseCarryWeight.Value)
            {
                EncumberanceAltThreshold.Value = (float)EncumberanceAltThreshold.DefaultValue;
                AppendReason(reason, nameof(EncumberanceAltThreshold), $"may not be less than {nameof(BaseCarryWeight)}");
            }

            return (true, "");

            void AppendReason(StringBuilder builder, string propName, string additional = null)
            {
                if (builder.Length > 0)
                    builder.Append(", ");

                builder.Append($"{propName} not valid");

                if (!string.IsNullOrEmpty(additional))
                    builder.Append($": {additional}");
            }
        }
    }
}
