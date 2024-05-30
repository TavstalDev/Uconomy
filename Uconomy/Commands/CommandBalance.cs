using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TLibrary.Utils;

namespace fr34kyn01535.Uconomy
{
    public class CommandBalance : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "balance";
        public string Help => "Shows the current balance";
        public string Syntax => "<player>";
        public List<string> Aliases => new List<string> { "saldo" };
        public List<string> Permissions => new List<string> { "uconomy.balance", "uconomy.commands.balance", "uconomy.commands.balance.other" };

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length == 0)
            {
                decimal balance = Uconomy.Instance.Database.GetBalance(caller.Id);
                ChatHelper.SendCommandReply(caller, "command_balance_show", balance, Uconomy.Instance.Configuration.Instance.MoneyName);
                return;
            }

            if (!caller.HasPermission("uconomy.commands.balance.other"))
            {
                ChatHelper.SendCommandReply(caller, "command_balance_other_forbidden");
                return;
            }

            UnturnedPlayer target = UnturnedPlayer.FromName(command[0]);
            if (target == null)
            {
                ChatHelper.SendCommandReply(caller, "command_pay_error_player_not_found");
                return;
            }

            decimal targetBalance = Uconomy.Instance.Database.GetBalance(target.Id);

            ChatHelper.SendCommandReply(caller, "command_balance_show_other", targetBalance, Uconomy.Instance.Configuration.Instance.MoneyName, target.CharacterName);
        }
    }
}
