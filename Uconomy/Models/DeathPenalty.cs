using SDG.Unturned;
using System;

namespace fr34kyn01535.Uconomy.Models
{
    [Serializable]
    public class DeathPenalty
    {
        public EDeathCause Cause { get; set; }
        public bool Enable { get; set; }
        public decimal Fine { get; set; }

        public DeathPenalty(EDeathCause cause, bool enable, decimal fine) { Cause = cause; Enable = enable; Fine = fine; }

        public DeathPenalty() { }
    }
}
