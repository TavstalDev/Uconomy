using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace fr34kyn01535.Uconomy
{
    /// <summary>
    /// Main class for the Uconomy plugin, handling player balances, salaries, and related events.
    /// </summary>
    public class Uconomy : RocketPlugin<UconomyConfiguration>
    {
        /// <summary>
        /// Manages database operations for player balances.
        /// </summary>
        public DatabaseManager Database;
        /// <summary>
        /// Singleton instance of the Uconomy class.
        /// </summary>
        public static Uconomy Instance;
        /// <summary>
        /// Tracks the last update time for salary processing.
        /// </summary>
        private DateTime _lastUpdate;
        /// <summary>
        /// Stores the next salary payment time for each player.
        /// </summary>
        internal Dictionary<string, DateTime> SalaryIntervals;

        #region Events
        /// <summary>
        /// Delegate for handling player balance updates.
        /// </summary>
        /// <param name="player">The player whose balance was updated.</param>
        /// <param name="amt">The amount by which the balance was updated.</param>
        public delegate void PlayerBalanceUpdate(UnturnedPlayer player, decimal amt);
        /// <summary>
        /// Event triggered when a player's balance is updated.
        /// </summary>
        public event PlayerBalanceUpdate OnBalanceUpdate;
        /// <summary>
        /// Delegate for handling player balance checks.
        /// </summary>
        /// <param name="player">The player whose balance was checked.</param>
        /// <param name="balance">The current balance of the player.</param>
        public delegate void PlayerBalanceCheck(UnturnedPlayer player, decimal balance);
        /// <summary>
        /// Event triggered when a player's balance is checked.
        /// </summary>
        public event PlayerBalanceCheck OnBalanceCheck;
        /// <summary>
        /// Delegate for handling player payments.
        /// </summary>
        /// <param name="sender">The player sending the payment.</param>
        /// <param name="receiver">The player receiving the payment.</param>
        /// <param name="amt">The amount being paid.</param>
        public delegate void PlayerPay(UnturnedPlayer sender, UnturnedPlayer receiver, decimal amt);
        /// <summary>
        /// Event triggered when a player makes a payment to another player.
        /// </summary>
        public event PlayerPay OnPlayerPay;

        #endregion

        /// <summary>
        /// Called when the plugin is loaded. Initializes necessary components.
        /// </summary>
        protected override void Load()
        {
            Instance = this;
            _lastUpdate = DateTime.Now;
            SalaryIntervals = new Dictionary<string, DateTime>();
            Database = new DatabaseManager();
            UnturnedEventHandler.Attach();
        }

        /// <summary>
        /// Called when the plugin is unloaded. Cleans up resources.
        /// </summary>
        protected override void Unload()
        {
            UnturnedEventHandler.Dettach();
        }

        /// <summary>
        /// Triggers the OnPlayerPay event when a payment is made.
        /// </summary>
        /// <param name="sender">The player sending the payment.</param>
        /// <param name="receiver">The player receiving the payment.</param>
        /// <param name="amt">The amount being paid.</param>
        internal void HasBeenPayed(UnturnedPlayer sender, UnturnedPlayer receiver, decimal amt)
        {
            if (OnPlayerPay != null)
                OnPlayerPay(sender, receiver, amt);
        }

        /// <summary>
        /// Triggers the OnBalanceUpdate event when a player's balance is updated.
        /// </summary>
        /// <param name="SteamID">The SteamID of the player whose balance was updated.</param>
        /// <param name="amt">The amount by which the balance was updated.</param>
        internal void BalanceUpdated(string SteamID, decimal amt)
        {
            if (OnBalanceUpdate != null)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(SteamID)));
                OnBalanceUpdate(player, amt);
            }
        }

        /// <summary>
        /// Triggers the OnBalanceCheck event when a player's balance is checked.
        /// </summary>
        /// <param name="SteamID">The SteamID of the player whose balance was checked.</param>
        /// <param name="balance">The current balance of the player.</param>
        internal void OnBalanceChecked(string SteamID, decimal balance)
        {
            if (OnBalanceCheck != null)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(SteamID)));
                OnBalanceCheck(player, balance);
            }
        }

        /// <summary>
        /// Updates the salary intervals and processes salary payments for players.
        /// </summary>
        private void Update()
        {
            if (_lastUpdate > DateTime.Now || !Configuration.Instance.EnableSalaries)
                return;


            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                if (!SalaryIntervals.TryGetValue(unturnedPlayer.Id, out DateTime salaryDate))
                    continue;

                if (salaryDate > DateTime.Now)
                    continue;

                SalaryIntervals[unturnedPlayer.Id] = DateTime.Now.AddSeconds(Configuration.Instance.SalaryInterval);

                Permission perm = R.Permissions.GetPermissions(unturnedPlayer).FirstOrDefault(x => x.Name.ToLower().StartsWith("uconomy.salary."));
                if (perm == null)
                    continue;

                if (!decimal.TryParse(perm.Name.Replace("uconomy.salary.", ""), out decimal salary))
                    continue;

                Database.IncreaseBalance(unturnedPlayer.Id, salary);
                ChatManager.serverSendMessage(Instance.Translate("salary_msg", GetPrefix(), salary, Configuration.Instance.MoneyName), 
                    Color.green, 
                    null, 
                    steamPlayer, 
                    EChatMode.GLOBAL, 
                    null, 
                    true);
            }


            _lastUpdate = DateTime.Now.AddMinutes(1);
        }

        public string GetPrefix() => Translate("prefix") ?? "";

        /// <summary>
        /// Provides the default translations for the plugin.
        /// </summary>
        public override TranslationList DefaultTranslations =>
            new TranslationList {
                {"prefix", "<color=#FFFF55>[</color><color=#FFAA00>Uconomy</color><color=#FFFF55>]</color>" },
                {"command_balance_show", "{0} <color=#55FF55>Your current balance is: {1} {2}</color>"},
                {"command_balance_show_other", "{0} <color=#55FF55>{3}'s current balance is: {1} {2}</color>"},
                {"command_balance_other_forbidden", "{0} <color=#FF5555>You do not have permission to do that.</color>" },
                {"command_pay_error_invalid", "{0} <color=#FF5555>Invalid arguments. Usage: /pay {1}</color>"},
                {"command_pay_error_pay_self", "{0} <color=#FF5555>You cant pay yourself.</color>"},
                {"command_pay_error_invalid_amount", "{0} <color=#FF5555>Invalid amount.</color>"},
                {"command_pay_error_cant_afford", "{0} <color=#FF5555>Your balance does not allow this payment.</color>"},
                {"command_pay_error_player_not_found", "{0} <color=#FF5555>Failed to find player.</color>"},
                {"command_pay_private", "{0} <color=#55FF55>You paid {1} {2} {3}</color>"},
                {"command_pay_console", "{0} <color=#55FF55>You received a payment of {1} {2}</color>"},
                {"command_pay_other_private", "{0} <color=#55FF55>You received a payment of {1} {2} from {3}</color>"},
                {"command_adminpay_error_invalid", "{0} <color=#FF5555>Invalid arguments. Usage: /adminpay {1}</color>"},
                {"command_setbalance_error_invalid", "{0} <color=#FF5555>Invalid arguments. Usage: /setbalance {1}</color>"},
                {"command_setbalance_error_generic", "{0} <color=#FF5555>Failed to change the balance of the player.</color>"},
                {"command_setbalance_success", "{0} <color=#55FF55>You have successfully set the balance of {3} to {1}{2}.</color>"},
                {"command_setbalance_success_other", "{0} <color=#55FF55>Your balance has been set to {1}{2}.</color>"},
                {"command_exchange_error_invalid", "{0} <color=#FF5555>Invalid arguments. Usage: /exchange {1}</color>" },
                {"command_exchange_error_cant_afford", "{0} <color=#FF5555>Your balance does not allow this exchange.</color>" },
                {"command_exchange_success", "{0} <color=#55FF55>The exchange was successful</color>" },
                {"salary_msg", "{0} <color=#55FF55>You have received your salary of {1} {2}</color>" },
                {"death_penalty_msg", "{0} <color=#55FF55>{1} {2} was deducted from your balance as death penalty.</color>" },
                {"kill_reward_msg", "{0} <color=#55FF55>{1} {2} was added to your balance as kill reward.</color>" }
            };
    }
}
