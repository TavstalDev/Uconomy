﻿using Rocket;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using SDG;
using Steamworks;

namespace unturned.ROCKS.Uconomy
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


        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                {"command_balance_show","Your current balance is: {0} {1}"},
                {"command_pay_invalid","Invalid arguments"},
                {"command_pay_error_pay_self","You cant pay yourself"},
                {"command_pay_error_invalid_amount","Invalid amount"},
                {"command_pay_error_cant_afford","Your balance does not allow this payment"},
                {"command_pay_error_player_not_found","Failed to find player"},
                {"command_pay_private","You paid {0} {1} {2}"},
                {"command_pay_other_private","You received a payment of {0} {1} from {2}"},
                };
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
           //setup account
            Database.CheckSetupAccount(player.CSteamID);
        }
    }
}
