using System;
using System.Collections.Generic;
using fr34kyn01535.Uconomy.Threading;
using fr34kyn01535.Uconomy.Utils;
using Rocket.API;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
// ReSharper disable UnusedType.Global

namespace fr34kyn01535.Uconomy.Commands
{
    /// <summary>
    /// Admin command that directly sets a player's balance
    /// to a specified amount, replacing their current balance.
    /// </summary>
    public class CommandSetBalance : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "setbalance";
        public string Help => "Sets the balance of a player";
        public string Syntax => "[player] [amount]";
        public List<string> Aliases => new List<string> { "setbal" };
        public List<string> Permissions => new List<string> { "uconomy.setbalance", "uconomy.commands.setbalance" };

        /// <summary>
        /// Executes the set balance command, updating the target player's
        /// balance to the specified amount.
        /// </summary>
        /// <param name="caller">The admin executing the command.</param>
        /// <param name="command">
        /// An array where:
        /// [0] is the name of the player whose balance to set,
        /// [1] is the new balance amount.
        /// </param>
        public void Execute(IRocketPlayer caller, params string[] command)
        {
            try
            {
                if (command.Length != 2)
                {
                    CommandUtils.SendCommandReply(caller,
                        Uconomy.Instance.Translate("command_setbalance_error_invalid", Uconomy.Instance.GetPrefix(),
                            Syntax));
                    return;
                }

                string targetId = command.GetCSteamIDParameter(0)?.ToString();
                UnturnedPlayer target = UnturnedPlayer.FromName(command[0]);
                if (target != null)
                    targetId = target.Id;

                if (string.IsNullOrEmpty(targetId))
                {
                    CommandUtils.SendCommandReply(caller,
                        Uconomy.Instance.Translate("command_pay_error_player_not_found", Uconomy.Instance.GetPrefix()));
                    return;
                }

                if (!decimal.TryParse(command[1], out decimal amount))
                {
                    CommandUtils.SendCommandReply(caller,
                        Uconomy.Instance.Translate("command_pay_error_invalid_amount", Uconomy.Instance.GetPrefix()));
                    return;
                }

                SafeTask.Run(() =>
                {
                    bool setResult = Uconomy.Instance.Database.SetBalance(targetId, amount);
                    MainThreadDispatcher.Run(() =>
                    {
                        if (setResult)
                        {
                            CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_setbalance_success",
                                Uconomy.Instance.GetPrefix(), amount,
                                Uconomy.Instance.Configuration.Instance.MoneyName, target?.CharacterName ?? targetId));

                            if (target != null)
                                CommandUtils.SendCommandReply(target, Uconomy.Instance.Translate(
                                    "command_setbalance_success_other",
                                    Uconomy.Instance.GetPrefix(), amount,
                                    Uconomy.Instance.Configuration.Instance.MoneyName));
                            return;
                        }
                        
                        CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_setbalance_error_generic",
                            Uconomy.Instance.GetPrefix()));
                    });
                }, GetType().Name);
            }
            catch (Exception ex)
            {
                CommandUtils.SendCommandReply(caller,
                    Uconomy.Instance.Translate("command_error_exception", Uconomy.Instance.GetPrefix()));
                Rocket.Core.Logging.Logger.LogError($"Failed to execute the '{this.GetType().Name}' command: ");
                Rocket.Core.Logging.Logger.LogException(ex);
            }
        }
    }
}
