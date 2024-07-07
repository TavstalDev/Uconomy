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
using Tavstal.TLibrary.Utils;

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
        /// Translates a message key into a formatted string with a prefix.
        /// </summary>
        /// <param name="translationKey">The key of the translation to use.</param>
        /// <param name="args">Arguments to format into the translation.</param>
        /// <returns>A formatted string with the translation and prefix.</returns>
        public string TranslateRich(string translationKey, params string[] args)
        {
            return FormatHelper.FormatTextV2(DefaultTranslations.Translate("prefix") + " " + DefaultTranslations.Translate(translationKey, args));
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

        // <summary>
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
                ChatHelper.SendChatMessage(steamPlayer, "salary_msg", salary, Configuration.Instance.MoneyName);
            }


            _lastUpdate = DateTime.Now.AddMinutes(1);
        }

        /// <summary>
        /// Provides the default translations for the plugin.
        /// </summary>
        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                    {"prefix", "&e[&6Uconomy&e]" },
                    {"command_balance_show", "&aYour current balance is: &c{0} {1}"},
                    {"command_balance_show_other", "&a{2}'s current balance is: &c{0} {1}"},
                    {"command_balance_other_forbidden", "&cYou do not have permission to do that." },
                    {"command_pay_invalid", "&cInvalid arguments. Usage: /pay {0}"},
                    {"command_pay_error_pay_self", "&cYou cant pay yourself"},
                    {"command_pay_error_invalid_amount", "&cInvalid amount"},
                    {"command_pay_error_cant_afford", "&cYour balance does not allow this payment"},
                    {"command_pay_error_player_not_found", "&cFailed to find player"},
                    {"command_pay_private", "&aYou paid {0} {1} {2}"},
                    {"command_pay_console", "&aYou received a payment of {0} {1} "},
                    {"command_pay_other_private", "&aYou received a payment of {0} {1} from {2}"},
                    {"command_exchange_invalid", "&cInvalid arguments. Usage: /exchange {0}" },
                    {"command_exchange_cant_afford", "&cYour balance does not allow this exchange." },
                    {"command_exchange_success", "&aThe exchange was successful" },
                    {"salary_msg", "&aYou have received your salary of {0} {1}" },
                    {"death_penalty_msg", "&a{0} {1} was deducted from your balance as death penalty." },
                    {"kill_reward_msg", "&a{0} {1} was added to your balance as kill reward." }
                }; 
            }
        }
    }
}
