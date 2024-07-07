using fr34kyn01535.Uconomy.Models;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using Tavstal.TLibrary.Utils;

namespace fr34kyn01535.Uconomy
{
    /// <summary>
    /// Class containing event handlers for player connections, disconnections, deaths, and stat updates in Unturned game.
    /// Provides methods to attach and detach event handlers, as well as handle player-related events such as setting up accounts, managing salaries, applying death penalties, and rewarding kills.
    /// </summary>
    internal class UnturnedEventHandler
    {
        private static bool _isAttached { get; set; }

        public static void Attach()
        {
            if (_isAttached)
                return;

            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;

            UnturnedPlayerEvents.OnPlayerDeath += Event_OnDeath;
            UnturnedPlayerEvents.OnPlayerUpdateStat += Event_OnStatUpdate;
        }

        public static void Dettach()
        {
            if (!_isAttached)
                return;

            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;

            UnturnedPlayerEvents.OnPlayerDeath -= Event_OnDeath;
            UnturnedPlayerEvents.OnPlayerUpdateStat -= Event_OnStatUpdate;
        }

        private static void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            //setup account
            Uconomy.Instance.Database.CheckSetupAccount(player.CSteamID);
            if (Uconomy.Instance.Configuration.Instance.EnableSalaries)
                Uconomy.Instance.SalaryIntervals.Add(player.Id, DateTime.Now.AddSeconds(Uconomy.Instance.Configuration.Instance.SalaryInterval));
        }

        private static void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            if (Uconomy.Instance.Configuration.Instance.EnableSalaries)
                Uconomy.Instance.SalaryIntervals.Remove(player.Id);
        }

        private static void Event_OnDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            // Death Fine
            // Paranoid 100
            if (!(player.Player.life.isDead || player.Health <= 0))
                return;

            DeathPenalty fine = Uconomy.Instance.Configuration.Instance.DeathPenalties.Find(x => x.Cause == cause && x.Enable && x.Fine > 0);
            if (fine == null)
                return;

            decimal amount = fine.Fine;
            Uconomy.Instance.Database.IncreaseBalance(player.Id, -amount);
            ChatHelper.SendChatMessage(player.SteamPlayer(), "death_penalty_msg", amount, Uconomy.Instance.Configuration.Instance.MoneyName);
        }

        private static void Event_OnStatUpdate(UnturnedPlayer player, EPlayerStat stat)
        {
            KillReward reward = Uconomy.Instance.Configuration.Instance.KillRewards.Find(x => x.EventName.ToLower().Equals(stat.ToString().ToLower()) && x.Enable && x.Reward > 0);
            if (reward == null)
                return;

            decimal amount = reward.Reward;
            Uconomy.Instance.Database.IncreaseBalance(player.Id, amount);
            ChatHelper.SendChatMessage(player.SteamPlayer(), "kill_reward_msg", amount, Uconomy.Instance.Configuration.Instance.MoneyName);
        }
    }
}
