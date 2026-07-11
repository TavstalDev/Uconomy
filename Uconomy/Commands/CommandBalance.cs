using System.Collections.Generic;
using fr34kyn01535.Uconomy.Utils;
using Rocket.API;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
// ReSharper disable UnusedType.Global

namespace fr34kyn01535.Uconomy.Commands
{
    /// <summary>
    /// Command that displays the caller's current Uconomy balance.
    /// If a player name is provided and the caller has the required permission,
    /// it displays that player's balance instead.
    /// </summary>
    public class CommandBalance : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "balance";
        public string Help => "Shows the current balance";
        public string Syntax => "<player>";
        public List<string> Aliases => new List<string> { "saldo" };
        public List<string> Permissions => new List<string> { "uconomy.balance", "uconomy.commands.balance", "uconomy.commands.balance.other" };

        /// <summary>
        /// Executes the balance command, retrieving and displaying the balance
        /// for the caller or a specified target player.
        /// </summary>
        /// <param name="caller">The player executing the command.</param>
        /// <param name="command">
        /// An optional array where:
        /// [0] is the name of the player whose balance to check (requires permission).
        /// If empty, the caller's own balance is shown.
        /// </param>
        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length == 0)
            {
                if (caller is ConsolePlayer)
                {
                    CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_balance_error_invalid", Uconomy.Instance.GetPrefix(), Syntax));
                    return;
                }
                
                decimal balance = Uconomy.Instance.Database.GetBalance(caller.Id);
                CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_balance_show", Uconomy.Instance.GetPrefix(), balance, 
                    Uconomy.Instance.Configuration.Instance.MoneyName));
                return;
            }

            if (!caller.HasPermission("uconomy.commands.balance.other"))
            {
                CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_balance_other_forbidden", Uconomy.Instance.GetPrefix()));
                return;
            }

            string targetId  = command.GetCSteamIDParameter(0)?.ToString();
            UnturnedPlayer target = UnturnedPlayer.FromName(command[0]);
            if (target != null)
                targetId = target.Id;
            
            if (string.IsNullOrEmpty(targetId))
            {
                CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_pay_error_player_not_found", Uconomy.Instance.GetPrefix()));
                return;
            }

            decimal targetBalance = Uconomy.Instance.Database.GetBalance(targetId);
            CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate( "command_balance_show_other", Uconomy.Instance.GetPrefix(), targetBalance, 
                Uconomy.Instance.Configuration.Instance.MoneyName, target?.CharacterName ?? targetId));
        }
    }
}
