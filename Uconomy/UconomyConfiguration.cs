using fr34kyn01535.Uconomy.Models;
using Rocket.API;
using SDG.Unturned;
using System.Collections.Generic;

namespace fr34kyn01535.Uconomy
{
    /// <summary>
    /// Represents the configuration settings for the Uconomy plugin,
    /// including database connection, economy defaults, salaries,
    /// kill rewards, and death penalties.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UconomyConfiguration : IRocketPluginConfiguration
    {
        /// <summary>
        /// The MySQL server address.
        /// </summary>
        public string DatabaseAddress;

        /// <summary>
        /// The MySQL username for authentication.
        /// </summary>
        public string DatabaseUsername;

        /// <summary>
        /// The MySQL password for authentication.
        /// </summary>
        public string DatabasePassword;

        /// <summary>
        /// The MySQL database name.
        /// </summary>
        public string DatabaseName;

        /// <summary>
        /// The name of the table storing player balances.
        /// </summary>
        public string DatabaseTableName;

        /// <summary>
        /// The MySQL server port number.
        /// </summary>
        public int DatabasePort;

        /// <summary>
        /// The starting balance given to new players.
        /// </summary>
        public decimal InitialBalance;

        /// <summary>
        /// The display name of the currency used in messages.
        /// </summary>
        public string MoneyName;

        /// <summary>
        /// Whether periodic salary payments are enabled.
        /// </summary>
        public bool EnableSalaries;

        /// <summary>
        /// The interval in seconds between salary payments.
        /// </summary>
        public int SalaryInterval;

        /// <summary>
        /// The list of configured kill reward rules.
        /// </summary>
        public List<KillReward> KillRewards;

        /// <summary>
        /// The list of configured death penalty rules.
        /// </summary>
        public List<DeathPenalty> DeathPenalties;

        /// <summary>
        /// Loads the default configuration values,
        /// including database settings, economy defaults,
        /// kill rewards, and death penalties.
        /// </summary>
        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabaseTableName = "uconomy";
            DatabasePort = 3306;
            InitialBalance = 30;
            MoneyName = "Credits";
            EnableSalaries = true;
            SalaryInterval = 1800;
            KillRewards = new List<KillReward>()
            {
                new KillReward(  EPlayerStat.KILLS_ZOMBIES_NORMAL, 5, true),
                new KillReward(EPlayerStat.KILLS_ZOMBIES_MEGA, 10, true),
                new KillReward(EPlayerStat.KILLS_PLAYERS, 15, true),
                new KillReward(EPlayerStat.HEADSHOTS, 20, true),
            };
            DeathPenalties = new List<DeathPenalty>()
            {
                new DeathPenalty(EDeathCause.ACID, true, 50),
                new DeathPenalty(EDeathCause.ANIMAL, true, 50),
                new DeathPenalty(EDeathCause.ARENA, true, 50),
                new DeathPenalty(EDeathCause.BLEEDING, true, 50),
                new DeathPenalty(EDeathCause.BONES, true, 50),
                new DeathPenalty(EDeathCause.BOULDER, true, 50),
                new DeathPenalty(EDeathCause.BREATH, true, 50),
                new DeathPenalty(EDeathCause.BURNER, true, 50),
                new DeathPenalty(EDeathCause.BURNING, true, 50),
                new DeathPenalty(EDeathCause.CHARGE, true, 50),
                new DeathPenalty(EDeathCause.FOOD, true, 50),
                new DeathPenalty(EDeathCause.FREEZING, true, 50),
                new DeathPenalty(EDeathCause.GRENADE, true, 50),
                new DeathPenalty(EDeathCause.GUN, true, 50),
                new DeathPenalty(EDeathCause.INFECTION, true, 50),
                new DeathPenalty(EDeathCause.KILL, true, 50),
                new DeathPenalty(EDeathCause.LANDMINE, true, 50),
                new DeathPenalty(EDeathCause.MELEE, true, 50),
                new DeathPenalty(EDeathCause.MISSILE, true, 50),
                new DeathPenalty(EDeathCause.PUNCH, true, 50),
                new DeathPenalty(EDeathCause.ROADKILL, true, 50),
                new DeathPenalty(EDeathCause.SENTRY, true, 50),
                new DeathPenalty(EDeathCause.SHRED, true, 50),
                new DeathPenalty(EDeathCause.SPARK, true,50),
                new DeathPenalty(EDeathCause.SPIT, true,50),
                new DeathPenalty(EDeathCause.SPLASH, true,50),
                new DeathPenalty(EDeathCause.SUICIDE, true,50),
                new DeathPenalty(EDeathCause.VEHICLE, true,50),
                new DeathPenalty(EDeathCause.WATER, true,50),
                new DeathPenalty(EDeathCause.ZOMBIE, true,50),
            };
        }
    }
}
