using Rocket.API;
using System.Collections.Generic;
using Tavstal.TLibrary.Utils;

namespace fr34kyn01535.Uconomy
{
    public class CommandBalance : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "balance";
        public string Help => "Shows the current balance";
        public string Syntax => "";
        public List<string> Aliases => new List<string> { "saldo" };
        public List<string> Permissions => new List<string> { "uconomy.balance", "uconomy.commands.balance" };

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            decimal balance = Uconomy.Instance.Database.GetBalance(caller.Id);
            ChatHelper.SendCommandReply(caller,"command_balance_show", balance, Uconomy.Instance.Configuration.Instance.MoneyName);
        }
    }
}
