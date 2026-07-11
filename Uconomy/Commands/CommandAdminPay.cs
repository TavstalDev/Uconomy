using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using fr34kyn01535.Uconomy.Utils;
// ReSharper disable UnusedType.Global

namespace fr34kyn01535.Uconomy.Commands
{
    /// <summary>
    /// Admin command that adds money directly to a player's balance.
    /// Unlike the regular pay command, this does not deduct from the caller's account.
    /// </summary>
    public class CommandAdminPay : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "adminpay";
        public string Help => "Pays a specific player money.";
        public string Syntax => "[player] [amount]";
        public List<string> Aliases => new List<string> { "apay" };
        public List<string> Permissions => new List<string> { "uconomy.adminpay", "uconomy.commands.adminpay" };

        /// <summary>
        /// Executes the admin pay command, adding the specified amount
        /// to the target player's balance.
        /// </summary>
        /// <param name="caller">The admin or console executing the command.</param>
        /// <param name="command">
        /// An array where:
        /// [0] is the name of the player to pay,
        /// [1] is the amount of money to add.
        /// </param>
        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length != 2)
            {
                CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_adminpay_error_invalid", Uconomy.Instance.GetPrefix(), Syntax));
                return;
            }

            UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
            if (otherPlayer != null)
            {
                if (!decimal.TryParse(command[1], out var amount) || amount <= 0)
                {
                    CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_pay_error_invalid_amount", Uconomy.Instance.GetPrefix()));
                    return;
                }

                Uconomy.Instance.Database.IncreaseBalance(otherPlayer.Id, amount);
                CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_pay_private", Uconomy.Instance.GetPrefix(), otherPlayer.CharacterName, 
                    amount, Uconomy.Instance.Configuration.Instance.MoneyName));

                CommandUtils.SendCommandReply(otherPlayer, Uconomy.Instance.Translate("command_pay_console", Uconomy.Instance.GetPrefix(), amount, 
                    Uconomy.Instance.Configuration.Instance.MoneyName));
                return;
            }

            CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_pay_error_player_not_found", Uconomy.Instance.GetPrefix()));
        }
    }
}
