using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using Tavstal.TLibrary.Utils;

namespace fr34kyn01535.Uconomy.Commands
{
    internal class CommandExchange : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "exchange";
        public string Help => "Exchanges the provided amount of your balance to the provided currency";
        public string Syntax => "[cash | xp] [amount]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "uconomy.exchange", "uconomy.commands.exchange" };

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length != 2)
            {
                ChatHelper.SendCommandReply(caller, "command_exchange_invalid", Syntax);
                return;
            }

            if (!decimal.TryParse(command[1], out decimal amount))
            {
                ChatHelper.SendCommandReply(caller, "command_pay_error_invalid_amount");
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
                            ChatHelper.SendCommandReply(caller, "command_exchange_cant_afford");
                            return;
                        }

                        Uconomy.Instance.Database.IncreaseBalance(caller.Id, -amount);
                        callerPlayer.Experience += (uint)amount;
                        ChatHelper.SendCommandReply(caller, "command_exchange_success");
                        break;
                    }
                case "xp":
                case "experience":
                    {
                        uint balance = callerPlayer.Experience;
                        if (balance < amount)
                        {
                            ChatHelper.SendCommandReply(caller, "command_exchange_cant_afford");
                            return;
                        }

                        Uconomy.Instance.Database.IncreaseBalance(caller.Id, amount);
                        callerPlayer.Experience -= (uint)amount;
                        ChatHelper.SendCommandReply(caller, "command_exchange_success");
                        break;
                    }
                default:
                    {
                        ChatHelper.SendCommandReply(caller, "command_exchange_invalid", Syntax);
                        return;
                    }
            }
        }
    }
}
