using System.Collections.Generic;
using fr34kyn01535.Uconomy.Utils;
using Rocket.API;
using Rocket.Unturned.Player;
// ReSharper disable UnusedType.Global

namespace fr34kyn01535.Uconomy.Commands
{
    /// <summary>
    /// Command that transfers money from the caller's balance to a target player's balance.
    /// </summary>
    public class CommandPay : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "pay";
        public string Help => "Pays a specific player money from your account";
        public string Syntax => "[player] [amount]";
        public List<string> Aliases => new List<string> { "pagar" };
        public List<string> Permissions => new List<string> { "uconomy.pay", "uconomy.commands.pay" };

        /// <summary>
        /// Transfers <paramref name="command"/>[1] money from the caller to the player specified in <paramref name="command"/>[0].
        /// </summary>
        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length != 2)
            {
                CommandUtils.SendCommandReply(caller,
                    Uconomy.Instance.Translate("command_pay_error_invalid", Uconomy.Instance.GetPrefix(), Syntax));
                return;
            }

            UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
            if (otherPlayer == null)
            {
                CommandUtils.SendCommandReply(caller,
                    Uconomy.Instance.Translate("command_pay_error_player_not_found", Uconomy.Instance.GetPrefix()));
                return;
            }

            if (Equals(caller, otherPlayer))
            {
                CommandUtils.SendCommandReply(caller,
                    Uconomy.Instance.Translate("command_pay_error_pay_self", Uconomy.Instance.GetPrefix()));
                return;
            }

            if (!decimal.TryParse(command[1], out var amount) || amount <= 0)
            {
                CommandUtils.SendCommandReply(caller,
                    Uconomy.Instance.Translate("command_pay_error_invalid_amount", Uconomy.Instance.GetPrefix()));
                return;
            }

            if (caller is ConsolePlayer)
            {
                Uconomy.Instance.Database.IncreaseBalance(otherPlayer.Id, amount);
                CommandUtils.SendCommandReply(otherPlayer,
                    Uconomy.Instance.Translate("command_pay_console", Uconomy.Instance.GetPrefix(), amount,
                        Uconomy.Instance.Configuration.Instance.MoneyName));
                return;
            }

            decimal myBalance = Uconomy.Instance.Database.GetBalance(caller.Id);
            if (myBalance - amount <= 0)
            {
                CommandUtils.SendCommandReply(caller,
                    Uconomy.Instance.Translate("command_pay_error_cant_afford", Uconomy.Instance.GetPrefix()));
                return;
            }

            Uconomy.Instance.Database.IncreaseBalance(caller.Id, -amount);
            CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_pay_private",
                Uconomy.Instance.GetPrefix(), otherPlayer.CharacterName,
                amount, Uconomy.Instance.Configuration.Instance.MoneyName));
            Uconomy.Instance.Database.IncreaseBalance(otherPlayer.Id, amount);
            CommandUtils.SendCommandReply(otherPlayer, Uconomy.Instance.Translate("command_pay_other_private",
                Uconomy.Instance.GetPrefix(), amount,
                Uconomy.Instance.Configuration.Instance.MoneyName, caller.DisplayName));
            Uconomy.Instance.HasBeenPayed((UnturnedPlayer)caller, otherPlayer, amount);
        }
    }
}
