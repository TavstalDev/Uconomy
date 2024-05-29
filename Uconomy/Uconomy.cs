using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using Tavstal.TLibrary.Utils;

namespace fr34kyn01535.Uconomy
{
    public class Uconomy : RocketPlugin<UconomyConfiguration>
    {
        public DatabaseManager Database;
        public static Uconomy Instance;

        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();
            U.Events.OnPlayerConnected+=Events_OnPlayerConnected;
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
                    {"command_pay_invalid", "&cInvalid arguments"},
                    {"command_pay_error_pay_self", "&cYou cant pay yourself"},
                    {"command_pay_error_invalid_amount", "&cInvalid amount"},
                    {"command_pay_error_cant_afford", "&cYour balance does not allow this payment"},
                    {"command_pay_error_player_not_found", "&cFailed to find player"},
                    {"command_pay_private", "&aYou paid {0} {1} {2}"},
                    {"command_pay_console", "&aYou received a payment of {0} {1} "},
                    {"command_pay_other_private", "&aYou received a payment of {0} {1} from {2}"},
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

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            //setup account
            Database.CheckSetupAccount(player.CSteamID);
        }
    }
}
