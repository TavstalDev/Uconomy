using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TLibrary.Utils;

namespace fr34kyn01535.Uconomy
{
    public class CommandPay : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "pay";
        public string Help => "Pays a specific player money from your account";
        public string Syntax => "[player] [amount]";
        public List<string> Aliases => new List<string> { "pagar" };
        public List<string> Permissions => new List<string> { "uconomy.pay", "uconomy.commands.pay" };

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length != 2)
            {
                ChatHelper.SendCommandReply(caller, "command_pay_invalid");
                return;
            }

            UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
            if (otherPlayer !=null)
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

                if (caller is ConsolePlayer)
                {
                    Uconomy.Instance.Database.IncreaseBalance(otherPlayer.Id, amount);
                    ChatHelper.SendCommandReply(otherPlayer, "command_pay_console", amount, Uconomy.Instance.Configuration.Instance.MoneyName);
                }
                else
                {

                    decimal myBalance = Uconomy.Instance.Database.GetBalance(caller.Id);
                    if ((myBalance - amount) <= 0)
                    {
                        ChatHelper.SendCommandReply(caller, "command_pay_error_cant_afford");
                        return;
                    }
                    else
                    {
                        Uconomy.Instance.Database.IncreaseBalance(caller.Id, -amount);
                        ChatHelper.SendCommandReply(caller,"command_pay_private", otherPlayer.CharacterName, amount, Uconomy.Instance.Configuration.Instance.MoneyName);
                        Uconomy.Instance.Database.IncreaseBalance(otherPlayer.Id, amount);
                        ChatHelper.SendCommandReply(otherPlayer, "command_pay_other_private", amount, Uconomy.Instance.Configuration.Instance.MoneyName, caller.DisplayName);
                        Uconomy.Instance.HasBeenPayed((UnturnedPlayer)caller, otherPlayer, amount);
                    }
                }

            }
            else
            {
                ChatHelper.SendCommandReply(caller, "command_pay_error_player_not_found");
            }
        }
    }
}
