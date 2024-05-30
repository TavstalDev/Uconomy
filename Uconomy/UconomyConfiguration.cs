using fr34kyn01535.Uconomy.Models;
using Rocket.API;
using SDG.Unturned;
using System.Collections.Generic;

namespace fr34kyn01535.Uconomy
{
    public class UconomyConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public int DatabasePort;
        public decimal InitialBalance;
        public string MoneyName;
        public bool EnableSalaries;
        public int SalaryInterval;
        public List<KillReward> KillRewards;
        public List<DeathPenalty> DeathPenalties;

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
                new KillReward("KILLS_ZOMBIES_NORMAL", 5, true),
                new KillReward("KILLS_ZOMBIES_MEGA", 10, true),
                new KillReward("KILLS_PLAYERS", 15, true),
                new KillReward("HEADSHOTS", 20, true),
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
