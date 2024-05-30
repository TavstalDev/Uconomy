using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Tavstal.TLibrary.Utils;

namespace fr34kyn01535.Uconomy
{
    public class Uconomy : RocketPlugin<UconomyConfiguration>
    {
        public DatabaseManager Database;
        public static Uconomy Instance;
        private DateTime _lastUpdate;
        internal Dictionary<string, DateTime> SalaryIntervals;

        protected override void Load()
        {
            Instance = this;
            _lastUpdate = DateTime.Now;
            SalaryIntervals = new Dictionary<string, DateTime>();
            Database = new DatabaseManager();
            UnturnedEventHandler.Attach();
        }

        protected override void Unload()
        {
            UnturnedEventHandler.Dettach();
        }

        public delegate void PlayerBalanceUpdate(UnturnedPlayer player, decimal amt);
        public event PlayerBalanceUpdate OnBalanceUpdate;
        public delegate void PlayerBalanceCheck(UnturnedPlayer player, decimal balance);
        public event PlayerBalanceCheck OnBalanceCheck;
        public delegate void PlayerPay(UnturnedPlayer sender, UnturnedPlayer receiver, decimal amt);
        public event PlayerPay OnPlayerPay;

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

        public string TranslateRich(string translationKey, params string[] args)
        {
            return FormatHelper.FormatTextV2(DefaultTranslations.Translate("prefix") + " " + DefaultTranslations.Translate(translationKey, args));
        }

        internal void HasBeenPayed(UnturnedPlayer sender, UnturnedPlayer receiver, decimal amt)
        {
            if (OnPlayerPay != null)
                OnPlayerPay(sender, receiver, amt);
        }

        internal void BalanceUpdated(string SteamID, decimal amt)
        {
            if (OnBalanceUpdate != null)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(SteamID)));
                OnBalanceUpdate(player, amt);
            }
        }

        internal void OnBalanceChecked(string SteamID, decimal balance)
        {
            if (OnBalanceCheck != null)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(SteamID)));
                OnBalanceCheck(player, balance);
            }
        }

        
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
    }
}
