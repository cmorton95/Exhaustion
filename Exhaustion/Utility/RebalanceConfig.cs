using BepInEx.Configuration;

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
        public static ConfigEntry<float> PushingSpeedMultiplier { get; private set; }
        public static ConfigEntry<bool> PushingWarms { get; private set; }
        public static ConfigEntry<float> PushingWarmRate { get; private set; }
        public static float STAM_EXH_ACCEL => 0.02f;

        //Encumberance
        public static ConfigEntry<float> BaseCarryWeight { get; private set; }
        public static ConfigEntry<bool> EncumberanceAltEnable { get; private set; }
        public static ConfigEntry<float> EncumberanceAltMinSpeed { get; private set; }
        public static ConfigEntry<float> EncumberanceAltMaxSpeed { get; private set; }
        public static ConfigEntry<float> EncumberanceAltThreshold { get; private set; }
        public static ConfigEntry<float> EncumberedDrain { get; private set; }

        //Nexus update support
        public static ConfigEntry<int> NexusID { get; private set; }

        public static void Bind(ConfigFile config)
        {
            //Stamina
            BaseStamina = config.Bind("Stamina", "BaseStamina", 75f, "Base player stamina; vanilla: 75");
            StaminaMinimum = config.Bind("Stamina", "StaminaMinimum", -50f, "Base stamina minimum, stamina is not usable in negative values but can be reached by using more stamina than you have; vanilla: 0");
            StaminaRegen = config.Bind("Stamina", "StaminaRegen", 12f, "Base stamina regen; vanilla: 6");
            StaminaDelay = config.Bind("Stamina", "StaminaDelay", 1.75f, "Base stamina regen delay; vanilla: 1");
            DodgeStamina = config.Bind("Stamina", "DodgeStamina", 12f, "Base dodge stamina usage; vanilla: 10");
            JumpStamina = config.Bind("Stamina", "JumpStamina", 5f, "Base jump stamina usage; vanilla: 10");
            BuildUseStamina = config.Bind("Stamina", "BuildUseStamina", false, "Enable or disable stamina usage when building, cultivating or uh.. hoeing");
            StaminaUseMultiplier = config.Bind("Stamina", "StaminaUseMultiplier", 1.5f, "Final stamina usage multiplier for any action; vanilla: 1");


            //Player
            BaseHealth = config.Bind("Player", "BaseHealth", 50f, "Base player health; vanilla: 25");
            Acceleration = config.Bind("Player", "Acceleration", 0.25f, "Base player movement acceleration; vanilla: 1");
            ParryTime = config.Bind("Player", "ParryTime", 0.13f, "Base parry time in seconds; vanilla: 0.25");
            ParryRefundEnable = config.Bind("Player", "ParryRefundEnable", true, "Enable or disable parry stamina refunds");
            ParryRefundMultiplier = config.Bind("Player", "ParryRefundMultiplier", 1f, "Final stamina refund multiplier applied for a successful parry");
            WeaponWeightStaminaScalingEnable = config.Bind("Player", "WeaponWeightStaminaScalingEnable", true, "Enable or disable stamina usage increase based on weapon weight (note that this applies before stamina use multiplier)");

            //Food
            FoodHealthMin = config.Bind("Food", "FoodHealthMin", 10f, "Minimum health a food item can give after multipliers");
            FoodHealthMultiplier = config.Bind("Food", "FoodHealthMultiplier", 0.8f, "Multiplier applied to food health");
            FoodStaminaMin = config.Bind("Food", "FoodStaminaMin", 20f, "Minimum stamina a food item can give after multipliers");
            FoodStaminaMultiplier = config.Bind("Food", "FoodStaminaMultiplier", 0.6f, "Multiplier applied to food stamina");
            FoodBurnTimeMultiplier = config.Bind("Food", "FoodBurnTimeMultiplier", 1.5f, "Multiplier applied to food burn time; vanilla: 1");

            //Exhaustion
            ExhaustionEnable = config.Bind("Exhaustion", "ExhaustionEnable", true, "Enable or disable exhaustion sprinting system, player will enter 'pushing' state when sprinting below 0 stamina, and 'exhausted' state at the configured exhaustion threshold");
            ExhaustionThreshold = config.Bind("Exhaustion", "ExhaustionThreshold", -40f, "Stamina threshold to activate exhaustion debuff");
            ExhaustionRecoveryThreshold = config.Bind("Exhaustion", "ExhaustionRecoveryThreshold", 0.8f, "Stamina percentage (where 0.0 = 0%, 1.0 = 100%) threshold to deactivate exhaustion debuff");
            ExhaustionSpeedMultiplier = config.Bind("Exhaustion", "ExhaustionSpeedModifier", 0.25f, "Movement speed multiplier applied when exhausted (note this stacks with the pushing speed modifier)");
            PushingSpeedMultiplier = config.Bind("Exhaustion", "PushingSpeedModifier", 0.85f, "Movement speed multiplier applied when pushing (sprinting below 0 stamina)");
            PushingWarms = config.Bind("Exhaustion", "PushingWarms", true, "Enable or disable the pushing debuff 'warming' the player (reduces time remaining on 'Wet' debuff and temporarily removes 'Cold' debuff)");
            PushingWarmRate = config.Bind("Exhaustion", "PushingWarmRate", 4f, "The rate at which pushing warms the player");

            //Encumberance
            BaseCarryWeight = config.Bind("Encumberance", "BaseCarryWeight", 300f, "Base carry weight; vanilla: 300");
            EncumberanceAltEnable = config.Bind("Encumberance", "EncumberanceAltEnable", true, "Enable or disable alternative encumberance, scales movement speed on carry weight");
            EncumberanceAltMinSpeed = config.Bind("Encumberance", "EncumberanceAltMinSpeed", 0.6f, "The minimum speed multiplier applied when reaching the alt encumberance threshold");
            EncumberanceAltMaxSpeed = config.Bind("Encumberance", "EncumberanceAltMaxSpeed", 1.1f, "The maximum speed multiplier applied when unencumbered");
            EncumberanceAltThreshold = config.Bind("Encumberance", "EncumberanceAltThreshold", 600f, "The carry weight threshold at which to apply the encumbered status");
            EncumberedDrain = config.Bind("Encumberance", "EncumberanceDrain", 2f, "Base stamina drain when encumbered, applies regardless of alternative encumberance; vanilla: 10");

            //NexusID
            NexusID = config.Bind("Utility", "NexusID", 297, "Nexus Mod ID for updates, do not change");
        }
    }
}
