using fr34kyn01535.Uconomy.Models;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;

namespace fr34kyn01535.Uconomy
{
    /// <summary>
    /// Handles Unturned game events such as player connections,
    /// disconnections, deaths, and stat updates for Uconomy.
    /// Manages account setup, salary tracking, death penalties, and kill rewards.
    /// </summary>
    internal static class UnturnedEventHandler
    {
        private static bool _isAttached => false;

        /// <summary>
        /// Subscribes to all Unturned player events
        /// if not already attached.
        /// </summary>
        public static void Attach()
        {
            if (_isAttached)
                return;

            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;

            UnturnedPlayerEvents.OnPlayerDeath += Event_OnDeath;
            UnturnedPlayerEvents.OnPlayerUpdateStat += Event_OnStatUpdate;
        }

        /// <summary>
        /// Unsubscribes from all Unturned player events
        /// if currently attached.
        /// </summary>
        public static void Dettach()
        {
            if (!_isAttached)
                return;

            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;

            UnturnedPlayerEvents.OnPlayerDeath -= Event_OnDeath;
            UnturnedPlayerEvents.OnPlayerUpdateStat -= Event_OnStatUpdate;
        }

        /// <summary>
        /// Handles player connection by creating or verifying their account
        /// and initializing their salary interval if enabled.
        /// </summary>
        /// <param name="player">The player who connected.</param>
        private static void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            //setup account
            Uconomy.Instance.Database.CheckSetupAccount(player.CSteamID);
            if (Uconomy.Instance.Configuration.Instance.EnableSalaries)
                Uconomy.Instance.SalaryIntervals.Add(player.Id, DateTime.Now.AddSeconds(Uconomy.Instance.Configuration.Instance.SalaryInterval));
        }

        /// <summary>
        /// Handles player disconnection by removing their salary interval if enabled.
        /// </summary>
        /// <param name="player">The player who disconnected.</param>
        private static void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            if (Uconomy.Instance.Configuration.Instance.EnableSalaries)
                Uconomy.Instance.SalaryIntervals.Remove(player.Id);
        }

        /// <summary>
        /// Handles player death by applying the configured monetary penalty
        /// for the specific death cause if enabled.
        /// </summary>
        /// <param name="player">The player who died.</param>
        /// <param name="cause">The cause of death.</param>
        /// <param name="limb">The limb that was hit.</param>
        /// <param name="murderer">The Steam ID of the killer, if applicable.</param>
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
            ChatManager.serverSendMessage(Uconomy.Instance.Translate("death_penalty_msg", Uconomy.Instance.GetPrefix(), amount, Uconomy.Instance.Configuration.Instance.MoneyName), 
                Color.red, 
                null, 
                player.SteamPlayer(), 
                EChatMode.GLOBAL, 
                null, 
                true);
        }

        /// <summary>
        /// Handles player stat updates by rewarding the player
        /// with the configured monetary reward for the specific stat if enabled.
        /// </summary>
        /// <param name="player">The player whose stat was updated.</param>
        /// <param name="stat">The type of stat that was updated.</param>
        private static void Event_OnStatUpdate(UnturnedPlayer player, EPlayerStat stat)
        {
            KillReward reward = Uconomy.Instance.Configuration.Instance.KillRewards.Find(x => x.Event == stat && x.Enable && x.Reward > 0);
            if (reward == null)
                return;

            decimal amount = reward.Reward;
            Uconomy.Instance.Database.IncreaseBalance(player.Id, amount);
            ChatManager.serverSendMessage(Uconomy.Instance.Translate("kill_reward_msg", Uconomy.Instance.GetPrefix(), amount, Uconomy.Instance.Configuration.Instance.MoneyName), 
                Color.red, 
                null, 
                player.SteamPlayer(), 
                EChatMode.GLOBAL, 
                null, 
                true);
        }
    }
}
