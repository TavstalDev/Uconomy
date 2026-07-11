using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using fr34kyn01535.Uconomy.Utils;
// ReSharper disable UnusedType.Global

namespace fr34kyn01535.Uconomy.Commands
{
    /// <summary>
    /// Command that exchanges money for experience or vice versa.
    /// Converts between the player's Uconomy balance and Unturned experience.
    /// </summary>
    public class CommandExchange : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "exchange";
        public string Help => "Exchanges the provided amount of your balance to the provided currency";
        public string Syntax => "[cash | xp] [amount]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "uconomy.exchange", "uconomy.commands.exchange" };

        /// <summary>
        /// Executes the exchange command, converting between cash and experience.
        /// </summary>
        /// <param name="caller">The player executing the command.</param>
        /// <param name="command">
        /// An array where:
        /// [0] is the currency type ("cash", "money", "xp", or "experience"),
        /// [1] is the amount to exchange.
        /// </param>
        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length != 2)
            {
                CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_exchange_error_invalid", Uconomy.Instance.GetPrefix(), Syntax));
                return;
            }

            if (!decimal.TryParse(command[1], out decimal amount))
            {
                CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_pay_error_invalid_amount", Uconomy.Instance.GetPrefix()));
                return;
            }
            if (amount <= 0)
                amount = Math.Abs(amount);

            UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
            switch (command[0].ToLower())
            {
                case "cash":
                case "money":
                    {
                        decimal balance = Uconomy.Instance.Database.GetBalance(caller.Id);
                        if (balance < amount)
                        {
                            CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_exchange_error_cant_afford", Uconomy.Instance.GetPrefix()));
                            return;
                        }

                        Uconomy.Instance.Database.IncreaseBalance(caller.Id, -amount);
                        callerPlayer.Experience += (uint)amount;
                        CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_exchange_success", Uconomy.Instance.GetPrefix()));
                        break;
                    }
                case "xp":
                case "experience":
                    {
                        uint balance = callerPlayer.Experience;
                        if (balance < amount)
                        {
                            CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_exchange_error_cant_afford", Uconomy.Instance.GetPrefix()));
                            return;
                        }

                        Uconomy.Instance.Database.IncreaseBalance(caller.Id, amount);
                        callerPlayer.Experience -= (uint)amount;
                        CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_exchange_success", Uconomy.Instance.GetPrefix()));
                        break;
                    }
                default:
                    {
                        CommandUtils.SendCommandReply(caller, Uconomy.Instance.Translate("command_exchange_error_invalid", Uconomy.Instance.GetPrefix(), Syntax));
                        return;
                    }
            }
        }
    }
}
