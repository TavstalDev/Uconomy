using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TLibrary.Utils;

namespace fr34kyn01535.Uconomy.Commands
{
    internal class CommandAdminPay
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "adminpay";
        public string Help => "Pays a specific player money.";
        public string Syntax => "[player] [amount]";
        public List<string> Aliases => new List<string> { "apay" };
        public List<string> Permissions => new List<string> { "uconomy.adminpay", "uconomy.commands.adminpay" };

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length != 2)
            {
                ChatHelper.SendCommandReply(caller, "command_pay_invalid");
                return;
            }

            UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
            if (otherPlayer != null)
            {
                if (caller == otherPlayer)
                {
                    ChatHelper.SendCommandReply(caller, "command_pay_error_pay_self");
                    return;
                }

                decimal amount = 0;
                if (!decimal.TryParse(command[1], out amount) || amount <= 0)
                {
                    ChatHelper.SendCommandReply(caller, "command_pay_error_invalid_amount");
                    return;
                }

                Uconomy.Instance.Database.IncreaseBalance(otherPlayer.Id, amount);
                ChatHelper.SendCommandReply(caller, "command_pay_private", otherPlayer.CharacterName, amount, Uconomy.Instance.Configuration.Instance.MoneyName);
                ChatHelper.SendCommandReply(otherPlayer, "command_pay_console", amount, Uconomy.Instance.Configuration.Instance.MoneyName);
            }
            else
            {
                ChatHelper.SendCommandReply(caller, "command_pay_error_player_not_found");
            }
        }
    }
}
